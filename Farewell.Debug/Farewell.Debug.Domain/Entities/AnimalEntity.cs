using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Farewell.Abstractions.Attributes.Domain;
using Farewell.Abstractions.Domain;

namespace Farewell.Debug.Domain.Entities;

public class AnimalEntity : DomainEntity<uint>
{
    [Identity, Column("identifier"), MaxLength(128)]
    public string Identifier { get; set; }
    
    
}