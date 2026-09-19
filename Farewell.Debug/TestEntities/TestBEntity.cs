using Farewell.Abstractions.Attributes.Domain;
using Farewell.Abstractions.Domain;

namespace Farewell.Debug.TestEntities;

public class TestBEntity : TestCEntity
{
    public required string FieldB1 { get; set; }
    public required string FieldB2 { get; set; }
    public required string FieldB3 { get; set; }
}

[Inheritance(InheritanceStrategy.Tpc)]
public class TestCEntity : DomainEntity<uint>
{
    public required string FieldC1 { get; set; }
    public required string FieldC2 { get; set; }
    public required string FieldC3 { get; set; }
}

public class TestDEntity : TestCEntity
{
    public required string FieldD1 { get; set; }
    public required string FieldD2 { get; set; }
    public required string FieldD3 { get; set; }
}