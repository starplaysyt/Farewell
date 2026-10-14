namespace Farewell.Debug.Tests.Configuration;

public sealed class TempDirectoryFixture : IDisposable
{
    public string BasePath { get; } = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

    public TempDirectoryFixture()
    {
        Directory.CreateDirectory(BasePath);
    }

    public string GetFilePath(string fileName) => Path.Combine(BasePath, fileName);

    public void Dispose()
    {
        if (Directory.Exists(BasePath))
            Directory.Delete(BasePath, recursive: true);
    }
}