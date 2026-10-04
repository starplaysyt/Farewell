using System.ComponentModel.DataAnnotations.Schema;
using Farewell.Abstractions.Domain;

namespace Farewell.Debug.ASP.Domain.Entities;

public class FirstDomainEntity : DomainEntity<uint>
{
    [Column("field1")]
    public required string Field1 { get; set; }
    
    [Column("field2")]
    public required string Field2 { get; set; }
}

public record FirstDomainEntityUpdateMap(
    string? Field1,
    string? Field2);