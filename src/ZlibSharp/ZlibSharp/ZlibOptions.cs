// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

using ZlibSharp.Interfaces;
using ZlibSharp.Internal;

/// <summary>
/// Represents configuration options for zlib compression and decompression operations.
/// </summary>
/// <remarks>This class provides settings to control the behavior of zlib-based compression and decompression. The
/// options include the compression level, window size, and compression strategy.  Note that some properties, such as
/// <see cref="CompressionLevel"/> and <see cref="Strategy"/>,  are only applicable during compression and are ignored
/// during decompression.</remarks>
public class ZlibOptions
{
    private readonly ICompressionHash adler32hash = new Adler32Hash();
    private readonly ICompressionHash crc32hash = new Crc32Hash();

    /// <summary>
    /// Initializes a new instance of the <see cref="ZlibOptions"/> class with default settings for compression/decompression.
    /// </summary>
    internal ZlibOptions()
        => this.Reset();

    /// <summary>
    /// Gets or sets the compression level to use to compress the data.
    /// </summary>
    /// <remarks>
    /// Note: This value is ignored when decompressing data.
    /// </remarks>
    public ZlibCompressionLevel CompressionLevel { get; set; }

    /// <summary>
    /// Gets or sets the window bits to use to compress/decompress the data.
    ///
    /// This also affects the hash algorithm used for integrity checks as well.
    /// </summary>
    public ZlibWindowBits WindowBits {
        get;
        set
        {
            this.HashAlgorithm = !value.Equals(ZlibWindowBits.GZip)
                ? this.adler32hash
                : this.crc32hash;
            field = value;
        }
    }

    /// <summary>
    /// Gets or sets the compression strategy to use to compress the data.
    /// </summary>
    /// <remarks>
    /// Note: This value is ignored when decompressing data.
    /// </remarks>
    public ZlibCompressionStrategy Strategy { get; set; }

    /// <summary>
    /// Gets or sets the hash algorithm to use.
    /// </summary>
    public ICompressionHash HashAlgorithm { get; set; } = null!;

    /// <summary>
    /// Resets the options to their default values.
    /// </summary>
    public void Reset()
    {
        this.CompressionLevel = ZlibCompressionLevel.DefaultCompression;
        this.WindowBits = ZlibWindowBits.Zlib;
        this.Strategy = ZlibCompressionStrategy.Default;
    }
}
