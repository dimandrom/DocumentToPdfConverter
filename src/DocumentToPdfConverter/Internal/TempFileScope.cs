namespace DocumentToPdfConverter.Internal;

internal readonly struct TempFileScope : IAsyncDisposable
{
    public string FilePath { get; }

    private TempFileScope(string filePath) => FilePath = filePath;

    public static TempFileScope Create(string extension)
    {
        var path = Path.Combine(
            Path.GetTempPath(),
            $"dpc_{Guid.NewGuid():N}{extension}");
        return new TempFileScope(path);
    }

    public ValueTask DisposeAsync()
    {
        try
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
        catch
        {
            // Best-effort cleanup — do not throw from Dispose
        }

        return ValueTask.CompletedTask;
    }
}
