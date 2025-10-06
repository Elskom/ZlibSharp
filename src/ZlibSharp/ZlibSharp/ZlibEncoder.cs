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
public class ZlibEncoder
{
    private static readonly Lazy<ZlibEncoder> _default = new(() => new ZlibEncoder());

    internal ZlibEncoder()
        => this.Options = new ZlibOptions();

    /// <summary>
    /// Gets the default instance of the <see cref="ZlibEncoder" /> class.
    /// </summary>
    public static ZlibEncoder Default
        => _default.Value;

    /// <summary>
    /// Gets or sets the options to use to compress the file.
    /// </summary>
    public ZlibOptions Options { get; internal set; }

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
    public bool TryCompress(string sourcePath, Span<byte> dest, out ZlibResult result)
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
    public bool TryCompress(ReadOnlySpan<byte> source, Span<byte> dest, out ZlibResult result)
        => this.TryCompressCore(source, dest, out result);

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
        var bytesWritten = ZlibHelper.Compress(source, dest, this.Options.CompressionLevel, this.Options.WindowBits, this.Options.Strategy, out var hash, out var status);
        return new(bytesWritten, 0, hash, status);
    }

    /// <summary>
    /// Computes an Adler32 Hash if the Window Bits in <see cref="Options" /> is
    /// <see cref="ZlibWindowBits.Deflate" /> or <see cref="ZlibWindowBits.Zlib" />,
    /// a Crc32 Hash otherwise.
    /// </summary>
    /// <param name="source">The input data to hash.</param>
    /// <returns>The computed Adler32 or Crc32 Hash.</returns>
    public uint ComputeHash(ReadOnlySpan<byte> source)
        => !this.Options.WindowBits.Equals(ZlibWindowBits.GZip)
            ? (uint)(ZlibHelper.GetAdler32(source) & 0xFFFFFFFF)
            : (uint)(ZlibHelper.GetCrc32(source) & 0xFFFFFFFF);

    [ExcludeFromCodeCoverage]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryCompressCore(ReadOnlySpan<byte> source, Span<byte> dest, out ZlibResult result)
    {
        try
        {
            result = this.Compress(source, dest);
            return true;
        }
        catch (NotPackableException)
        {
            result = default;
            return false;
        }
    }
}
