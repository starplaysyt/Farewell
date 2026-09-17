using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farewell.Abstractions.Domain;

public abstract class DomainEntity<TKey> where TKey : IComparable<TKey>
{
    [Key, Column("id")]
    public TKey Id { get; set; }
}