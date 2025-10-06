// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Tests;

using System.Buffers;
using ZlibSharp.Exceptions;
using ZlibSharp.Extensions;

[ExcludeFromCodeCoverage]
public class Test
{
    private readonly byte[] sourceString, sourceStringCompressed, sourceBuffer;
    private readonly int lengthOfCompressed;

    public Test()
    {
        sourceString = File.ReadAllBytes("SourceText.txt");
        // var destBuffer = new byte[sourceString.Length];
        lengthOfCompressed = (int)ZlibEncoder.Default.GetCompressedSize(sourceString);
        sourceStringCompressed = new byte[lengthOfCompressed];
        _ = ZlibEncoder.Default.Compress(sourceString, sourceStringCompressed);
        // destBuffer.AsSpan(0, lengthOfCompressed).CopyTo(sourceStringCompressed);
        sourceBuffer = new byte[sourceString.Length];
    }

    [Fact]
    public void CompressFileWorks()
    {
        var destBuffer = new byte[sourceString.Length];
        ZlibEncoder.Default.Options.CompressionLevel = ZlibCompressionLevel.Level7;
        var result = ZlibEncoder.Default.Compress("SourceText.txt", destBuffer);
        _ = result.BytesWritten.Should().BeGreaterThan(0);
        _ = result.Hash.Should().BeGreaterThan(0);
        _ = result.Status.Should().Be(OperationStatus.Done);

        // overwrite destBuffer to test TryCompress.
        destBuffer = new byte[sourceString.Length];
        ZlibEncoder.Default.TryCompress("SourceText.txt", destBuffer, out _).Should().BeTrue();
    }

    [Fact]
    public void DecompressFileWorks()
    {
        var destBuffer = new byte[sourceString.Length];
        var result = ZlibDecoder.Default.Decompress("CompressedText.txt", destBuffer);
        _ = result.BytesRead.Should().BeGreaterThan(0);
        _ = result.Status.Should().Be(OperationStatus.Done);
        _ = destBuffer.Should().Equal(sourceString);

        // overwrite destBuffer to test TryDecompress.
        destBuffer = new byte[sourceString.Length];
        ZlibDecoder.Default.TryDecompress("CompressedText.txt", destBuffer, out _).Should().BeTrue();
    }

    [Fact]
    public void DecompressionWorks()
    {
        var result = ZlibDecoder.Default.Decompress(sourceStringCompressed, sourceBuffer);
        _ = result.BytesRead.Should().Be(0);
        _ = result.Hash.Should().BeGreaterThan(0);
        _ = result.Status.Should().Be(OperationStatus.Done);
        _ = sourceBuffer.Should().Equal(sourceString);

        // Test TryDecompress as well to ensure it returns true here.
        ZlibDecoder.Default.TryDecompress(sourceStringCompressed, sourceBuffer, out _).Should().BeTrue();
    }

    [Fact]
    public void DecompressionToUnderAllocatedFailure()
    {
        const int undersizedBufferLength = 69;
        _ = undersizedBufferLength.Should().BeLessThan((int)ZlibDecoder.Default.GetDecompressedSize(sourceStringCompressed));
        var undersizedDestBuffer = new byte[undersizedBufferLength];
        Assert.Throws<NotUnpackableException>(
            [ExcludeFromCodeCoverage] () => _ = ZlibDecoder.Default.Decompress(sourceStringCompressed, undersizedDestBuffer));

        // Test TryDecompress as well to ensure it returns false here.
        ZlibDecoder.Default.TryDecompress(sourceStringCompressed, undersizedDestBuffer, out _).Should().BeFalse();
    }

    [Fact]
    public void CompressionToUnderAllocatedBufferReturnsNonZeroValue()
    {
        const int undersizedBufferLength = 69;
        _ = undersizedBufferLength.Should().BeLessThan(lengthOfCompressed);
        var undersizedDestBuffer = new byte[undersizedBufferLength];
        _ = ZlibEncoder.Default.Compress(sourceStringCompressed, undersizedDestBuffer).BytesWritten.Should().NotBe(0);
        _ = ZlibEncoder.Default.TryCompress(sourceStringCompressed, undersizedDestBuffer, out _).Should().BeTrue();
    }

    [Fact]
    public void DecompressionToOverAllocatedBufferShouldHaveBytesWrittenEqualToSourceStringLength()
    {
        const uint oversizeBy = 69;
        var sourceLength = ZlibDecoder.Default.GetDecompressedSize(sourceStringCompressed);
        var oversizedDestBuffer = new byte[sourceLength + oversizeBy];
        var result = ZlibDecoder.Default.Decompress(sourceStringCompressed, oversizedDestBuffer);
        _ = result.BytesRead.Should().Be(0);
        _ = result.BytesWritten.Should().Be(sourceLength);
        _ = result.Status.Should().Be(OperationStatus.Done);
    }

    [Fact]
    public void IsCompressedByZlibWorksAndIsFalse()
        => _ = ZlibDecoder.Default.IsCompressedByZlib("SourceText.txt").Should().BeFalse();

    [Fact]
    public void IsCompressedByZlibWorksAndIsTrue()
        => _ = ZlibDecoder.Default.IsCompressedByZlib("CompressedText.txt").Should().BeTrue();

    [Fact]
    public void IsCompressedByZlibFailure()
        => _ = Assert.Throws<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => _ = ZlibDecoder.Default.IsCompressedByZlib([]));

    [Fact]
    public void IsCompressedByGZipWorksAndIsFalse()
        => _ = ZlibDecoder.Default.IsCompressedByGZip("SourceText.txt").Should().BeFalse();

    [Fact]
    public void IsCompressedByGZipWorksAndIsTrue()
    {
        var oldWindowBits = ZlibEncoder.Default.Options.WindowBits;
        var destBuffer = new byte[ZlibEncoder.Default.GetCompressedSize(sourceString)];
        ZlibEncoder.Default.Options.WindowBits = ZlibWindowBits.GZip;
        _ = ZlibEncoder.Default.Compress(sourceString, destBuffer);
        _ = ZlibDecoder.Default.IsCompressedByGZip(destBuffer).Should().BeTrue();
        ZlibEncoder.Default.Options.WindowBits = oldWindowBits;
    }

    [Fact]
    public void IsCompressedByGZipFailure()
        => _ = Assert.Throws<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => _ = ZlibDecoder.Default.IsCompressedByGZip([]));

    [Fact]
    public void GetAdler32Works()
    {
        _ = ZlibEncoder.Default.ComputeHash(File.ReadAllBytes("SourceText.txt")).Should().Be(2150767711U);
        _ = ZlibDecoder.Default.ComputeHash(File.ReadAllBytes("SourceText.txt")).Should().Be(2150767711U);
    }

    [Fact]
    public void GetCrc32Works()
    {
        ZlibEncoder.Default.Options.WindowBits = ZlibWindowBits.GZip;
        ZlibDecoder.Default.Options.WindowBits = ZlibWindowBits.GZip;
        _ = ZlibEncoder.Default.ComputeHash(File.ReadAllBytes("SourceText.txt")).Should().Be(739290345U);
        _ = ZlibDecoder.Default.ComputeHash(File.ReadAllBytes("SourceText.txt")).Should().Be(739290345U);
        ZlibDecoder.Default.Options.Reset();
        ZlibEncoder.Default.Options.Reset();
    }
}
