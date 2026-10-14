using Farewell.Abstractions.Logging;

namespace Farewell.Debug.Tests.StateRules;

public sealed class FakeLogger : ILogger
{
    public List<(LogLevel Level, string Message, Exception? Exception)> Entries { get; } = new();

    public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log(LogLevel level, string message, string sender, Exception? exception = null)
    {
        Entries.Add((level, message, exception));
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();
        public void Dispose() { }
    }
}