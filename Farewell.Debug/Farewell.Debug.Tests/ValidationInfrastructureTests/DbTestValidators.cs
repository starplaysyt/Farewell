using Farewell.Abstractions.Attributes.Validation;
using Farewell.Abstractions.Extensions;
using Farewell.Abstractions.Validation;

namespace Farewell.Debug.Tests.ValidationInfrastructureTests;

[ValidationContext("DbCreate")]
public sealed class CreateUserDbValidator : FluentValidator<CreateUserCommand>
{
    protected override void DefineRules(IConstraintBuilder<CreateUserCommand> b)
    {
        b.Property(x => x.Email)
            .NotEmpty()
            .DbUnique<string?, UserEntity>(
                (email, sp, q) => q.Where(u => u.Email == email));

        b.Property(x => x.Username)
            .NotEmpty()
            .DbNotExists<string?, UserEntity>(
                (name, sp, q) => q.Where(u => u.Username == name));

        b.Property(x => x.DepartmentId)
            .Positive()
            .DbExists<int, DepartmentEntity>(
                (id, sp, q) => q.Where(d => d.Id == id));
    }
}

[ValidationContext("DbUpdate")]
public sealed class UpdateUserDbValidator : FluentValidator<UpdateUserCommand>
{
    protected override void DefineRules(IConstraintBuilder<UpdateUserCommand> b)
    {
        b.Property(x => x.Email)
            .NotEmpty()
            .DbUnique<string?, UserEntity>(
                (email, sp, q) => q.Where(u => u.Email == email && u.Id != sp.GetRequiredService<UpdateUserCommand>().Id));
        // Note: ugly SP trick for demo — real code would capture entity differently
    }
}

[ValidationContext("DbOrder")]
public sealed class CreateOrderDbValidator : FluentValidator<CreateOrderCommand>
{
    protected override void DefineRules(IConstraintBuilder<CreateOrderCommand> b)
    {
        b.Property(x => x.UserId)
            .Positive()
            .DbExists<int, UserEntity>(
                (id, sp, q) => q.Where(u => u.Id == id))
            .DbCount<int, OrderEntity>(
                (userId, sp, q) => q.Where(o => o.UserId == userId && o.IsActive),
                min: 0, max: 5);
    }
}

[ValidationContext("DbRole")]
public sealed class AssignRoleDbValidator : FluentValidator<AssignRoleCommand>
{
    protected override void DefineRules(IConstraintBuilder<AssignRoleCommand> b)
    {
        b.Property(x => x.Role)
            .NotEmpty()
            .DbAny<string?, UserEntity>(
                (sp, q) => q.Where(u => u.Role == "Admin"));

        b.Property(x => x.AccountId)
            .DbNone<int, OrderEntity>(
                (sp, q) => q.Where(o => o.IsActive && o.UserId == 999));
    }
}