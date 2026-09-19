using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Farewell.Abstractions.Attributes.Domain;

namespace Farewell.Abstractions.Domain;

public abstract class DomainEntity<TKey> where TKey : IComparable<TKey>
{
    [Key, Column("id")]
    public TKey Id { get; set; }
}