using System.Buffers;

namespace DocumentToPdfConverter.Internal;

internal static class StreamCopyHelper
{
    private const int DefaultBufferSize = 81920;

    public static async ValueTask CopyToAsync(
        Stream source,
        Stream destination,
        CancellationToken ct)
    {
        byte[] buffer = ArrayPool<byte>.Shared.Rent(DefaultBufferSize);
        try
        {
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(
                buffer.AsMemory(0, DefaultBufferSize), ct)
                .ConfigureAwait(false)) > 0)
            {
                await destination.WriteAsync(
                    buffer.AsMemory(0, bytesRead), ct)
                    .ConfigureAwait(false);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
