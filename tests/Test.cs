using FluentAssertions;
using Xunit;

namespace ZlibSharp.Tests;

public class Test
{
    private readonly byte[] SourceString, SourceStringCompressed, SourceBuffer;

    public Test()
    {
        SourceString = File.ReadAllBytes("SourceText.txt");

        var DestBuffer = new byte[SourceString.Length];
        
        var CompressedLength = (int) MemoryZlib.Compress(SourceString, DestBuffer);

        SourceStringCompressed = new byte[CompressedLength];
        
        DestBuffer.AsSpan(0, CompressedLength).CopyTo(SourceStringCompressed);

        SourceBuffer = new byte[SourceString.Length];
    }
    
    [Fact]
    public void DecompressionWorks()
    {
        MemoryZlib.Decompress(SourceStringCompressed, SourceBuffer).Should().Be(0);

        SourceBuffer.Should().Equal(SourceString);
    }
}