// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Extensions;

using ZlibSharp.Exceptions;

/// <summary>
/// Extension methods for the <see cref="ZlibEncoder" /> class.
/// </summary>
public static class ZlibEncoderExtensions
{
    /// <summary>
    /// Gets the size of the data when it is compressed. Useful for when the data actually gets compressed.
    /// </summary>
    /// <param name="encoder">The <see cref="ZlibEncoder" /> instance to use.</param>
    /// <param name="source">The input data buffer.</param>
    /// <returns>The size of the data when it is compressed.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GetCompressedSize(this ZlibEncoder encoder, ReadOnlySpan<byte> source)
    {
        var discard = new byte[source.Length];
        _ = encoder.TryCompress(source, discard, out var bytesWritten, out _, out _);
        return bytesWritten;
    }
}
