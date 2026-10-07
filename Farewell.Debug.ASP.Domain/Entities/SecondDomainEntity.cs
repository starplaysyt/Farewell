using Farewell.Abstractions.Domain;

namespace Farewell.Debug.ASP.Domain.Entities;

public class SecondDomainEntity : DomainEntity<uint>
{
    public required string TestField { get; set; }
}