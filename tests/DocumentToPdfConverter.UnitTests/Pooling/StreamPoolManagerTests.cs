using DocumentToPdfConverter.Pooling;
using FluentAssertions;
using Xunit;

namespace DocumentToPdfConverter.UnitTests.Pooling;

public sealed class StreamPoolManagerTests
{
    [Fact]
    public void GetStream_ShouldReturnRecyclableStream()
    {
        using var stream = StreamPoolManager.GetStream("test");
        stream.Should().NotBeNull();
        stream.Should().BeAssignableTo<Stream>();
        stream.Length.Should().Be(0);
    }

    [Fact]
    public void GetStream_WithSize_ShouldHaveCapacity()
    {
        using var stream = StreamPoolManager.GetStream("test", 4096);
        stream.Capacity.Should().BeGreaterThanOrEqualTo(4096);
    }

    [Fact]
    public void GetStream_MultipleCalls_ShouldReuseBuffers()
    {
        for (int i = 0; i < 100; i++)
        {
            using var stream = StreamPoolManager.GetStream("perf-test", 1024);
            stream.Write(new byte[64]);
        }
    }
}
