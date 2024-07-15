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
    private readonly ZlibEncoder encoder;
    private readonly ZlibDecoder decoder;

    public Test()
    {
        encoder = new ZlibEncoder();
        decoder = new ZlibDecoder();
        sourceString = File.ReadAllBytes("SourceText.txt");
        // var destBuffer = new byte[sourceString.Length];
        lengthOfCompressed = (int)Checks.GetCompressedSize(sourceString);
        sourceStringCompressed = new byte[lengthOfCompressed];
        _ = encoder.Compress(sourceString, sourceStringCompressed);
        // destBuffer.AsSpan(0, lengthOfCompressed).CopyTo(sourceStringCompressed);
        sourceBuffer = new byte[sourceString.Length];
    }

    [Fact]
    public void CompressFileWorks()
    {
        var destBuffer = new byte[sourceString.Length];
        encoder.CompressionLevel = ZlibCompressionLevel.Level7;
        var result = encoder.Compress("SourceText.txt", destBuffer);
        _ = result.BytesWritten.Should().BeGreaterThan(0);
        _ = result.Adler32.Should().BeGreaterThan(0);
        _ = result.Crc32.Should().BeGreaterThan(0);

        // overwrite destBuffer to test TryCompress.
        destBuffer = new byte[sourceString.Length];
        encoder.TryCompress("SourceText.txt", destBuffer, out _).Should().BeTrue();
    }

    [Fact]
    public void DecompressFileWorks()
    {
        var destBuffer = new byte[sourceString.Length];
        var result = decoder.Decompress("CompressedText.txt", destBuffer);
        _ = result.BytesRead.Should().BeGreaterThan(0);
        _ = destBuffer.Should().Equal(sourceString);

        // overwrite destBuffer to test TryDecompress.
        destBuffer = new byte[sourceString.Length];
        decoder.TryDecompress("CompressedText.txt", destBuffer, out _).Should().BeTrue();
    }

    [Fact]
    public void DecompressionWorks()
    {
        var result  = decoder.Decompress(sourceStringCompressed, sourceBuffer);
        _ = result.BytesRead.Should().Be(0);
        _ = result.Adler32.Should().BeGreaterThan(0);
        _ = result.Crc32.Should().BeGreaterThan(0);
        _ = sourceBuffer.Should().Equal(sourceString);

        // Test TryDecompress as well to ensure it returns true here.
        decoder.TryDecompress(sourceStringCompressed, sourceBuffer, out _).Should().BeTrue();
    }

    [Fact]
    public void DecompressionToUnderAllocatedFailure()
    {
        const int undersizedBufferLength = 69;
        _ = undersizedBufferLength.Should().BeLessThan((int)Checks.GetDecompressedSize(sourceStringCompressed));
        var undersizedDestBuffer = new byte[undersizedBufferLength];
        Assert.Throws<NotUnpackableException>(
            [ExcludeFromCodeCoverage] () => _ = decoder.Decompress(sourceStringCompressed, undersizedDestBuffer));

        // Test TryDecompress as well to ensure it returns false here.
        decoder.TryDecompress(sourceStringCompressed, undersizedDestBuffer, out _).Should().BeFalse();
    }

    [Fact]
    public void CompressionToUnderAllocatedBufferReturnsNonZeroValue()
    {
        const int undersizedBufferLength = 69;
        _ = undersizedBufferLength.Should().BeLessThan(lengthOfCompressed);
        var undersizedDestBuffer = new byte[undersizedBufferLength];
        _ = encoder.Compress(sourceStringCompressed, undersizedDestBuffer).BytesWritten.Should().NotBe(0);
        encoder.TryCompress(sourceStringCompressed, undersizedDestBuffer, out _).Should().BeTrue();
    }

    [Fact]
    public void DecompressionToOverAllocatedBufferShouldHaveBytesWrittenEqualToSourceStringLength()
    {
        const uint oversizeBy = 69;
        var sourceLength = Checks.GetDecompressedSize(sourceStringCompressed);
        var oversizedDestBuffer = new byte[sourceLength + oversizeBy];
        var result = decoder.Decompress(sourceStringCompressed, oversizedDestBuffer);
        _ = result.BytesRead.Should().Be(0);
        _ = result.BytesWritten.Should().Be(sourceLength);
    }

    [Fact]
    public void ZlibSharpVersionWorks()
        => _ = Checks.ZlibSharpVersion().Should().Be("1.2.13.4");

    [Fact]
    public void ZlibVersionWorks()
        => _ = Checks.ZlibVersion().Should().Be(Checks.NativeZlibVersion);

    [Fact]
    public void IsCompressedByZlibWorksAndIsFalse()
        => _ = Checks.IsCompressedByZlib("SourceText.txt").Should().BeFalse();

    [Fact]
    public void IsCompressedByZlibWorksAndIsTrue()
        => _ = Checks.IsCompressedByZlib("CompressedText.txt").Should().BeTrue();

    [Fact]
    public void IsCompressedByZlibFailure()
        => _ = Assert.Throws<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => _ = Checks.IsCompressedByZlib(Array.Empty<byte>()));

    [Fact]
    public void IsCompressedByGZipWorksAndIsFalse()
        => _ = Checks.IsCompressedByGZip("SourceText.txt").Should().BeFalse();

    [Fact]
    public void IsCompressedByGZipWorksAndIsTrue()
    {
        var destBuffer = new byte[Checks.GetCompressedSize(sourceString)];
        encoder.WindowBits = ZlibWindowBits.GZip;
        _ = encoder.Compress(sourceString, destBuffer);
        _ = Checks.IsCompressedByGZip(destBuffer).Should().BeTrue();
    }

    [Fact]
    public void IsCompressedByGZipFailure()
        => _ = Assert.Throws<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => _ = Checks.IsCompressedByGZip(Array.Empty<byte>()));

    [Fact]
    public void NativeZlibVersionWorks()
    {
        var testZlibVersion = "1.2.13";
        var originalVersion = Checks.NativeZlibVersion;
        Checks.NativeZlibVersion = testZlibVersion;
        _ = Checks.NativeZlibVersion.Should().Be(testZlibVersion);
        Checks.NativeZlibVersion = originalVersion;
    }

    [Fact]
    public void GetAdler32Works()
        => _ = Checks.ZlibGetAdler32(File.ReadAllBytes("SourceText.txt")).Should().Be(2150767711UL);

    [Fact]
    public void GetCrc32Works()
        => _ = Checks.ZlibGetCrc32(File.ReadAllBytes("SourceText.txt")).Should().Be(739290345UL);
}
