using System.Linq.Expressions;
using System.Reflection;
using Farewell.Abstractions.DI;

namespace Farewell.DI.Internal;

internal static class ExpressionFactoryCompiler
{
    public static Func<IServiceProvider, object> Compile(
        ConstructorInfo ctor, ParameterInfo[] parameters)
    {
        var spParam = Expression.Parameter(typeof(IServiceProvider), "sp");
        var args = new Expression[parameters.Length];

        var getServiceMethod = typeof(IServiceProvider)
            .GetMethod(nameof(IServiceProvider.GetService))!;
        var getKeyedMethod = typeof(IKeyedServiceProvider)
            .GetMethod(nameof(IKeyedServiceProvider.GetKeyedService))!;

        for (int i = 0; i < parameters.Length; i++)
        {
            var param = parameters[i];
            var keyAttr = param.GetCustomAttribute<FromKeyedServicesAttribute>();

            Expression resolveExpr;

            if (keyAttr is not null)
            {
                var cast = Expression.Convert(spParam, typeof(IKeyedServiceProvider));
                resolveExpr = Expression.Call(cast, getKeyedMethod,
                    Expression.Constant(param.ParameterType, typeof(Type)),
                    Expression.Constant(keyAttr.Key, typeof(object)));
            }
            else
            {
                resolveExpr = Expression.Call(spParam, getServiceMethod,
                    Expression.Constant(param.ParameterType, typeof(Type)));
            }

            args[i] = Expression.Convert(resolveExpr, param.ParameterType);
        }

        var body = Expression.Convert(Expression.New(ctor, args), typeof(object));
        return Expression.Lambda<Func<IServiceProvider, object>>(body, spParam).Compile();
    }

    public static Func<IServiceProvider, object> Compile(ConstructorInfo ctor)
    {
        return Compile(ctor, ctor.GetParameters());
    }
}