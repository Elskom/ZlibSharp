// Copyright (c) 2021~2026, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

// using Internal;

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
    /// Gets the size of the data when it is compressed. Useful for when the data actually gets compressed.
    /// </summary>
    /// <param name="source">The input data buffer.</param>
    /// <returns>The size of the data when it is compressed.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong GetCompressedSize(ReadOnlySpan<byte> source)
    {
        var discard = new byte[source.Length];
        _ = this.TryCompress(source, discard, out var bytesWritten, out _);
        return bytesWritten;
    }

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
    public bool TryCompress(string sourcePath, Span<byte> dest, out ulong bytesWritten, out OperationStatus status)
        => this.TryCompress(File.ReadAllBytes(sourcePath), dest, out bytesWritten, out status);

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
    public bool TryCompress(ReadOnlySpan<byte> source, Span<byte> dest, out ulong bytesWritten, out OperationStatus status)
        => this.TryCompressCore(source, dest, out bytesWritten, out status);

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
    public OperationStatus Compress(string sourcePath, Span<byte> dest, out ulong bytesWritten)
        => this.Compress(File.ReadAllBytes(sourcePath), dest, out bytesWritten);

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
    public OperationStatus Compress(ReadOnlySpan<byte> source, Span<byte> dest, out ulong bytesWritten)
        => this.CompressCore(source, dest, out bytesWritten);

    [ExcludeFromCodeCoverage]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private OperationStatus CompressCore(ReadOnlySpan<byte> source, Span<byte> dest, out ulong bytesWritten)
    {
        bytesWritten = ZlibHelper.Compress(source, dest, this.Options.CompressionLevel, this.Options.WindowBits, this.Options.Strategy, out var zstatus);
        var status = zstatus.ToOperationStatus();
        return status;
    }

    [ExcludeFromCodeCoverage]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryCompressCore(ReadOnlySpan<byte> source, Span<byte> dest, out ulong bytesWritten, out OperationStatus status)
    {
        status = this.CompressCore(source, dest, out bytesWritten);
        return status == OperationStatus.Done;
    }
}
