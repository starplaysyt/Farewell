using System.Text.Json;

namespace Farewell.Abstractions.Configuration;

public interface IConfigParser
{
    JsonElement Parse();
}