// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

/// <summary>
/// Zlib compression strategies.
/// </summary>
public enum ZlibCompressionStrategy
{
    /// <summary>
    /// Default strategy.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Filtered strategy.
    /// </summary>
    Filtered = 1,

    /// <summary>
    /// Huffman strategy only.
    /// </summary>
    HuffmanOnly = 2,

    /// <summary>
    /// RLE strategy.
    /// </summary>
    Rle = 3,

    /// <summary>
    /// Fixed strategy.
    /// </summary>
    Fixed = 4,
}
