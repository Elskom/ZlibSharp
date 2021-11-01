// Copyright (c) 2021, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

/// <summary>
/// Zlib Memory Compression and Decompression Helper Class.
/// </summary>
public static unsafe class MemoryZlib
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Compress(byte[] Source, byte[] Dest, ZlibCompressionLevel CompressionLevel = ZlibCompressionLevel.DefaultCompression)
    {
        return Compress(Source.AsSpan(), Dest.AsSpan(), CompressionLevel);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Compress(string SourcePath, byte[] Dest, ZlibCompressionLevel CompressionLevel = ZlibCompressionLevel.DefaultCompression)
    {
        var Source = File.ReadAllBytes(SourcePath);
        
        return Compress(Source, Dest, CompressionLevel);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Compress(Span<byte> Source, Span<byte> Dest, ZlibCompressionLevel CompressionLevel = ZlibCompressionLevel.DefaultCompression)
    {
        return Compress(Source, Dest, CompressionLevel, out _);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Compress(Span<byte> Source, Span<byte> Dest, ZlibCompressionLevel CompressionLevel, out uint Adler32)
    {
        return ZlibHelper.Compress(Source, Dest, CompressionLevel, out Adler32);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Decompress(byte[] Source, byte[] Dest, out uint BytesWritten)
    {
        return Decompress(Source.AsSpan(), Dest.AsSpan(), out BytesWritten);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Decompress(string SourcePath, byte[] Dest, out uint BytesWritten)
    {
        var Source = File.ReadAllBytes(SourcePath);
        
        return Decompress(Source, Dest, out BytesWritten);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Decompress(Span<byte> Source, Span<byte> Dest, out uint BytesWritten)
    {
        return Decompress(Source, Dest, out BytesWritten, out _);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Decompress(Span<byte> Source, Span<byte> Dest, out uint BytesWritten, out uint Adler32)
    {
        return ZlibHelper.Decompress(Source, Dest, out BytesWritten, out Adler32);
    }

    /// <summary>
    /// Check data for compression by zlib.
    /// </summary>
    /// <param name="Source">Input stream.</param>
    /// <returns>Returns <see langword="true" /> if data is compressed by zlib, else <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="Source"/> is <see langword="null" />.</exception>
    public static bool IsCompressedByZlib(Span<byte> Source)
    {
        if (Source.Length >= 2)
        {
            ref var SourceRef = ref MemoryMarshal.GetReference(Source);

            var byte1 = SourceRef;

            var byte2 = Unsafe.Add(ref SourceRef, 1);
        
            return byte1 is 0x78 && byte2 is 0x01 or 0x5E or 0x9C or 0xDA;
        }

        throw new ArgumentNullException(nameof(Source));
    }

    /// <summary>
    /// Check data for compression by zlib.
    /// </summary>
    /// <param name="Path">The file to check on if it is compressed by zlib.</param>
    /// <returns>Returns <see langword="true" /> if data is compressed by zlib, else <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="Path"/> is <see langword="null" /> or <see cref="string.Empty"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCompressedByZlib(string Path)
        => IsCompressedByZlib(File.ReadAllBytes(Path));

    /// <summary>
    /// Check data for compression by zlib.
    /// </summary>
    /// <param name="Data">Input array.</param>
    /// <returns>Returns <see langword="true" /> if data is compressed by zlib, else <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="Data"/> is <see langword="null" />.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCompressedByZlib(byte[] Data)
        => IsCompressedByZlib(Data.AsSpan());

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
    /// <param name="Data">The data to checksum.</param>
    /// <returns>The Adler32 hash of the input data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ZlibGetAdler32(byte[] Data)
    {
        return ZlibGetAdler32(Data.AsSpan());
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ZlibGetAdler32(Span<byte> Data)
    {
        fixed (byte* DataPtr = Data)
        {
            return UnsafeNativeMethods.adler32(
                UnsafeNativeMethods.adler32(0L, null, 0),
                DataPtr,
                (uint) Data.Length);
        }
    }
}
