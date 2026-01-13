// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

using Exceptions;
using Internal;
using System.Buffers;

/// <summary>
/// Zlib Memory Decompression class.
/// </summary>
public class ZlibDecoder
{
    private static readonly Lazy<ZlibDecoder> _default = new(() => new ZlibDecoder());

    internal ZlibDecoder()
        => this.Options = new ZlibOptions();

    /// <summary>
    /// Gets the default instance of the <see cref="ZlibDecoder" /> class.
    /// </summary>
    public static ZlibDecoder Default
        => _default.Value;

    /// <summary>
    /// Gets or sets the options to use to decompress the data.
    /// </summary>
    public ZlibOptions Options { get; internal set; }

    /// <summary>
    /// Decompresses a file.
    /// </summary>
    /// <param name="sourcePath">The path to the file to decompress.</param>
    /// <param name="dest">The decompressed data buffer.</param>
    /// <param name="bytesWritten">The amount of bytes written to the destination buffer.</param>
    /// <param name="bytesRead">The amount of data available from the source buffer, allowing users to reallocate
    /// and continue decompressing the remaining data.</param>
    /// <param name="hash">The Adler32 checksum of the decompressed data if
    /// it was decompressed with <see cref="ZlibWindowBits.Deflate" />
    /// or <see cref="ZlibWindowBits.Zlib" />, the Crc32 checksum otherwise.</param>
    /// <param name="status">The resulting status code from zlib.</param>
    /// <returns>
    /// <see langword="true"/> if the compression was a success, <see langword="false"/> otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryDecompress(string sourcePath, Span<byte> dest, out uint bytesWritten, out uint bytesRead, out uint hash, out OperationStatus status)
        => this.TryDecompress(File.ReadAllBytes(sourcePath), dest, out bytesWritten, out bytesRead, out hash, out status);

    /// <summary>
    /// Decompresses data.
    /// </summary>
    /// <param name="source">The compressed input data.</param>
    /// <param name="dest">The decompressed data buffer.</param>
    /// <param name="bytesWritten">The amount of bytes written to the destination buffer.</param>
    /// <param name="bytesRead">The amount of data available from the source buffer, allowing users to reallocate
    /// and continue decompressing the remaining data.</param>
    /// <param name="hash">The Adler32 checksum of the decompressed data if
    /// it was decompressed with <see cref="ZlibWindowBits.Deflate" />
    /// or <see cref="ZlibWindowBits.Zlib" />, the Crc32 checksum otherwise.</param>
    /// <param name="status">The resulting status code from zlib.</param>
    /// <returns>
    /// <see langword="true"/> if the compression was a success, <see langword="false"/> otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryDecompress(ReadOnlySpan<byte> source, Span<byte> dest, out uint bytesWritten, out uint bytesRead, out uint hash, out OperationStatus status)
        => this.TryDecompressCore(source, dest, out bytesWritten, out bytesRead, out hash, out status);

    /// <summary>
    /// Decompresses a file.
    /// </summary>
    /// <param name="sourcePath">The path to the file to decompress.</param>
    /// <param name="dest">The decompressed data buffer.</param>
    /// <param name="bytesWritten">The amount of bytes written to the destination buffer.</param>
    /// <param name="bytesRead">The amount of data available from the source buffer, allowing users to reallocate
    /// and continue decompressing the remaining data.</param>
    /// <param name="hash">The Adler32 checksum of the decompressed data if
    /// it was decompressed with <see cref="ZlibWindowBits.Deflate" />
    /// or <see cref="ZlibWindowBits.Zlib" />, the Crc32 checksum otherwise.</param>
    /// <param name="status">The resulting status code from zlib.</param>
    /// <exception cref="NotUnpackableException">
    /// Thrown when zlib errors internally in any way.
    /// </exception>
    /// <returns>
    /// The zlib result structure that contains the amount of bytes read, written,
    /// and the adler32 hash of the data that can be used to compare the integrity
    /// of the compressed/decompressed results.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Decompress(string sourcePath, Span<byte> dest, out uint bytesWritten, out uint bytesRead, out uint hash, out OperationStatus status)
        => this.Decompress(File.ReadAllBytes(sourcePath), dest, out bytesWritten, out bytesRead, out hash, out status);

    /// <summary>
    /// Decompresses data.
    /// </summary>
    /// <param name="source">The compressed input data.</param>
    /// <param name="dest">The decompressed data buffer.</param>
    /// <param name="bytesWritten">The amount of bytes written to the destination buffer.</param>
    /// <param name="bytesRead">The amount of data available from the source buffer, allowing users to reallocate
    /// and continue decompressing the remaining data.</param>
    /// <param name="hash">The Adler32 checksum of the decompressed data if
    /// it was decompressed with <see cref="ZlibWindowBits.Deflate" />
    /// or <see cref="ZlibWindowBits.Zlib" />, the Crc32 checksum otherwise.</param>
    /// <param name="status">The resulting status code from zlib.</param>
    /// <exception cref="NotUnpackableException">
    /// Thrown when zlib errors internally in any way.
    /// </exception>
    /// <returns>
    /// The zlib result structure that contains the amount of bytes read, written,
    /// and the adler32 hash of the data that can be used to compare the integrity
    /// of the compressed/decompressed results.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Decompress(ReadOnlySpan<byte> source, Span<byte> dest, out uint bytesWritten, out uint bytesRead, out uint hash, out OperationStatus status)
        => this.DecompressCore(source, dest, out bytesWritten, out bytesRead, out hash, out status);

    [ExcludeFromCodeCoverage]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DecompressCore(ReadOnlySpan<byte> source, Span<byte> dest, out uint bytesWritten, out uint bytesRead, out uint hash, out OperationStatus status)
    {
        bytesRead = ZlibHelper.Decompress(source, dest, out bytesWritten, out var zstatus, this.Options.WindowBits);
        hash = this.Options.HashAlgorithm.ComputeHash(dest);
        status = zstatus.ToOperationStatus();
    }

    [ExcludeFromCodeCoverage]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryDecompressCore(ReadOnlySpan<byte> source, Span<byte> dest, out uint bytesWritten, out uint bytesRead, out uint hash, out OperationStatus status)
    {
        try
        {
            this.DecompressCore(source, dest, out bytesWritten, out bytesRead, out hash, out status);
            return true;
        }
        catch (NotUnpackableException)
        {
            bytesWritten = default;
            bytesRead = default;
            hash = default;
            status = default;
            return false;
        }
    }
}
