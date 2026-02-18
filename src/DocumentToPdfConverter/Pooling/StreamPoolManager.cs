using System.Runtime.CompilerServices;
using Microsoft.IO;

namespace DocumentToPdfConverter.Pooling;

internal static class StreamPoolManager
{
    private static readonly RecyclableMemoryStreamManager s_manager = new(
        new RecyclableMemoryStreamManager.Options
        {
            BlockSize = 128 * 1024,
            LargeBufferMultiple = 1024 * 1024,
            MaximumBufferSize = 64 * 1024 * 1024,
            MaximumSmallPoolFreeBytes = 16 * 1024 * 1024,
            MaximumLargePoolFreeBytes = 64 * 1024 * 1024,
            AggressiveBufferReturn = true,
        });

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MemoryStream GetStream(string tag)
        => s_manager.GetStream(tag);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MemoryStream GetStream(string tag, int requiredSize)
        => s_manager.GetStream(tag, requiredSize);
}
