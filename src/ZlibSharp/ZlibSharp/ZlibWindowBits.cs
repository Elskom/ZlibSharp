// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

/// <summary>
/// Represents the supported Window Bits for zlib compression/decompression.
/// </summary>
public enum ZlibWindowBits
{
    /// <summary>
    /// The window bits for deflate.
    /// </summary>
    Deflate = -15,

    /// <summary>
    /// The window bits for zlib streams.
    /// </summary>
    Zlib = 15,

    /// <summary>
    /// The window bits for gzip streams.
    /// </summary>
    GZip = 31,
}
