// Copyright (c) 2021, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

/// <summary>
/// Zlib Memory Compression and Decompression Class.
/// </summary>
public static unsafe class MemoryZlib
{
    /// <summary>
    /// Gets or sets the native zlib version to use.
    ///
    /// Default: version 1.2.11
    /// </summary>
    public static string NativeZlibVersion { get; set; } = "1.2.11";

    /// <summary>
    /// Compresses data using the user specified compression level.
    /// </summary>
    /// <param name="sourcePath">The path to the file to compress.</param>
    /// <param name="dest">The compressed data buffer.</param>
    /// <param name="compressionLevel">The compression level to use to compress the file.</param>
    /// <exception cref="NotPackableException">
    /// Thrown when zlib errors internally in any way.
    /// </exception>
    /// <returns>
    /// The amount of bytes written to the destination buffer.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Compress(string sourcePath, Span<byte> dest, ZlibCompressionLevel compressionLevel = ZlibCompressionLevel.DefaultCompression)
    {
        var source = File.ReadAllBytes(sourcePath);
        return Compress(source, dest, compressionLevel);
    }

    /// <summary>
    /// Compresses data using the user specified compression level.
    /// </summary>
    /// <param name="source">The input data buffer.</param>
    /// <param name="dest">The compressed data buffer.</param>
    /// <param name="compressionLevel">The compression level to use to compress the file.</param>
    /// <exception cref="NotPackableException">
    /// Thrown when zlib errors internally in any way.
    /// </exception>
    /// <returns>
    /// The amount of bytes written to the destination buffer.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Compress(Span<byte> source, Span<byte> dest, ZlibCompressionLevel compressionLevel = ZlibCompressionLevel.DefaultCompression)
        => Compress(source, dest, compressionLevel, out _);

    /// <summary>
    /// Compresses data using the user specified compression level.
    /// </summary>
    /// <param name="source">The input data buffer.</param>
    /// <param name="dest">The compressed data buffer.</param>
    /// <param name="compressionLevel">The compression level to use to compress the file.</param>
    /// <param name="adler32">The Adler32 checksum of the compressed data.</param>
    /// <exception cref="NotPackableException">
    /// Thrown when zlib errors internally in any way.
    /// </exception>
    /// <returns>
    /// The amount of bytes written to the destination buffer.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Compress(ReadOnlySpan<byte> source, Span<byte> dest, ZlibCompressionLevel compressionLevel, out uint adler32)
        => ZlibHelper.Compress(source, dest, compressionLevel, out adler32);

    /// <summary>
    /// Decompresses a file.
    /// </summary>
    /// <param name="sourcePath">The path to the file to decompress.</param>
    /// <param name="dest">The decompressed data buffer.</param>
    /// <param name="bytesWritten">The amount of bytes written to the destination buffer.</param>
    /// <exception cref="NotPackableException">
    /// Thrown when zlib errors internally in any way.
    /// </exception>
    /// <returns>
    /// The amount of data available from the source buffer, allowing users to reallocate
    /// and continue decompressing the remaining data.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Decompress(string sourcePath, Span<byte> dest, out uint bytesWritten)
    {
        var source = File.ReadAllBytes(sourcePath);
        return Decompress(source, dest, out bytesWritten);
    }

    /// <summary>
    /// Decompresses data.
    /// </summary>
    /// <param name="source">The compressed input data.</param>
    /// <param name="dest">The decompressed data buffer.</param>
    /// <param name="bytesWritten">The amount of bytes written to the destination buffer.</param>
    /// <exception cref="NotPackableException">
    /// Thrown when zlib errors internally in any way.
    /// </exception>
    /// <returns>
    /// The amount of data available from the source buffer, allowing users to reallocate
    /// and continue decompressing the remaining data.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Decompress(ReadOnlySpan<byte> source, Span<byte> dest, out uint bytesWritten)
        => Decompress(source, dest, out bytesWritten, out _);

    /// <summary>
    /// Decompresses data.
    /// </summary>
    /// <param name="source">The compressed input data.</param>
    /// <param name="dest">The decompressed data buffer.</param>
    /// <param name="bytesWritten">The amount of bytes written to the destination buffer.</param>
    /// <param name="adler32">The Adler32 checksum of the decompressed data.</param>
    /// <exception cref="NotPackableException">
    /// Thrown when zlib errors internally in any way.
    /// </exception>
    /// <returns>
    /// The amount of data available from the source buffer, allowing users to reallocate
    /// and continue decompressing the remaining data.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Decompress(ReadOnlySpan<byte> source, Span<byte> dest, out uint bytesWritten, out uint adler32)
        => ZlibHelper.Decompress(source, dest, out bytesWritten, out adler32);

    /// <summary>
    /// Check data for compression by zlib.
    /// </summary>
    /// <param name="source">Input stream.</param>
    /// <returns>Returns <see langword="true" /> if data is compressed by zlib, else <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="source"/> is <see langword="null" />.</exception>
    public static bool IsCompressedByZlib(ReadOnlySpan<byte> source)
    {
        if (source.Length >= 2)
        {
            ref var sourceRef = ref MemoryMarshal.GetReference(source);
            var byte1 = sourceRef;
            var byte2 = Unsafe.Add(ref sourceRef, 1);
            return byte1 is 0x78 && byte2 is 0x01 or 0x5E or 0x9C or 0xDA;
        }

        throw new ArgumentNullException(nameof(source));
    }

    /// <summary>
    /// Check data for compression by zlib.
    /// </summary>
    /// <param name="path">The file to check on if it is compressed by zlib.</param>
    /// <returns>Returns <see langword="true" /> if data is compressed by zlib, else <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="path"/> is <see langword="null" /> or <see cref="string.Empty"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCompressedByZlib(string path)
        => IsCompressedByZlib(File.ReadAllBytes(path));

    // NEW: Zlib version check.

    /// <summary>
    /// Gets the version to ZlibSharp.
    /// </summary>
    /// <returns>The version string to this version of ZlibSharp.</returns>
    public static string ZlibVersion()
        => typeof(MemoryZlib).Assembly.GetName().Version!.ToString(3);

    // NEW: Adler32 hasher.

    /// <summary>
    /// Gets the Adler32 checksum of the input data at the specified index and length.
    /// </summary>
    /// <param name="data">The data to checksum.</param>
    /// <returns>The Adler32 hash of the input data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ZlibGetAdler32(ReadOnlySpan<byte> data)
    {
        fixed (byte* dataPtr = data)
        {
            return UnsafeNativeMethods.adler32(
                UnsafeNativeMethods.adler32(0L, null, 0),
                dataPtr,
                (uint)data.Length);
        }
    }
}
