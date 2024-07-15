// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

using Internal;
using Exceptions;

/// <summary>
/// Common zlib checks.
/// </summary>
public static unsafe class Checks
{
    /// <summary>
    /// Gets or sets the native zlib version to use.
    /// </summary>
    /// <remarks>
    /// Default: Version 1.2.13.
    /// </remarks>
    public static string NativeZlibVersion { get; set; } = "1.2.13";

    /// <summary>
    /// Check data for compression by gzip.
    /// </summary>
    /// <param name="source">Input data.</param>
    /// <returns>Returns <see langword="true" /> if data is compressed by gzip, else <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="source"/> is <see langword="null" />.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCompressedByGZip(ReadOnlySpan<byte> source)
    {
        if (source.Length >= 2)
        {
            ref var sourceRef = ref MemoryMarshal.GetReference(source);
            var byte1 = sourceRef;
            var byte2 = Unsafe.Add(ref sourceRef, 1);
            return byte1 is 0x1F && byte2 is 0x8B;
        }

        throw new ArgumentNullException(nameof(source));
    }

    /// <summary>
    /// Check data for compression by gzip.
    /// </summary>
    /// <param name="path">The file to check on if it is compressed by gzip.</param>
    /// <returns>Returns <see langword="true" /> if data is compressed by gzip, else <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="path"/> is <see langword="null" /> or <see cref="string.Empty"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCompressedByGZip(string path)
        => IsCompressedByGZip(File.ReadAllBytes(path));

    /// <summary>
    /// Check data for compression by zlib.
    /// </summary>
    /// <param name="source">Input data.</param>
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

    /// <summary>
    /// Gets the version to ZlibSharp.
    /// </summary>
    /// <returns>The version string to this version of ZlibSharp.</returns>
    public static string ZlibSharpVersion()
        // => typeof(Checks).Assembly.GetName().Version!.ToString(3);
        => typeof(Checks).Assembly.GetName().Version!.ToString(4);

    /// <summary>
    /// Gets the version to the imported native zlib library.
    /// </summary>
    /// <returns>The version to the imported native zlib library.</returns>
    public static string ZlibVersion()
        => Encoding.UTF8.GetString(UnsafeNativeMethods.zlibVersion(), 6);

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

    /// <summary>
    /// Gets the Crc32 checksum of the input data at the specified index and length.
    /// </summary>
    /// <param name="data">The data to checksum.</param>
    /// <returns>The Crc32 hash of the input data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ZlibGetCrc32(ReadOnlySpan<byte> data)
    {
        fixed (byte* dataPtr = data)
        {
            return UnsafeNativeMethods.crc32(
                UnsafeNativeMethods.crc32(0L, null, 0),
                dataPtr,
                (uint)data.Length);
        }
    }

    // NEW: GetCompressedSize/GetDecompressedSize to get the (de)compressed buffer size to use when (de)compressing.

    /// <summary>
    /// Gets the size of the data when it is compressed. Useful for when the data actually gets compressed.
    /// </summary>
    /// <param name="source">The input data buffer.</param>
    /// <param name="compressionLevel">The compression level to use to compress the data.</param>
    /// <param name="windowBits">The window bits to use to compress the data.</param>
    /// <exception cref="NotPackableException">
    /// Thrown when zlib errors internally in any way.
    /// </exception>
    /// <returns>The size of the data when it is compressed.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GetCompressedSize(ReadOnlySpan<byte> source, ZlibCompressionLevel compressionLevel = ZlibCompressionLevel.DefaultCompression, ZlibWindowBits windowBits = ZlibWindowBits.Zlib)
    {
        var discard = new byte[source.Length];
        var encoder = new ZlibEncoder()
        {
            CompressionLevel = compressionLevel,
            WindowBits = windowBits,
        };
        var result = encoder.Compress(source, discard);
        return result.BytesWritten;
    }

    /// <summary>
    /// Gets the size of the data when it is decompressed. Useful for when the data actually gets decompressed.
    /// </summary>
    /// <param name="source">The compressed input data.</param>
    /// <param name="windowBits">The window bits to use to decompress the data.</param>
    /// <exception cref="NotUnpackableException">
    /// Thrown when zlib errors internally in any way.
    /// </exception>
    /// <returns>The size of the data when it is decompressed.</returns>
    public static uint GetDecompressedSize(ReadOnlySpan<byte> source, ZlibWindowBits windowBits = ZlibWindowBits.Zlib)
    {
        var discard = new byte[Array.MaxLength];
        var decoder = new ZlibDecoder(windowBits);
        var result = decoder.Decompress(source, discard);
        return result.BytesWritten;
    }
}
