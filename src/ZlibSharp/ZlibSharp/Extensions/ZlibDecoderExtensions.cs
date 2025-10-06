namespace ZlibSharp.Extensions;

using ZlibSharp.Exceptions;

/// <summary>
/// Extension methods for the <see cref="ZlibDecoder" /> class.
/// </summary>
public static class ZlibDecoderExtensions
{
    /// <summary>
    /// Gets the size of the data when it is decompressed. Useful for when the data actually gets decompressed.
    /// </summary>
    /// <param name="decoder">The <see cref="ZlibDecoder" /> instance to use.</param>
    /// <param name="source">The compressed input data.</param>
    /// <exception cref="NotUnpackableException">
    /// Thrown when zlib errors internally in any way.
    /// </exception>
    /// <returns>The size of the data when it is decompressed.</returns>
    public static uint GetDecompressedSize(this ZlibDecoder decoder, ReadOnlySpan<byte> source)
    {
        var discard = new byte[Array.MaxLength];
        _ = decoder.TryDecompress(source, discard, out var result);
        return result.BytesWritten;
    }

    /// <summary>
    /// Check data for compression by gzip.
    /// </summary>
    /// <param name="_">The <see cref="ZlibDecoder" /> instance to use.</param>
    /// <param name="source">Input data.</param>
    /// <returns>Returns <see langword="true" /> if data is compressed by gzip, else <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="source"/> is <see langword="null" />.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCompressedByGZip(this ZlibDecoder _, ReadOnlySpan<byte> source)
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
    /// <param name="decoder">The <see cref="ZlibDecoder" /> instance to use.</param>
    /// <param name="path">The file to check on if it is compressed by gzip.</param>
    /// <returns>Returns <see langword="true" /> if data is compressed by gzip, else <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="path"/> is <see langword="null" /> or <see cref="string.Empty"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCompressedByGZip(this ZlibDecoder decoder, string path)
        => IsCompressedByGZip(decoder, File.ReadAllBytes(path));

    /// <summary>
    /// Check data for compression by zlib.
    /// </summary>
    /// <param name="_">The <see cref="ZlibDecoder" /> instance to use.</param>
    /// <param name="source">Input data.</param>
    /// <returns>Returns <see langword="true" /> if data is compressed by zlib, else <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="source"/> is <see langword="null" />.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCompressedByZlib(this ZlibDecoder _, ReadOnlySpan<byte> source)
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
    /// <param name="decoder">The <see cref="ZlibDecoder" /> instance to use.</param>
    /// <param name="path">The file to check on if it is compressed by zlib.</param>
    /// <returns>Returns <see langword="true" /> if data is compressed by zlib, else <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException">When <paramref name="path"/> is <see langword="null" /> or <see cref="string.Empty"/>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCompressedByZlib(this ZlibDecoder decoder, string path)
        => IsCompressedByZlib(decoder, File.ReadAllBytes(path));

    [ExcludeFromCodeCoverage]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsZlibHeader(byte byte1, byte byte2)
        => byte1 is 0x78 && byte2 is 0x01 or 0x5E or 0x9C or 0xDA;
}
