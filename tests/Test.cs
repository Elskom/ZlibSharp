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
        ZlibEncoder.Default.Compress(sourceString, sourceStringCompressed, out _, out _, out _);
        // destBuffer.AsSpan(0, lengthOfCompressed).CopyTo(sourceStringCompressed);
        sourceBuffer = new byte[sourceString.Length];
    }

    [Fact]
    public void CompressFileWorks()
    {
        var destBuffer = new byte[sourceString.Length];
        ZlibEncoder.Default.Options.CompressionLevel = ZlibCompressionLevel.Level7;
        ZlibEncoder.Default.Compress("SourceText.txt", destBuffer, out var bytesWritten, out var hash, out var status);
        _ = bytesWritten.Should().BeGreaterThan(0);
        _ = hash.Should().BeGreaterThan(0);
        _ = status.Should().Be(OperationStatus.Done);

        // overwrite destBuffer to test TryCompress.
        destBuffer = new byte[sourceString.Length];
        ZlibEncoder.Default.TryCompress("SourceText.txt", destBuffer, out _, out _, out _).Should().BeTrue();
    }

    [Fact]
    public void DecompressFileWorks()
    {
        var destBuffer = new byte[sourceString.Length];
        ZlibDecoder.Default.Decompress("CompressedText.txt", destBuffer, out _, out var bytesRead, out _, out var status);
        _ = bytesRead.Should().BeGreaterThan(0);
        _ = status.Should().Be(OperationStatus.Done);
        _ = destBuffer.Should().Equal(sourceString);

        // overwrite destBuffer to test TryDecompress.
        destBuffer = new byte[sourceString.Length];
        ZlibDecoder.Default.TryDecompress("CompressedText.txt", destBuffer, out _, out _, out _, out _).Should().BeTrue();
    }

    [Fact]
    public void DecompressionWorks()
    {
        ZlibDecoder.Default.Decompress(sourceStringCompressed, sourceBuffer, out _, out var bytesRead, out var hash, out var status);
        _ = bytesRead.Should().Be(0);
        _ = hash.Should().BeGreaterThan(0);
        _ = status.Should().Be(OperationStatus.Done);
        _ = sourceBuffer.Should().Equal(sourceString);

        // Test TryDecompress as well to ensure it returns true here.
        ZlibDecoder.Default.TryDecompress(sourceStringCompressed, sourceBuffer, out _, out _, out _, out _).Should().BeTrue();
    }

    [Fact]
    public void DecompressionToUnderAllocatedFailure()
    {
        const int undersizedBufferLength = 69;
        _ = undersizedBufferLength.Should().BeLessThan((int)ZlibDecoder.Default.GetDecompressedSize(sourceStringCompressed));
        var undersizedDestBuffer = new byte[undersizedBufferLength];
        Assert.Throws<NotUnpackableException>(
            [ExcludeFromCodeCoverage] () => ZlibDecoder.Default.Decompress(sourceStringCompressed, undersizedDestBuffer, out _, out _, out _, out _));

        // Test TryDecompress as well to ensure it returns false here.
        ZlibDecoder.Default.TryDecompress(sourceStringCompressed, undersizedDestBuffer, out _, out _, out _, out _).Should().BeFalse();
    }

    [Fact]
    public void CompressionToUnderAllocatedBufferReturnsNonZeroValue()
    {
        const int undersizedBufferLength = 69;
        _ = undersizedBufferLength.Should().BeLessThan(lengthOfCompressed);
        var undersizedDestBuffer = new byte[undersizedBufferLength];
        ZlibEncoder.Default.Compress(sourceStringCompressed, undersizedDestBuffer, out var bytesWritten, out _, out _);
        _ = bytesWritten.Should().NotBe(0);
        _ = ZlibEncoder.Default.TryCompress(sourceStringCompressed, undersizedDestBuffer, out _, out _, out _).Should().BeTrue();
    }

    [Fact]
    public void DecompressionToOverAllocatedBufferShouldHaveBytesWrittenEqualToSourceStringLength()
    {
        const uint oversizeBy = 69;
        var sourceLength = ZlibDecoder.Default.GetDecompressedSize(sourceStringCompressed);
        var oversizedDestBuffer = new byte[sourceLength + oversizeBy];
        ZlibDecoder.Default.Decompress(sourceStringCompressed, oversizedDestBuffer, out var bytesWritten, out var bytesRead, out _, out var status);
        _ = bytesRead.Should().Be(0);
        _ = bytesWritten.Should().Be(sourceLength);
        _ = status.Should().Be(OperationStatus.Done);
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
        ZlibEncoder.Default.Compress(sourceString, destBuffer, out _, out _, out _);
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
        _ = ZlibEncoder.Default.Options.HashAlgorithm.ComputeHash(File.ReadAllBytes("SourceText.txt")).Should().Be(2150767711U);
        _ = ZlibDecoder.Default.Options.HashAlgorithm.ComputeHash(File.ReadAllBytes("SourceText.txt")).Should().Be(2150767711U);
    }

    [Fact]
    public void GetCrc32Works()
    {
        ZlibEncoder.Default.Options.WindowBits = ZlibWindowBits.GZip;
        ZlibDecoder.Default.Options.WindowBits = ZlibWindowBits.GZip;
        _ = ZlibEncoder.Default.Options.HashAlgorithm.ComputeHash(File.ReadAllBytes("SourceText.txt")).Should().Be(739290345U);
        _ = ZlibDecoder.Default.Options.HashAlgorithm.ComputeHash(File.ReadAllBytes("SourceText.txt")).Should().Be(739290345U);
        ZlibDecoder.Default.Options.Reset();
        ZlibEncoder.Default.Options.Reset();
    }
}
