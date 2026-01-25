// Copyright (c) 2021~2026, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Tests;

using System.Buffers;
using System.IO.Hashing;
using ZlibSharp.Exceptions;
using ZlibSharp.IO.Hashing;

[ExcludeFromCodeCoverage]
public class Test
{
    private readonly byte[] sourceString, sourceStringCompressed, sourceBuffer;
    private readonly ulong lengthOfCompressed;

    public Test()
    {
        sourceString = File.ReadAllBytes("SourceText.txt");
        // var destBuffer = new byte[sourceString.Length];
        lengthOfCompressed = ZlibEncoder.Default.GetCompressedSize(sourceString);
        sourceStringCompressed = new byte[lengthOfCompressed];
        ZlibEncoder.Default.Options.CompressionLevel = ZlibCompressionLevel.Level7;
        _ = ZlibEncoder.Default.Compress(sourceString, sourceStringCompressed, out _);
        ZlibEncoder.Default.Options.Reset();
        // destBuffer.AsSpan(0, lengthOfCompressed).CopyTo(sourceStringCompressed);
        sourceBuffer = new byte[sourceString.Length];
    }

    [Fact]
    public void CompressFileWorks()
    {
        var destBuffer = new byte[lengthOfCompressed];
        ZlibEncoder.Default.Options.CompressionLevel = ZlibCompressionLevel.Level7;
        var status = ZlibEncoder.Default.Compress("SourceText.txt", destBuffer, out var bytesWritten);
        _ = bytesWritten.Should().BeGreaterThan(0);
        _ = status.Should().Be(OperationStatus.Done);
        _ = destBuffer.Should().Equal(sourceStringCompressed);

        // overwrite destBuffer to test TryCompress.
        destBuffer = new byte[lengthOfCompressed];
        ZlibEncoder.Default.TryCompress("SourceText.txt", destBuffer, out _, out _).Should().BeTrue();
    }

    [Fact]
    public void DecompressFileWorks()
    {
        var destBuffer = new byte[sourceString.Length];
        var status = ZlibDecoder.Default.Decompress("CompressedText.txt", destBuffer, out _, out var bytesRead);
        _ = bytesRead.Should().Be(0);
        _ = status.Should().Be(OperationStatus.Done);
        _ = destBuffer.Should().Equal(sourceString);

        // overwrite destBuffer to test TryDecompress.
        destBuffer = new byte[sourceString.Length];
        ZlibDecoder.Default.TryDecompress("CompressedText.txt", destBuffer, out _, out _, out _).Should().BeTrue();
    }

    [Fact]
    public void DecompressionWorks()
    {
        var status = ZlibDecoder.Default.Decompress(sourceStringCompressed, sourceBuffer, out _, out var bytesRead);
        _ = bytesRead.Should().Be(0);
        _ = status.Should().Be(OperationStatus.Done);
        _ = sourceBuffer.Should().Equal(sourceString);

        // Test TryDecompress as well to ensure it returns true here.
        ZlibDecoder.Default.TryDecompress(sourceStringCompressed, sourceBuffer, out _, out _, out _).Should().BeTrue();
    }

    [Fact]
    public void DecompressionToUnderAllocatedFailure()
    {
        const ulong undersizedBufferLength = 69ul;
        _ = undersizedBufferLength.Should().BeLessThan(ZlibDecoder.Default.GetDecompressedSize(sourceStringCompressed));
        var undersizedDestBuffer = new byte[undersizedBufferLength];
        Assert.Throws<NotUnpackableException>(
            [ExcludeFromCodeCoverage] () => {
                var status = ZlibDecoder.Default.Decompress(sourceStringCompressed, undersizedDestBuffer, out _, out _);
                if (status != OperationStatus.Done)
                {
                    throw new NotUnpackableException($"Decompression failed with status: {status}");
                }
            });

        // Test TryDecompress as well to ensure it returns false here.
        ZlibDecoder.Default.TryDecompress(sourceStringCompressed, undersizedDestBuffer, out _, out _, out _).Should().BeFalse();
    }

    [Fact]
    public void CompressionToUnderAllocatedBufferReturnsNonZeroValue()
    {
        const ulong undersizedBufferLength = 69ul;
        _ = undersizedBufferLength.Should().BeLessThan(lengthOfCompressed);
        var undersizedDestBuffer = new byte[undersizedBufferLength];
        _ = ZlibEncoder.Default.Compress(sourceString, undersizedDestBuffer, out var bytesWritten);
        _ = bytesWritten.Should().NotBe(0);
        _ = ZlibEncoder.Default.TryCompress(sourceString, undersizedDestBuffer, out _, out _).Should().BeFalse();
    }

    [Fact]
    public void DecompressionToOverAllocatedBufferShouldHaveBytesWrittenEqualToSourceStringLength()
    {
        const ulong oversizeBy = 69ul;
        var sourceLength = ZlibDecoder.Default.GetDecompressedSize(sourceStringCompressed);
        var oversizedDestBuffer = new byte[sourceLength + oversizeBy];
        var status = ZlibDecoder.Default.Decompress(sourceStringCompressed, oversizedDestBuffer, out var bytesWritten, out var bytesRead);
        _ = bytesRead.Should().Be(0);
        _ = bytesWritten.Should().Be(sourceLength);
        _ = status.Should().Be(OperationStatus.Done);
    }

    [Fact]
    public void IsCompressedByZlibWorksAndIsFalse()
        => _ = ZlibDecoder.IsCompressedByZlib("SourceText.txt").Should().BeFalse();

    [Fact]
    public void IsCompressedByZlibWorksAndIsTrue()
        => _ = ZlibDecoder.IsCompressedByZlib("CompressedText.txt").Should().BeTrue();

    [Fact]
    public void IsCompressedByZlibFailure()
        => _ = Assert.Throws<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => _ = ZlibDecoder.IsCompressedByZlib([]));

    [Fact]
    public void IsCompressedByGZipWorksAndIsFalse()
        => _ = ZlibDecoder.IsCompressedByGZip("SourceText.txt").Should().BeFalse();

    [Fact]
    public void IsCompressedByGZipWorksAndIsTrue()
    {
        var destBuffer = new byte[ZlibEncoder.Default.GetCompressedSize(sourceString)];
        ZlibEncoder.Default.Options.WindowBits = ZlibWindowBits.GZip;
        var status = ZlibEncoder.Default.Compress(sourceString, destBuffer, out _);
        _ = ZlibDecoder.IsCompressedByGZip(destBuffer).Should().BeTrue();
        ZlibEncoder.Default.Options.Reset();
    }

    [Fact]
    public void IsCompressedByGZipFailure()
        => _ = Assert.Throws<ArgumentNullException>(
            [ExcludeFromCodeCoverage] () => _ = ZlibDecoder.IsCompressedByGZip([]));

    [Fact]
    public void GetAdler32Works()
        => _ = Adler32.HashToUInt32(File.ReadAllBytes("SourceText.txt")).Should().Be(2150767711U);

    [Fact]
    public void GetCrc32Works()
        => _ = Crc32.HashToUInt32(File.ReadAllBytes("SourceText.txt")).Should().Be(739290345U);
}
