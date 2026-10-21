using System.Buffers;
using System.Linq.Expressions;
using System.Reflection;
using Farewell.Abstractions.Presentation;

namespace Farewell.Presentation.Internal;

internal sealed class PresentationCompiler(
    IReadOnlyDictionary<(Type, PresentationContext), Registration> registrations)
{
    private readonly Dictionary<(Type, PresentationContext), Func<object, string>>
        _compiled = new();

    private readonly HashSet<(Type, PresentationContext)> _inProgress = [];

    private static readonly Type StateType = typeof(ValueTuple<string[], int>);

    private static readonly ConstructorInfo StateCtor =
        StateType.GetConstructor([typeof(string[]), typeof(int)])!;

    private static readonly PropertyInfo PoolSharedProperty =
        typeof(ArrayPool<string>).GetProperty(nameof(ArrayPool<string>.Shared))!;

    private static readonly MethodInfo RentMethod =
        typeof(ArrayPool<string>).GetMethod(nameof(ArrayPool<string>.Rent))!;

    private static readonly MethodInfo ReturnMethod =
        typeof(ArrayPool<string>).GetMethod(nameof(ArrayPool<string>.Return))!;

    private static readonly PropertyInfo StringLengthProperty =
        typeof(string).GetProperty(nameof(string.Length))!;

    private static readonly MethodInfo StringCreateMethod =
        typeof(string)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m is { Name: nameof(string.Create), IsGenericMethodDefinition: true }
                        && m.GetGenericArguments().Length == 1
                        && m.GetParameters().Length == 3)
            .MakeGenericMethod(StateType);

    public Dictionary<(Type, PresentationContext), Func<object, string>> CompileAll()
    {
        foreach (var key in registrations.Keys)
            CompileOne(key);

        return _compiled;
    }

    private Func<object, string> CompileOne((Type type, PresentationContext ctx) key)
    {
        if (_compiled.TryGetValue(key, out var cached))
            return cached;

        if (!_inProgress.Add(key))
            throw new InvalidOperationException(
                $"Circular dependency detected while compiling presentation for " +
                $"{key.type.FullName} in context {key.ctx.Id}");

        try
        {
            if (!registrations.TryGetValue(key, out var reg))
                throw new InvalidOperationException(
                    $"No registration for {key.type.FullName} in context {key.ctx.Id}");

            var compiled = reg switch
            {
                SimpleRegistration s => CompileSimple(s),
                AggregateRegistration a => CompileAggregate(a),
                StatedRegistration st => CompileStated(st),
                _ => throw new NotSupportedException()
            };

            _compiled[key] = compiled;
            return compiled;
        }
        finally
        {
            _inProgress.Remove(key);
        }
    }

    private Func<object, string> CompileSimple(SimpleRegistration reg)
    {
        var targetParam = Expression.Parameter(typeof(object), "target");
        var typedTarget = Expression.Variable(reg.TargetType, "e");

        var body = BuildFormatExpression(reg.Template, typedTarget, reg.TargetType, reg.Context,
            reg.CustomBindings);

        var block = Expression.Block(
            [typedTarget],
            Expression.Assign(typedTarget, Expression.Convert(targetParam, reg.TargetType)),
            body);

        return Expression.Lambda<Func<object, string>>(block, targetParam).Compile();
    }

    /// <summary>
    /// Builds the full string.Create expression for a given template and target.
    /// Returns Expression of type string.
    /// </summary>
    private BlockExpression BuildFormatExpression(
        ParsedTemplate tpl,
        Expression target,
        Type targetType,
        PresentationContext context,
        IReadOnlyDictionary<string, LambdaExpression>? customBindings)
    {
        var bufSize = tpl.Literals.Length + tpl.Slots.Length;

        var pool = Expression.Property(null, PoolSharedProperty);
        var buf = Expression.Variable(typeof(string[]), "buf");
        var rent = Expression.Call(pool, RentMethod, Expression.Constant(bufSize));

        var assignments = new List<Expression>(bufSize);
        var slotLengths = new List<Expression>(tpl.Slots.Length);

        var bufIndex = 0;
        for (var i = 0; i < tpl.Literals.Length; i++)
        {
            assignments.Add(Expression.Assign(
                Expression.ArrayAccess(buf, Expression.Constant(bufIndex)),
                Expression.Constant(tpl.Literals[i])));
            bufIndex++;

            if (i >= tpl.Slots.Length) continue;

            var slotValue = BuildSlotValue(
                tpl.Slots[i], target, targetType, context, customBindings);

            var cell = Expression.ArrayAccess(buf, Expression.Constant(bufIndex));
            assignments.Add(Expression.Assign(cell, slotValue));
            slotLengths.Add(Expression.Property(cell, StringLengthProperty));
            bufIndex++;
        }

        Expression totalLen = Expression.Constant(tpl.ConstLiteralsLength);
        totalLen = slotLengths.Aggregate(totalLen, Expression.Add);

        // state = new ValueTuple<string[], int>(buf, bufSize)
        var state = Expression.New(StateCtor, buf, Expression.Constant(bufSize));

        var createCall = Expression.Call(
            StringCreateMethod,
            totalLen,
            state,
            Expression.Constant(PresentationWriter.Instance));

        var result = Expression.Variable(typeof(string), "result");

        var tryBody = Expression.Block(
            [result],
            Expression.Block(assignments),
            Expression.Assign(result, createCall),
            result);

        var finallyBody = Expression.Call(
            pool, ReturnMethod, buf, Expression.Constant(false));

        return Expression.Block(
            [buf],
            Expression.Assign(buf, rent),
            Expression.TryFinally(tryBody, finallyBody));
    }

    private Expression BuildSlotValue(
        TemplateSlot slot,
        Expression target,
        Type targetType,
        PresentationContext context,
        IReadOnlyDictionary<string, LambdaExpression>? customBindings)
    {
        Expression rawValue;
        Type rawType;

        if (slot.IsRecursive && string.IsNullOrEmpty(slot.Name))
        {
            rawValue = target;
            rawType = targetType;
        }
        else if (customBindings != null && customBindings.TryGetValue(slot.Name, out var binding))
        {
            rawValue = ReplaceParameter(binding.Body, binding.Parameters[0], target);
            rawType = binding.ReturnType;
        }
        else
        {
            var prop = targetType.GetProperty(slot.Name,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop == null)
                throw new InvalidOperationException(
                    $"Template placeholder '{{{(slot.IsRecursive ? "$" : "")}{slot.Name}}}' " +
                    $"not resolved: no property '{slot.Name}' on {targetType.FullName} and no custom binding.");

            rawValue = Expression.Property(target, prop);
            rawType = prop.PropertyType;
        }

        if (!slot.IsRecursive) return BuildToStringWithNullFallback(rawValue, rawType);

        var childKey = (rawType, context);
        var childDelegate = CompileOne(childKey);

        var childConst = Expression.Constant(childDelegate);
        var boxed = rawType.IsValueType
            ? Expression.Convert(rawValue, typeof(object))
            : rawValue;

        var invokeCall = Expression.Invoke(childConst, boxed);

        var canBeNull = (!rawType.IsValueType || Nullable.GetUnderlyingType(rawType) != null)
                        && !(slot.IsRecursive && string.IsNullOrEmpty(slot.Name));

        if (canBeNull)
        {
            return Expression.Condition(
                Expression.Equal(rawValue, Expression.Constant(null, rawType)),
                Expression.Constant("null"),
                invokeCall);
        }

        return invokeCall;
    }

    private static Expression BuildToStringWithNullFallback(Expression value, Type type)
    {
        if (type == typeof(string))
        {
            return Expression.Coalesce(value, Expression.Constant("null"));
        }

        if (type.IsValueType && Nullable.GetUnderlyingType(type) == null)
        {
            // non-nullable value type - just ToString, no null check needed
            return Expression.Call(value, type.GetMethod("ToString", Type.EmptyTypes)!);
        }

        // reference type or nullable: null-check
        var toStringCall = Expression.Call(value, nameof(ToString), Type.EmptyTypes);
        return Expression.Condition(
            Expression.Equal(value, Expression.Constant(null, type)),
            Expression.Constant("null"),
            Expression.Coalesce(toStringCall, Expression.Constant("null")));
    }

    private Func<object, string> CompileAggregate(AggregateRegistration reg)
    {
        var itemFunc = CompileAggregateItemPresenter(reg);
        var headerFunc = CompileAggregateHeaderPresenter(reg);

        // Compile source extractor: object -> IEnumerable
        var sourceCompiled = CompileSourceExtractor(reg.Source, reg.TargetType);
        var separator = reg.Separator;

        return target =>
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(headerFunc(target));

            var items = sourceCompiled(target);

            var first = true;
            foreach (var item in items)
            {
                if (!first && separator.Length > 0)
                    sb.Append(separator);
                first = false;
                sb.Append(itemFunc(item!));
            }

            return sb.ToString();
        };
    }

    private static Func<object, System.Collections.IEnumerable> CompileSourceExtractor(
        LambdaExpression source, Type targetType)
    {
        var objParam = Expression.Parameter(typeof(object), "target");
        var typedTarget = Expression.Convert(objParam, targetType);

        // Inline the body: replace source.Parameters[0] with typedTarget
        var inlinedBody = ReplaceParameter(source.Body, source.Parameters[0], typedTarget);

        // Convert IEnumerable<TItem> -> IEnumerable (always safe)
        var asNonGeneric = Expression.Convert(inlinedBody, typeof(System.Collections.IEnumerable));

        return Expression
            .Lambda<Func<object, System.Collections.IEnumerable>>(asNonGeneric, objParam).Compile();
    }

    private Func<object, string> CompileAggregateHeaderPresenter(AggregateRegistration reg)
    {
        var targetParam = Expression.Parameter(typeof(object), "target");
        var typedTarget = Expression.Variable(reg.TargetType, "e");

        var body = BuildFormatExpression(reg.HeaderTemplate, typedTarget, reg.TargetType,
            reg.Context, null);

        var block = Expression.Block(
            [typedTarget],
            Expression.Assign(typedTarget, Expression.Convert(targetParam, reg.TargetType)),
            body);

        return Expression.Lambda<Func<object, string>>(block, targetParam).Compile();
    }

    private Func<object, string> CompileAggregateItemPresenter(AggregateRegistration reg)
    {
        var itemParam = Expression.Parameter(typeof(object), "item");
        var typedItem = Expression.Variable(reg.ItemType, "it");

        var body =
            BuildFormatExpression(reg.ItemTemplate, typedItem, reg.ItemType, reg.Context, null);

        var block = Expression.Block(
            [typedItem],
            Expression.Assign(typedItem, Expression.Convert(itemParam, reg.ItemType)),
            body);

        return Expression.Lambda<Func<object, string>>(block, itemParam).Compile();
    }

    private Func<object, string> CompileStated(StatedRegistration reg)
    {
        // Compile selector: object -> TState (boxed as object for dict lookup)
        var selectorCompiled = CompileSelector(reg.Selector, reg.TargetType);

        // Compile each case template to Func<object,string>
        var cases = new Dictionary<object, Func<object, string>>(reg.Cases.Count);
        foreach (var (state, tpl) in reg.Cases)
            cases[state] = CompileTemplateAsFunc(tpl, reg.TargetType, reg.Context);

        var fallback = reg.Fallback is { } fb
            ? CompileTemplateAsFunc(fb, reg.TargetType, reg.Context)
            : null;

        return target =>
        {
            var state = selectorCompiled(target);
            if (cases.TryGetValue(state, out var fn))
                return fn(target);
            if (fallback != null)
                return fallback(target);
            throw new InvalidOperationException(
                $"No stated presentation case for state '{state}' and no fallback.");
        };
    }

    /// <summary>
    /// Compiles LambdaExpression of shape <c>(T t) => TState</c>
    /// into <c>Func&lt;object, object&gt;</c> (boxing value-type state for dictionary lookup).
    /// </summary>
    private static Func<object, object> CompileSelector(LambdaExpression selector, Type targetType)
    {
        var objParam = Expression.Parameter(typeof(object), "target");
        var typedTarget = Expression.Convert(objParam, targetType);

        var inlinedBody = ReplaceParameter(selector.Body, selector.Parameters[0], typedTarget);

        // Box to object (needed for Dictionary<object,...> lookup)
        var boxed = selector.ReturnType.IsValueType
            ? Expression.Convert(inlinedBody, typeof(object))
            : inlinedBody;

        return Expression.Lambda<Func<object, object>>(boxed, objParam).Compile();
    }

    private Func<object, string> CompileTemplateAsFunc(ParsedTemplate tpl, Type targetType,
        PresentationContext context)
    {
        var targetParam = Expression.Parameter(typeof(object), "target");
        var typedTarget = Expression.Variable(targetType, "e");

        var body = BuildFormatExpression(tpl, typedTarget, targetType, context, null);

        var block = Expression.Block(
            [typedTarget],
            Expression.Assign(typedTarget, Expression.Convert(targetParam, targetType)),
            body);

        return Expression.Lambda<Func<object, string>>(block, targetParam).Compile();
    }

    private static Expression ReplaceParameter(Expression body, ParameterExpression oldParam,
        Expression newExpr)
    {
        return new ParameterReplacer(oldParam, newExpr).Visit(body)!;
    }

    private sealed class ParameterReplacer(ParameterExpression old, Expression @new)
        : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
            => node == old ? @new : base.VisitParameter(node);
    }
}