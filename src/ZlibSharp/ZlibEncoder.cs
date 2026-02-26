// Copyright (c) 2021~2026, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

/// <summary>
/// Zlib Memory Compression class.
/// </summary>
public class ZlibEncoder
{
    private static readonly Lazy<ZlibEncoder> _default = new(() => new ZlibEncoder());

    internal ZlibEncoder()
        => this.Options = new ZlibOptions();

    /// <summary>
    /// Gets the default instance of the <see cref="ZlibEncoder" /> class.
    /// </summary>
    public static ZlibEncoder Default
        => _default.Value;

    /// <summary>
    /// Gets or sets the options to use to compress the file.
    /// </summary>
    public ZlibOptions Options { get; internal set; }

    /// <summary>
    /// Tries to compress a file using the user specified compression level.
    /// </summary>
    /// <param name="sourcePath">The path to the file to compress.</param>
    /// <param name="dest">The compressed data buffer.</param>
    /// <param name="bytesWritten">The amount of bytes written to the destination buffer.</param>
    /// <param name="status">The resulting status code from zlib.</param>
    /// <returns>
    /// <see langword="true"/> if the compression was a success, <see langword="false"/> otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCompress(string sourcePath, out CompressionSpan<byte> dest, out ulong bytesWritten, out OperationStatus status)
        => this.TryCompress(File.ReadAllBytes(sourcePath), out dest, out bytesWritten, out status);

    /// <summary>
    /// Tries to compress the data using the user specified compression level.
    /// </summary>
    /// <param name="source">The input data buffer.</param>
    /// <param name="dest">The compressed data buffer.</param>
    /// <param name="bytesWritten">The amount of bytes written to the destination buffer.</param>
    /// <param name="status">The resulting status code from zlib.</param>
    /// <returns>
    /// <see langword="true"/> if the compression was a success, <see langword="false"/> otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCompress(ReadOnlySpan<byte> source, out CompressionSpan<byte> dest, out ulong bytesWritten, out OperationStatus status)
        => this.TryCompressCore(source, out dest, out bytesWritten, out status);

    /// <summary>
    /// Compresses a file using the user specified compression level.
    /// </summary>
    /// <param name="sourcePath">The path to the file to compress.</param>
    /// <param name="dest">The compressed data buffer.</param>
    /// <param name="bytesWritten">The amount of bytes written to the destination buffer.</param>
    /// <returns>
    /// The resulting status code from zlib.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public OperationStatus Compress(string sourcePath, out CompressionSpan<byte> dest, out ulong bytesWritten)
        => this.Compress(File.ReadAllBytes(sourcePath), out dest, out bytesWritten);

    /// <summary>
    /// Compresses data using the user specified compression level.
    /// </summary>
    /// <param name="source">The input data buffer.</param>
    /// <param name="dest">The compressed data buffer.</param>
    /// <param name="bytesWritten">The amount of bytes written to the destination buffer.</param>
    /// <returns>
    /// The resulting status code from zlib.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public OperationStatus Compress(ReadOnlySpan<byte> source, out CompressionSpan<byte> dest, out ulong bytesWritten)
        => this.CompressCore(source, out dest, out bytesWritten);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private OperationStatus CompressCore(ReadOnlySpan<byte> source, out CompressionSpan<byte> dest, out ulong bytesWritten)
    {
        bytesWritten = ZlibHelper.Compress(source, this.Options.CompressionLevel, this.Options.WindowBits, this.Options.Strategy, out dest, out var zstatus);
        var status = zstatus.ToOperationStatus();
        return status;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryCompressCore(ReadOnlySpan<byte> source, out CompressionSpan<byte> dest, out ulong bytesWritten, out OperationStatus status)
    {
        status = this.CompressCore(source, out dest, out bytesWritten);
        return status == OperationStatus.Done;
    }
}
