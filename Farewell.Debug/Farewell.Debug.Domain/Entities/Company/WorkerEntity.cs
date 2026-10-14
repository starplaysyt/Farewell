using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Farewell.Abstractions.Attributes.Domain;
using Farewell.Abstractions.Domain;

namespace Farewell.Debug.Domain.Entities.Company;

[Table("workers")]
public class WorkerEntity : DomainEntity<uint>
{
    [Identity, Column("identifier")]
    public required string Identifier { get; set; }
    
    [Column("first_name"), MaxLength(32)]
    public required string FirstName { get; set; }
    
    [Column("last_name"), MaxLength(32)]
    public required string LastName { get; set; }
}

public record WorkerEntityUpdateMap(
    string? Identifier,
    string? FirstName,
    string? LastName);