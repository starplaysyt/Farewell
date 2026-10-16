namespace Farewell.Abstractions.Mediator;

public abstract record Command<TSelf, TResult> : IResultOperation<TSelf, TResult>
{
    
}

public abstract record Command<TSelf> : IResultOperation<TSelf>
{
    
}