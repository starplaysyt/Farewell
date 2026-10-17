using Farewell.Abstractions.Domain;

namespace Farewell.Debug.Tests.ValidationInfrastructureTests;

public sealed class UserEntity : DomainEntity<uint>
{
    public string Email { get; set; } = "";
    public string Username { get; set; } = "";
    public string Role { get; set; } = "";
    public bool IsActive { get; set; }
}

public sealed class DepartmentEntity : DomainEntity<uint>
{
    public string Name { get; set; } = "";
}

public sealed class OrderEntity : DomainEntity<uint>
{
    public int UserId { get; set; }
    public bool IsActive { get; set; }
}