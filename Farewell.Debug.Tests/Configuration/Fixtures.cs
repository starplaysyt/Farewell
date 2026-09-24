namespace Farewell.Debug.Tests.Configuration;

internal sealed class TestConfig
{
    public string Name { get; set; } = "Default";
    public int Value { get; set; } = 42;
}

internal sealed class NestedConfig
{
    public string Host { get; set; } = "localhost";
    public InnerConfig Inner { get; set; } = new();
}

internal sealed class InnerConfig
{
    public int Port { get; set; } = 8080;
}