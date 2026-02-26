// Copyright (c) 2021~2026, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

/// <summary>
/// Zlib Memory Decompression class.
/// </summary>
public class ZlibDecoder
{
    private static readonly Lazy<ZlibDecoder> _default = new(() => new ZlibDecoder());

    internal ZlibDecoder()
        => this.Options = new ZlibOptions();

    /// <summary>
    /// Gets the default instance of the <see cref="ZlibDecoder" /> class.
    /// </summary>
    public static ZlibDecoder Default
        => _default.Value;

    /// <summary>
    /// Gets or sets the options to use to decompress the data.
    /// </summary>
    public ZlibOptions Options { get; internal set; }

    /// <summary>
    /// Decompresses a file.
    /// </summary>
    /// <param name="sourcePath">The path to the file to decompress.</param>
    /// <param name="dest">The decompressed data buffer.</param>
    /// <param name="bytesWritten">The amount of bytes written to the destination buffer.</param>
    /// <param name="bytesRead">The amount of data available from the source buffer, allowing users to reallocate
    /// and continue decompressing the remaining data.</param>
    /// <param name="status">The resulting status code from zlib.</param>
    /// <returns>
    /// <see langword="true"/> if the compression was a success, <see langword="false"/> otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryDecompress(string sourcePath, out CompressionSpan<byte> dest, out ulong bytesWritten, out uint bytesRead, out OperationStatus status)
        => this.TryDecompress(File.ReadAllBytes(sourcePath), out dest, out bytesWritten, out bytesRead, out status);

    /// <summary>
    /// Decompresses data.
    /// </summary>
    /// <param name="source">The compressed input data.</param>
    /// <param name="dest">The decompressed data buffer.</param>
    /// <param name="bytesWritten">The amount of bytes written to the destination buffer.</param>
    /// <param name="bytesRead">The amount of data available from the source buffer, allowing users to reallocate
    /// and continue decompressing the remaining data.</param>
    /// <param name="status">The resulting status code from zlib.</param>
    /// <returns>
    /// <see langword="true"/> if the compression was a success, <see langword="false"/> otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryDecompress(ReadOnlySpan<byte> source, out CompressionSpan<byte> dest, out ulong bytesWritten, out uint bytesRead, out OperationStatus status)
        => this.TryDecompressCore(source, out dest, out bytesWritten, out bytesRead, out status);

    /// <summary>
    /// Decompresses a file.
    /// </summary>
    /// <param name="sourcePath">The path to the file to decompress.</param>
    /// <param name="dest">The decompressed data buffer.</param>
    /// <param name="bytesWritten">The amount of bytes written to the destination buffer.</param>
    /// <param name="bytesRead">The amount of data available from the source buffer, allowing users to reallocate
    /// and continue decompressing the remaining data.</param>
    /// <returns>
    /// The resulting status code from zlib.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public OperationStatus Decompress(string sourcePath, out CompressionSpan<byte> dest, out ulong bytesWritten, out uint bytesRead)
        => this.Decompress(File.ReadAllBytes(sourcePath), out dest, out bytesWritten, out bytesRead);

    /// <summary>
    /// Decompresses data.
    /// </summary>
    /// <param name="source">The compressed input data.</param>
    /// <param name="dest">The decompressed data buffer.</param>
    /// <param name="bytesWritten">The amount of bytes written to the destination buffer.</param>
    /// <param name="bytesRead">The amount of data available from the source buffer, allowing users to reallocate
    /// and continue decompressing the remaining data.</param>
    /// <returns>
    /// The resulting status code from zlib.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public OperationStatus Decompress(ReadOnlySpan<byte> source, out CompressionSpan<byte> dest, out ulong bytesWritten, out uint bytesRead)
        => this.DecompressCore(source, out dest, out bytesWritten, out bytesRead);

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCompressedByZlib(ReadOnlySpan<byte> source)
    {
        if (source.Length >= 2)
        {
            ref var sourceRef = ref MemoryMarshal.GetReference(source);
            var byte1 = sourceRef;
            var byte2 = Unsafe.Add(ref sourceRef, 1);
            return IsZlibHeader(byte1, byte2);
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

    [ExcludeFromCodeCoverage]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsZlibHeader(byte byte1, byte byte2)
        => byte1 is 0x78 && byte2 is 0x01 or 0x5E or 0x9C or 0xDA;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private OperationStatus DecompressCore(ReadOnlySpan<byte> source, out CompressionSpan<byte> dest, out ulong bytesWritten, out uint bytesRead)
    {
        bytesRead = ZlibHelper.Decompress(source, out dest, out bytesWritten, out var zstatus, this.Options.WindowBits);
        var status = zstatus.ToOperationStatus();
        return status;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryDecompressCore(ReadOnlySpan<byte> source, out CompressionSpan<byte> dest, out ulong bytesWritten, out uint bytesRead, out OperationStatus status)
    {
        status = this.DecompressCore(source, out dest, out bytesWritten, out bytesRead);
        return status == OperationStatus.Done;
    }
}
