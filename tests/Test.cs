// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Tests;

[ExcludeFromCodeCoverage]
public class Test
{
    private readonly byte[] sourceString, sourceStringCompressed, sourceBuffer;
    private readonly int lengthOfCompressed;

    public Test()
    {
        sourceString = File.ReadAllBytes("SourceText.txt");
        var destBuffer = new byte[sourceString.Length];
        lengthOfCompressed = (int)MemoryZlib.Compress(sourceString, destBuffer).BytesWritten;
        sourceStringCompressed = new byte[lengthOfCompressed];
        destBuffer.AsSpan(0, lengthOfCompressed).CopyTo(sourceStringCompressed);
        sourceBuffer = new byte[sourceString.Length];
    }

    [Fact]
    public void CompressFileWorks()
    {
        var destBuffer = new byte[sourceString.Length];
        var result = MemoryZlib.Compress("SourceText.txt", destBuffer.AsSpan(), ZlibCompressionLevel.Level7);
        _ = result.BytesWritten.Should().BeGreaterThan(0);
        _ = result.Adler32.Should().BeGreaterThan(0);
    }

    [Fact]
    public void DecompressionWorks()
    {
        var result  = MemoryZlib.Decompress(sourceStringCompressed, sourceBuffer);
        _ = result.BytesRead.Should().Be(0);
        _ = result.Adler32.Should().BeGreaterThan(0);
        _ = sourceBuffer.Should().Equal(sourceString);
    }

    [Fact]
    public void DecompressionToUnderAllocatedBufferReturnsNonZeroValue()
    {
        const int undersizedBufferLength = 69;
        _ = undersizedBufferLength.Should().BeLessThan(sourceBuffer.Length);
        var undersizedDestBuffer = new byte[undersizedBufferLength];
        var result = MemoryZlib.Decompress(sourceStringCompressed, undersizedDestBuffer);
        _ = result.BytesRead.Should().NotBe(0);
        _ = result.BytesWritten.Should().Be(undersizedBufferLength);
    }

    [Fact]
    public void CompressionToUnderAllocatedBufferReturnsNonZeroValue()
    {
        const int undersizedBufferLength = 69;
        _ = undersizedBufferLength.Should().BeLessThan(lengthOfCompressed);
        var undersizedDestBuffer = new byte[undersizedBufferLength];
        _ = MemoryZlib.Compress(sourceStringCompressed, undersizedDestBuffer).BytesWritten.Should().NotBe(0);
    }

    [Fact]
    public void DecompressionToOverAllocatedBufferShouldHaveBytesWrittenEqualToSourceStringLength()
    {
        const uint oversizeBy = 69;
        var sourceLength = (uint)sourceString.Length;
        var oversizedDestBuffer = new byte[sourceLength + oversizeBy];
        var result = MemoryZlib.Decompress(sourceStringCompressed, oversizedDestBuffer);
        _ = result.BytesRead.Should().Be(0);
        _ = result.BytesWritten.Should().Be(sourceLength);
    }

    [Fact]
    public void ZlibSharpVersionWorks()
        => _ = MemoryZlib.ZlibSharpVersion().Should().Be("1.2.13");

    [Fact]
    public void ZlibVersionWorks()
        => _ = MemoryZlib.ZlibVersion().Should().Be(MemoryZlib.NativeZlibVersion);

    [Fact]
    public void IsCompressedByZlibWorks()
        => _ = MemoryZlib.IsCompressedByZlib("SourceText.txt").Should().BeFalse();

    [Fact]
    public void IsCompressedByZlibFailure()
        => _ = Assert.Throws<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => _ = MemoryZlib.IsCompressedByZlib(Array.Empty<byte>()));

    [Fact]
    public void NativeZlibVersionWorks()
    {
        var testZlibVersion = "1.2.11";
        var originalVersion = MemoryZlib.NativeZlibVersion;
        MemoryZlib.NativeZlibVersion = testZlibVersion;
        _ = MemoryZlib.NativeZlibVersion.Should().Be(testZlibVersion);
        MemoryZlib.NativeZlibVersion = originalVersion;
    }

    [Fact]
    public void GetAdler32Works()
        => _ = MemoryZlib.ZlibGetAdler32(File.ReadAllBytes("SourceText.txt")).Should().Be(2150767711UL);
}
