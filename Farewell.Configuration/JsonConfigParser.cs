using System.Text.Json;
using Farewell.Abstractions.Configuration;

namespace Farewell.Configuration;

public class JsonConfigParser : IWatchableConfigParser
{
    public string FilePath { get; }

    public JsonConfigParser(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Configuration file not found: '{filePath}'");
            
        FilePath = filePath;
    }

    public JsonElement Parse()
    {
        var json = File.ReadAllText(FilePath);
        return JsonDocument.Parse(json).RootElement.Clone();
    }
}