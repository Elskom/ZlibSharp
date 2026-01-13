// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Internal;

using ZlibSharp.Interfaces;

internal struct Crc32Hash : ICompressionHash
{
    // <inheritdoc/>
    public unsafe readonly uint ComputeHash(ReadOnlySpan<byte> source)
    {
        fixed (byte* sourcePtr = source)
        {
            return unchecked((uint)(UnsafeNativeMethods.Crc32_ComputeHash(sourcePtr) & 0xFFFFFFFF));
        }
    }
}
