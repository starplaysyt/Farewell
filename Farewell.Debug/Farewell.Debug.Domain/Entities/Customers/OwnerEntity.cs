using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Farewell.Abstractions.Domain;

namespace Farewell.Debug.Domain.Entities.Customers;

[Table("owners")]
public class OwnerEntity : DomainEntity<uint>
{
    [Column("first_name"), MaxLength(32)]
    public required string FirstName { get; set; }
    [Column("last_name"), MaxLength(32)]
    public required string LastName { get; set; }
}

public record OwnerEntityUpdateMap(
    string? FirstName,
    string? LastName
    );