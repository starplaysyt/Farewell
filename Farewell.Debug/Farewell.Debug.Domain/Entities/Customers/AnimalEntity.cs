using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Farewell.Abstractions.Attributes.Domain;
using Farewell.Abstractions.Domain;

namespace Farewell.Debug.Domain.Entities.Customers;

[Table("workers")]
public class AnimalEntity : DomainEntity<uint>
{
    [Identity, Column("identifier"), MaxLength(128)]
    public string Identifier { get; set; }
    
    [Column("name"), MaxLength(32)]
    public string Name { get; set; }
    
    [Column("description")]
    public string Description { get; set; }
}

public record AnimalEntityUpdateMap(
    string? Identifier,
    string? Name,
    string? Description);