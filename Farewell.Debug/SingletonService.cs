using Farewell.Abstractions.DI;
using Farewell.Abstractions.Extensions;

namespace Farewell.Debug;

public class SingletonService(IScopeProvider provider)
{
    public void DoSomeStuff()
    {
        using var scope = provider.CreateScope();

        Console.WriteLine("Complete");
    }
}