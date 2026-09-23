using System.Text.Json;
using Farewell.Configuration;

namespace Farewell.Debug.Tests.Configuration;

public class JsonConfigParserTests : IDisposable
{
    private readonly string _tempFile = Path.GetTempFileName() + ".json";

    [Fact]
    public void Parse_ValidJson_ReturnsRootElement()
    {
        var json = """{ "server": { "port": 8080 } }""";
        File.WriteAllText(_tempFile, json);

        var parser = new JsonConfigParser(_tempFile);
        var element = parser.Parse();

        Assert.Equal(JsonValueKind.Object, element.ValueKind);
    }

    [Fact]
    public void Parse_FileNotFound_ThrowsFileNotFoundException()
    {
        Assert.Throws<FileNotFoundException>(() =>
            new JsonConfigParser("nonexistent.json")
        );
    }

    [Fact]
    public void Parse_InvalidJson_ThrowsJsonException()
    {
        File.WriteAllText(_tempFile, "not a json");
        var parser = new JsonConfigParser(_tempFile);

        Assert.ThrowsAny<JsonException>(() => parser.Parse());
    }

    [Fact]
    public void Parse_ReturnsClonedElement_IndependentOfDocument()
    {
        var json = """{ "key": "value" }""";
        File.WriteAllText(_tempFile, json);
        var parser = new JsonConfigParser(_tempFile);

        var element = parser.Parse();

        Assert.Equal("value", element.GetProperty("key").GetString());
    }

    [Fact]
    public void FilePath_ReturnsCorrectPath()
    {
        File.WriteAllText(_tempFile, "{}");
        var parser = new JsonConfigParser(_tempFile);

        Assert.Equal(_tempFile, parser.FilePath);
    }

    public void Dispose() => File.Delete(_tempFile);
}