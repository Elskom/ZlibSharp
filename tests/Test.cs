// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Tests;

using ZlibSharp.Exceptions;

[ExcludeFromCodeCoverage]
public class Test
{
    private readonly byte[] sourceString, sourceStringCompressed, sourceBuffer;
    private readonly int lengthOfCompressed;

    public Test()
    {
        sourceString = File.ReadAllBytes("SourceText.txt");
        // var destBuffer = new byte[sourceString.Length];
        lengthOfCompressed = (int)MemoryZlib.GetCompressedSize(sourceString);
        sourceStringCompressed = new byte[lengthOfCompressed];
        _ = MemoryZlib.Compress(sourceString, sourceStringCompressed);
        // destBuffer.AsSpan(0, lengthOfCompressed).CopyTo(sourceStringCompressed);
        sourceBuffer = new byte[sourceString.Length];
    }

    [Fact]
    public void CompressFileWorks()
    {
        var destBuffer = new byte[sourceString.Length];
        var result = MemoryZlib.Compress("SourceText.txt", destBuffer, ZlibCompressionLevel.Level7);
        _ = result.BytesWritten.Should().BeGreaterThan(0);
        _ = result.Adler32.Should().BeGreaterThan(0);
    }

    [Fact]
    public void DecompressFileWorks()
    {
        var destBuffer = new byte[sourceString.Length];
        var result = MemoryZlib.Decompress("CompressedText.txt", destBuffer);
        _ = result.BytesRead.Should().BeGreaterThan(0);
        _ = destBuffer.Should().Equal(sourceString);
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
    public void DecompressionToUnderAllocatedFailure()
    {
        const int undersizedBufferLength = 69;
        _ = undersizedBufferLength.Should().BeLessThan((int)MemoryZlib.GetDecompressedSize(sourceStringCompressed));
        var undersizedDestBuffer = new byte[undersizedBufferLength];
        Assert.Throws<NotUnpackableException>(
            [ExcludeFromCodeCoverage] () => _ = MemoryZlib.Decompress(sourceStringCompressed, undersizedDestBuffer));
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
        var sourceLength = MemoryZlib.GetDecompressedSize(sourceStringCompressed);
        var oversizedDestBuffer = new byte[sourceLength + oversizeBy];
        var result = MemoryZlib.Decompress(sourceStringCompressed, oversizedDestBuffer);
        _ = result.BytesRead.Should().Be(0);
        _ = result.BytesWritten.Should().Be(sourceLength);
    }

    [Fact]
    public void ZlibSharpVersionWorks()
        => _ = MemoryZlib.ZlibSharpVersion().Should().Be("1.2.13.1");

    [Fact]
    public void ZlibVersionWorks()
        => _ = MemoryZlib.ZlibVersion().Should().Be(MemoryZlib.NativeZlibVersion);

    [Fact]
    public void IsCompressedByZlibWorksAndIsFalse()
        => _ = MemoryZlib.IsCompressedByZlib("SourceText.txt").Should().BeFalse();

    [Fact]
    public void IsCompressedByZlibWorksAndIsTrue()
    {
        var destBuffer = new byte[MemoryZlib.GetCompressedSize(sourceString)];
        _ = MemoryZlib.Compress(sourceString, destBuffer, windowBits: ZlibWindowBits.Zlib);
        _ = MemoryZlib.IsCompressedByZlib(destBuffer).Should().BeTrue();
    }

    [Fact]
    public void IsCompressedByZlibFailure()
        => _ = Assert.Throws<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => _ = MemoryZlib.IsCompressedByZlib(Array.Empty<byte>()));

    [Fact]
    public void IsCompressedByGZipWorksAndIsFalse()
        => _ = MemoryZlib.IsCompressedByGZip("SourceText.txt").Should().BeFalse();

    [Fact]
    public void IsCompressedByGZipWorksAndIsTrue()
    {
        var destBuffer = new byte[MemoryZlib.GetCompressedSize(sourceString)];
        _ = MemoryZlib.Compress(sourceString, destBuffer, windowBits: ZlibWindowBits.GZip);
        _ = MemoryZlib.IsCompressedByGZip(destBuffer).Should().BeTrue();
    }

    [Fact]
    public void IsCompressedByGZipFailure()
        => _ = Assert.Throws<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => _ = MemoryZlib.IsCompressedByGZip(Array.Empty<byte>()));

    [Fact]
    public void NativeZlibVersionWorks()
    {
        var testZlibVersion = "1.2.13";
        var originalVersion = MemoryZlib.NativeZlibVersion;
        MemoryZlib.NativeZlibVersion = testZlibVersion;
        _ = MemoryZlib.NativeZlibVersion.Should().Be(testZlibVersion);
        MemoryZlib.NativeZlibVersion = originalVersion;
    }

    [Fact]
    public void GetAdler32Works()
        => _ = MemoryZlib.ZlibGetAdler32(File.ReadAllBytes("SourceText.txt")).Should().Be(2150767711UL);

    [Fact]
    public void GetCrc32Works()
        => _ = MemoryZlib.ZlibGetCrc32(File.ReadAllBytes("SourceText.txt")).Should().Be(739290345UL);
}
