// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

/// <summary>
/// Zlib's supported compression levels.
/// </summary>
public enum ZlibCompressionLevel
{
    /// <summary>
    /// The default compression level.
    /// </summary>
    DefaultCompression = -1,

    /// <summary>
    /// No compression.
    /// </summary>
    NoCompression,

    /// <summary>
    /// Compression level 1.
    /// </summary>
    Level1,

    /// <summary>
    /// Best speed compression level (level 1).
    /// </summary>
    BestSpeed = Level1,

    /// <summary>
    /// Compression level 2.
    /// </summary>
    Level2,

    /// <summary>
    /// Compression level 3.
    /// </summary>
    Level3,

    /// <summary>
    /// Compression level 4.
    /// </summary>
    Level4,

    /// <summary>
    /// Compression level 5.
    /// </summary>
    Level5,

    /// <summary>
    /// Compression level 6.
    /// </summary>
    Level6,

    /// <summary>
    /// Compression level 7.
    /// </summary>
    Level7,

    /// <summary>
    /// Compression level 8.
    /// </summary>
    Level8,

    /// <summary>
    /// Compression level 9.
    /// </summary>
    Level9,

    /// <summary>
    /// The best compression level (level 9).
    /// </summary>
    BestCompression = Level9,
}
