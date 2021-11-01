// Copyright (c) 2021, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Tests;

public class Test
{
    private readonly byte[] sourceString, sourceStringCompressed, sourceBuffer;
    private readonly int lengthOfCompressed;

    public Test()
    {
        sourceString = File.ReadAllBytes("SourceText.txt");
        var destBuffer = new byte[sourceString.Length];
        lengthOfCompressed = (int)MemoryZlib.Compress(sourceString, destBuffer);
        sourceStringCompressed = new byte[lengthOfCompressed];
        destBuffer.AsSpan(0, lengthOfCompressed).CopyTo(sourceStringCompressed);
        sourceBuffer = new byte[sourceString.Length];
    }

    [Fact]
    public void DecompressionWorks()
    {
        MemoryZlib.Decompress(sourceStringCompressed, sourceBuffer, out _).Should().Be(0);
        sourceBuffer.Should().Equal(sourceString);
    }

    [Fact]
    public void DecompressionToUnderAllocatedBufferReturnsNonZeroValue()
    {
        const int undersizedBufferLength = 69;
        undersizedBufferLength.Should().BeLessThan(sourceBuffer.Length);
        var undersizedDestBuffer = new byte[undersizedBufferLength];
        MemoryZlib.Decompress(sourceStringCompressed, undersizedDestBuffer, out var bytesWritten).Should().NotBe(0);
        bytesWritten.Should().Be(undersizedBufferLength);
    }

    [Fact]
    public void CompressionToUnderAllocatedBufferReturnsNonZeroValue()
    {
        const int undersizedBufferLength = 69;
        undersizedBufferLength.Should().BeLessThan(lengthOfCompressed);
        var undersizedDestBuffer = new byte[undersizedBufferLength];
        MemoryZlib.Compress(sourceStringCompressed, undersizedDestBuffer).Should().NotBe(0);
    }

    [Fact]
    public void DecompressionToOverAllocatedBufferShouldHaveBytesWrittenEqualToSourceStringLength()
    {
        const uint oversizeBy = 69;
        var sourceLength = (uint)sourceString.Length;
        var oversizedDestBuffer = new byte[sourceLength + oversizeBy];
        MemoryZlib.Decompress(sourceStringCompressed, oversizedDestBuffer, out var bytesWritten).Should().Be(0);
        bytesWritten.Should().Be(sourceLength);
    }
}
