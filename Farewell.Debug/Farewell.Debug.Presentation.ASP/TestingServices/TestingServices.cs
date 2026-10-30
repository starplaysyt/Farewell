namespace Farewell.Debug.Presentation.ASP.TestingServices;

public interface IUnkeyedSingletonTest
{
    Guid Id { get; }
}

public interface IUnkeyedScopedTest
{
    Guid Id { get; }
}

public interface IUnkeyedTransientTest
{
    Guid Id { get; }
}

public sealed class UnkeyedSingletonTest : IUnkeyedSingletonTest
{
    public Guid Id { get; } = Guid.NewGuid();
}

public sealed class UnkeyedScopedTest : IUnkeyedScopedTest
{
    public Guid Id { get; } = Guid.NewGuid();
}

public sealed class UnkeyedTransientTest : IUnkeyedTransientTest
{
    public Guid Id { get; } = Guid.NewGuid();
}

public interface IKeyedTest
{
    string Key { get; }
    Guid Id { get; }
}

public sealed class KeyedTest(string key) : IKeyedTest
{
    public string Key { get; } = key;
    public Guid Id { get; } = Guid.NewGuid();
}

