namespace Farewell.Abstractions.Mediator;

public abstract record Query<TSelf, TResult> : IResultOperation<TSelf, TResult>
{
    
}

public abstract record Query<TSelf> : IResultOperation<TSelf>
{
    
}