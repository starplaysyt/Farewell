namespace Farewell.Debug.Tests.ValidationInfrastructureTests;

public sealed class CreateUserCommand
{
    public string? Email { get; set; }
    public string? Username { get; set; }
    public int DepartmentId { get; set; }
}

public sealed class UpdateUserCommand
{
    public int Id { get; set; }
    public string? Email { get; set; }
}

public sealed class CreateOrderCommand
{
    public int UserId { get; set; }
}

public sealed class AssignRoleCommand
{
    public string? Role { get; set; }
    public int AccountId { get; set; }
}