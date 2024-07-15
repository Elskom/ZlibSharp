// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

/// <summary>
/// The zlib result structure that contains the amount of bytes read, written,
/// and the adler32 hash of the data that can be used to compare the integrity
/// of the compressed/decompressed results.
/// </summary>
public readonly struct ZlibResult
{
    internal ZlibResult(uint bytesWritten, uint bytesRead, uint adler32, uint crc32, ZlibStatus status)
    {
        this.BytesWritten = bytesWritten;
        this.BytesRead = bytesRead;
        this.Adler32 = adler32;
        this.Crc32 = crc32;
        this.Status = status;
    }

    /// <summary>
    /// The amount of bytes written to the destination buffer.
    /// </summary>
    public uint BytesWritten { get; }

    /// <summary>
    /// The amount of data available from the source buffer, allowing users to reallocate
    /// and continue decompressing the remaining data.
    /// </summary>
    /// <remarks>
    /// Note: When compressing, this value is always 0. For compression use <see cref="BytesWritten" /> instead.
    /// </remarks>
    public uint BytesRead { get; }

    /// <summary>
    /// The Adler32 checksum of the compressed/decompressed data.
    /// </summary>
    public uint Adler32 { get; }

    /// <summary>
    /// The Crc32 checksum of the compressed/decompressed data.
    /// </summary>
    public uint Crc32 { get; }

    /// <summary>
    /// The resulting status code from zlib.
    /// </summary>
    public ZlibStatus Status { get; }
}
