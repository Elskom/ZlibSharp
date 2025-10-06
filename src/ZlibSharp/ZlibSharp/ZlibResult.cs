// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

using System.Buffers;
using ZlibSharp.Internal;

/// <summary>
/// The zlib result structure that contains the amount of bytes read, written,
/// and the adler32 hash of the data that can be used to compare the integrity
/// of the compressed/decompressed results.
/// </summary>
public readonly struct ZlibResult
{
    internal ZlibResult(uint bytesWritten, uint bytesRead, uint hash, ZlibStatus status)
    {
        this.BytesWritten = bytesWritten;
        this.BytesRead = bytesRead;
        this.Hash = hash;
        this.Status = status.ToOperationStatus();
    }

    /// <summary>
    /// Gets the amount of bytes written to the destination buffer.
    /// </summary>
    public uint BytesWritten { get; }

    /// <summary>
    /// Gets the amount of data available from the source buffer, allowing users to reallocate
    /// and continue decompressing the remaining data.
    /// </summary>
    /// <remarks>
    /// Note: When compressing, this value is always 0. For compression use <see cref="BytesWritten" /> instead.
    /// </remarks>
    public uint BytesRead { get; }

    /// <summary>
    /// Gets the Adler32 checksum of the compressed/decompressed data if
    /// it was compressed/decompressed with <see cref="ZlibWindowBits.Deflate" />
    /// or <see cref="ZlibWindowBits.Zlib" />, the Crc32 checksum otherwise.
    /// </summary>
    public uint Hash { get; }

    /// <summary>
    /// The resulting status code from zlib.
    /// </summary>
    public OperationStatus Status { get; }
}
