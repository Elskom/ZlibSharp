// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

using Internal;
using Exceptions;

/// <summary>
/// Zlib Memory Compression class.
/// </summary>
public record ZlibEncoder
{
    public ZlibEncoder() : this(ZlibCompressionLevel.DefaultCompression, ZlibWindowBits.Zlib, ZlibCompressionStrategy.Default)
    {
    }

    public ZlibEncoder(ZlibCompressionLevel compressionLevel, ZlibWindowBits windowBits, ZlibCompressionStrategy strategy)
    {
        this.CompressionLevel = compressionLevel;
        this.WindowBits = windowBits;
        this.Strategy = strategy;
    }

    /// <summary>
    /// Gets or sets the compression level to use to compress the file.
    /// </summary>
    public ZlibCompressionLevel CompressionLevel { get; set; }

    /// <summary>
    /// Gets or sets the window bits to use to compress the data.
    /// </summary>
    public ZlibWindowBits WindowBits { get; set; }

    /// <summary>
    /// Gets or sets the compression strategy to use to compress the data.
    /// </summary>
    public ZlibCompressionStrategy Strategy { get; set; }

    /// <summary>
    /// Tries to compress a file using the user specified compression level.
    /// </summary>
    /// <param name="sourcePath">The path to the file to compress.</param>
    /// <param name="dest">The compressed data buffer.</param>
    /// <param name="result">
    /// The zlib result structure that contains the amount of bytes read, written,
    /// and the adler32 hash of the data that can be used to compare the integrity
    /// of the compressed/decompressed results.
    ///
    /// If the compression failed this is set to <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the compression was a success, <see langword="false"/> otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCompress(string sourcePath, Span<byte> dest, out ZlibResult? result)
        => this.TryCompress(File.ReadAllBytes(sourcePath), dest, out result);

    /// <summary>
    /// Tries to compress the data using the user specified compression level.
    /// </summary>
    /// <param name="source">The input data buffer.</param>
    /// <param name="dest">The compressed data buffer.</param>
    /// <param name="result">
    /// The zlib result structure that contains the amount of bytes read, written,
    /// and the adler32 hash of the data that can be used to compare the integrity
    /// of the compressed/decompressed results.
    ///
    /// If the compression failed this is set to <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the compression was a success, <see langword="false"/> otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCompress(ReadOnlySpan<byte> source, Span<byte> dest, out ZlibResult? result)
    {
        try
        {
            result = this.Compress(source, dest);
            return true;
        }
        catch (NotPackableException)
        {
            result = null;
            return false;
        }
    }

    /// <summary>
    /// Compresses a file using the user specified compression level.
    /// </summary>
    /// <param name="sourcePath">The path to the file to compress.</param>
    /// <param name="dest">The compressed data buffer.</param>
    /// <exception cref="NotPackableException">
    /// Thrown when zlib errors internally in any way.
    /// </exception>
    /// <returns>
    /// The zlib result structure that contains the amount of bytes read, written,
    /// and the adler32 hash of the data that can be used to compare the integrity
    /// of the compressed/decompressed results.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ZlibResult Compress(string sourcePath, Span<byte> dest)
        => this.Compress(File.ReadAllBytes(sourcePath), dest);

    /// <summary>
    /// Compresses data using the user specified compression level.
    /// </summary>
    /// <param name="source">The input data buffer.</param>
    /// <param name="dest">The compressed data buffer.</param>
    /// <exception cref="NotPackableException">
    /// Thrown when zlib errors internally in any way.
    /// </exception>
    /// <returns>
    /// The zlib result structure that contains the amount of bytes read, written,
    /// and the adler32 hash of the data that can be used to compare the integrity
    /// of the compressed/decompressed results.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ZlibResult Compress(ReadOnlySpan<byte> source, Span<byte> dest)
    {
        var bytesWritten = ZlibHelper.Compress(source, dest, this.CompressionLevel, this.WindowBits, this.Strategy, out var adler32, out var crc32, out var status);
        return new(bytesWritten, 0, adler32, crc32, status);
    }
}
