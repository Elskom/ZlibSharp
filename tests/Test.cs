// Copyright (c) 2021~2026, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Tests;

using System.Buffers;
using System.IO.Hashing;
using ZlibSharp.Exceptions;

[ExcludeFromCodeCoverage]
public class Test
{
    private readonly byte[] sourceString;
    private readonly CompressionSpan<byte> sourceStringCompressed, gzSourceStringCompressed;

    public Test()
    {
        sourceString = File.ReadAllBytes("SourceText.txt");
        ZlibEncoder.Default.Options.CompressionLevel = ZlibCompressionLevel.Level7;
        _ = ZlibEncoder.Default.Compress(sourceString, out sourceStringCompressed, out _);
        ZlibEncoder.Default.Options.Reset();
        ZlibEncoder.Default.Options.WindowBits = ZlibWindowBits.GZip;
        _ = ZlibEncoder.Default.Compress(sourceString, out gzSourceStringCompressed, out _);
        ZlibEncoder.Default.Options.Reset();
    }

    [Fact]
    public void CompressFileWorks()
    {
        ZlibEncoder.Default.Options.CompressionLevel = ZlibCompressionLevel.Level7;
        var status = ZlibEncoder.Default.Compress("SourceText.txt", out var destBuffer, out var bytesWritten);
        _ = bytesWritten.Should().BeGreaterThan(0);
        _ = status.Should().Be(OperationStatus.Done);
        _ = destBuffer.Length.Should().Be(sourceStringCompressed.Length);
        _ = destBuffer.AsSpan().Should().Equal(sourceStringCompressed.AsSpan());
        _ = destBuffer.AsReadOnlySpan().Should().Equal(sourceStringCompressed.AsReadOnlySpan());
        _ = destBuffer.AsMemory().Should().Equal(sourceStringCompressed.AsMemory());
        _ = destBuffer.AsReadOnlyMemory().Should().Equal(sourceStringCompressed.AsReadOnlyMemory());
        _ = destBuffer.AsArray().Should().Equal(sourceStringCompressed.AsArray());

        // overwrite destBuffer to test TryCompress.
        ZlibEncoder.Default.TryCompress("SourceText.txt", out destBuffer, out _, out _).Should().BeTrue();
        _ = destBuffer.Length.Should().Be(sourceStringCompressed.Length);
        _ = destBuffer.AsSpan().Should().Equal(sourceStringCompressed.AsSpan());
        _ = destBuffer.AsReadOnlySpan().Should().Equal(sourceStringCompressed.AsReadOnlySpan());
        _ = destBuffer.AsMemory().Should().Equal(sourceStringCompressed.AsMemory());
        _ = destBuffer.AsReadOnlyMemory().Should().Equal(sourceStringCompressed.AsReadOnlyMemory());
        _ = destBuffer.AsArray().Should().Equal(sourceStringCompressed.AsArray());
    }

    [Fact]
    public void DecompressFileWorks()
    {
        var status = ZlibDecoder.Default.Decompress("CompressedText.txt", out var destBuffer, out _, out var bytesRead);
        _ = bytesRead.Should().Be(0);
        _ = status.Should().Be(OperationStatus.Done);
        _ = destBuffer.Length.Should().Be(Convert.ToUInt64(sourceString.Length));
        _ = destBuffer.AsSpan().Should().Equal(sourceString);
        _ = destBuffer.AsReadOnlySpan().Should().Equal(sourceString);
        _ = destBuffer.AsMemory().Should().Equal(sourceString);
        _ = destBuffer.AsReadOnlyMemory().Should().Equal(sourceString);
        _ = destBuffer.AsArray().Should().Equal(sourceString);

        // overwrite destBuffer to test TryDecompress.
        ZlibDecoder.Default.TryDecompress("CompressedText.txt", out destBuffer, out _, out _, out _).Should().BeTrue();
        _ = destBuffer.Length.Should().Be(Convert.ToUInt64(sourceString.Length));
        _ = destBuffer.AsSpan().Should().Equal(sourceString);
        _ = destBuffer.AsReadOnlySpan().Should().Equal(sourceString);
        _ = destBuffer.AsMemory().Should().Equal(sourceString);
        _ = destBuffer.AsReadOnlyMemory().Should().Equal(sourceString);
        _ = destBuffer.AsArray().Should().Equal(sourceString);
    }

    [Fact]
    public void DecompressionWorks()
    {
        var status = ZlibDecoder.Default.Decompress(sourceStringCompressed.AsReadOnlySpan(), out var sourceBuffer, out _, out var bytesRead);
        _ = bytesRead.Should().Be(0);
        _ = status.Should().Be(OperationStatus.Done);
        _ = sourceBuffer.Length.Should().Be(Convert.ToUInt64(sourceString.Length));
        _ = sourceBuffer.AsSpan().Should().Equal(sourceString);
        _ = sourceBuffer.AsReadOnlySpan().Should().Equal(sourceString);
        _ = sourceBuffer.AsMemory().Should().Equal(sourceString);
        _ = sourceBuffer.AsReadOnlyMemory().Should().Equal(sourceString);
        _ = sourceBuffer.AsArray().Should().Equal(sourceString);

        // Test TryDecompress as well to ensure it returns true here.
        ZlibDecoder.Default.TryDecompress(sourceStringCompressed.AsReadOnlySpan(), out sourceBuffer, out _, out _, out _).Should().BeTrue();
        _ = sourceBuffer.Length.Should().Be(Convert.ToUInt64(sourceString.Length));
        _ = sourceBuffer.AsSpan().Should().Equal(sourceString);
        _ = sourceBuffer.AsReadOnlySpan().Should().Equal(sourceString);
        _ = sourceBuffer.AsMemory().Should().Equal(sourceString);
        _ = sourceBuffer.AsReadOnlyMemory().Should().Equal(sourceString);
        _ = sourceBuffer.AsArray().Should().Equal(sourceString);
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
        ZlibEncoder.Default.Options.WindowBits = ZlibWindowBits.GZip;
        var status = ZlibEncoder.Default.Compress(sourceString, out var destBuffer, out _);
        _ = status.Should().Be(OperationStatus.Done);
        _ = destBuffer.Length.Should().Be(gzSourceStringCompressed.Length);
        _ = destBuffer.AsSpan().Should().Equal(gzSourceStringCompressed.AsSpan());
        _ = destBuffer.AsReadOnlySpan().Should().Equal(gzSourceStringCompressed.AsReadOnlySpan());
        _ = destBuffer.AsMemory().Should().Equal(gzSourceStringCompressed.AsMemory());
        _ = destBuffer.AsReadOnlyMemory().Should().Equal(gzSourceStringCompressed.AsReadOnlyMemory());
        _ = destBuffer.AsArray().Should().Equal(gzSourceStringCompressed.AsArray());
        _ = ZlibDecoder.IsCompressedByGZip(destBuffer.AsReadOnlySpan()).Should().BeTrue();
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
