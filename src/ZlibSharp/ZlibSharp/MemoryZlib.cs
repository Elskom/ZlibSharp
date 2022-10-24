// Copyright (c) 2021~2022, Els_kom org.
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
    /// </summary>
    /// <remarks>
    /// Default: Version 1.2.11 on Linux / Windows, for MacOS 1.2.13.
    /// </remarks>
    public static string NativeZlibVersion { get; set; } = OperatingSystem.IsLinux() || OperatingSystem.IsWindows()
        ? "1.2.11"
        : "1.2.13";

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
    /// The zlib result structure that contains the amount of bytes read, written,
    /// and the adler32 hash of the data that can be used to compare the integrity
    /// of the compressed/decompressed results.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ZlibResult Compress(string sourcePath, Span<byte> dest, ZlibCompressionLevel compressionLevel = ZlibCompressionLevel.DefaultCompression)
        => Compress(File.ReadAllBytes(sourcePath), dest, compressionLevel);

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
    /// The zlib result structure that contains the amount of bytes read, written,
    /// and the adler32 hash of the data that can be used to compare the integrity
    /// of the compressed/decompressed results.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ZlibResult Compress(Span<byte> source, Span<byte> dest, ZlibCompressionLevel compressionLevel = ZlibCompressionLevel.DefaultCompression)
    {
        var bytesWritten = ZlibHelper.Compress(source, dest, compressionLevel, out var adler32);
        return new(bytesWritten, 0, adler32);
    }

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
    /// The zlib result structure that contains the amount of bytes read, written,
    /// and the adler32 hash of the data that can be used to compare the integrity
    /// of the compressed/decompressed results.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ZlibResult Decompress(string sourcePath, Span<byte> dest)
        => Decompress(File.ReadAllBytes(sourcePath), dest);

    /// <summary>
    /// Decompresses data.
    /// </summary>
    /// <param name="source">The compressed input data.</param>
    /// <param name="dest">The decompressed data buffer.</param>
    /// <exception cref="NotPackableException">
    /// Thrown when zlib errors internally in any way.
    /// </exception>
    /// <returns>
    /// The zlib result structure that contains the amount of bytes read, written,
    /// and the adler32 hash of the data that can be used to compare the integrity
    /// of the compressed/decompressed results.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ZlibResult Decompress(ReadOnlySpan<byte> source, Span<byte> dest)
    {
        var bytesRead = ZlibHelper.Decompress(source, dest, out var bytesWritten, out var adler32);
        return new ZlibResult(bytesWritten, bytesRead, adler32);
    }

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
    public static string ZlibSharpVersion()
        => typeof(MemoryZlib).Assembly.GetName().Version!.ToString(3);

    /// <summary>
    /// Gets the version to the imported native zlib library.
    /// </summary>
    /// <returns>The version to the imported native zlib library.</returns>
    public static string ZlibVersion()
        => new((char*)UnsafeNativeMethods.zlibVersion());

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
