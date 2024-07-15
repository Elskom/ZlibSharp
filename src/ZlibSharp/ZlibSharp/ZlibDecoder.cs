// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

using Internal;
using Exceptions;

/// <summary>
/// Zlib Memory Decompression class.
/// </summary>
/// <param name="WindowBits"> Gets or sets the window bits to use to decompress the data. </param>
public record ZlibDecoder(ZlibWindowBits WindowBits)
{
    public ZlibDecoder() : this(ZlibWindowBits.Zlib)
    {
    }

    /// <summary>
    /// Decompresses a file.
    /// </summary>
    /// <param name="sourcePath">The path to the file to decompress.</param>
    /// <param name="dest">The decompressed data buffer.</param>
    /// <param name="result">
    /// The zlib result structure that contains the amount of bytes read, written,
    /// and the adler32 hash of the data that can be used to compare the integrity
    /// of the compressed/decompressed results.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the compression was a success, <see langword="false"/> otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryDecompress(string sourcePath, Span<byte> dest, out ZlibResult? result)
        => this.TryDecompress(File.ReadAllBytes(sourcePath), dest, out result);

    /// <summary>
    /// Decompresses data.
    /// </summary>
    /// <param name="source">The compressed input data.</param>
    /// <param name="dest">The decompressed data buffer.</param>
    /// <param name="result">
    /// The zlib result structure that contains the amount of bytes read, written,
    /// and the adler32 hash of the data that can be used to compare the integrity
    /// of the compressed/decompressed results.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the compression was a success, <see langword="false"/> otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryDecompress(ReadOnlySpan<byte> source, Span<byte> dest, out ZlibResult? result)
    {
        try
        {
            result = this.Decompress(source, dest);
            return true;
        }
        catch (NotUnpackableException)
        {
            result = null;
            return false;
        }
    }

    /// <summary>
    /// Decompresses a file.
    /// </summary>
    /// <param name="sourcePath">The path to the file to decompress.</param>
    /// <param name="dest">The decompressed data buffer.</param>
    /// <exception cref="NotUnpackableException">
    /// Thrown when zlib errors internally in any way.
    /// </exception>
    /// <returns>
    /// The zlib result structure that contains the amount of bytes read, written,
    /// and the adler32 hash of the data that can be used to compare the integrity
    /// of the compressed/decompressed results.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ZlibResult Decompress(string sourcePath, Span<byte> dest)
        => this.Decompress(File.ReadAllBytes(sourcePath), dest);

    /// <summary>
    /// Decompresses data.
    /// </summary>
    /// <param name="source">The compressed input data.</param>
    /// <param name="dest">The decompressed data buffer.</param>
    /// <exception cref="NotUnpackableException">
    /// Thrown when zlib errors internally in any way.
    /// </exception>
    /// <returns>
    /// The zlib result structure that contains the amount of bytes read, written,
    /// and the adler32 hash of the data that can be used to compare the integrity
    /// of the compressed/decompressed results.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ZlibResult Decompress(ReadOnlySpan<byte> source, Span<byte> dest)
    {
        var bytesRead = ZlibHelper.Decompress(source, dest, out var bytesWritten, out var adler32, out var crc32, out var status, this.WindowBits);
        return new ZlibResult(bytesWritten, bytesRead, adler32, crc32, status);
    }
}
