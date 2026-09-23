namespace Farewell.Abstractions.Configuration;

public interface IWatchableConfigParser : IConfigParser
{
    string FilePath { get; }
}