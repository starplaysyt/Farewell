using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Farewell.Abstractions.Domain;

namespace Farewell.Debug.TestEntities;

[Table("test_a_entities")]
public class TestAEntity : DomainEntity<uint>
{
    [Key, Column("test_key"), MaxLength(32)]
    public required string TestKey { get; set; }

    [Column("test_nullable_field")]
    public int? TestNullableField { get; set; }
    
    [Column("test_string_nullable_field"), MaxLength(32)]
    public string? TestStringNullableField { get; set; }

    [Column("test_not_nullable_field"), MaxLength(32)]
    public required string TestNotNullableField { get; set; }

    [Column("test_not_updatable_field"), MaxLength(32)]
    public required string TestNotUpdatableField { get; set; }
}

public record TestAEntityUpdateMap(
    string? TestKey,
    NullableField<int?>? TestNullableField,
    NullableField<string?>? TestStringNullableField,
    string? TestNotNullableField
);