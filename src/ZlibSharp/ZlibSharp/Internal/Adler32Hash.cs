// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Internal;

using ZlibSharp.Interfaces;

internal struct Adler32Hash : ICompressionHash
{
    // <inheritdoc/>
    public unsafe readonly uint ComputeHash(ReadOnlySpan<byte> source)
    {
        fixed (byte* sourcePtr = source)
        {
            return unchecked((uint)(UnsafeNativeMethods.Adler32_ComputeHash(sourcePtr) & 0xFFFFFFFF));
        }
    }

    // private const uint Base = 65521; // largest prime smaller than 65536
    // private const int NMax = 5552;   // NMax is the largest n such that 255n(n+1)/2 + (n+1)(BASE-1) <= 2^32-1

    // adler32 is a copy of the zlib implementation of the same name.
    // private static uint adler32(uint adler, ReadOnlySpan<byte> buf)
    // {
    //     if (buf.IsEmpty)
    //     {
    //         return 1;
    //     }
    //     var s1 = adler & 0xFFFF;
    //     var s2 = (adler >> 16) & 0xFFFF;
    //     while (buf.Length > 0)
    //     {
    //         var k = buf.Length < NMax ? buf.Length : NMax;
    //         for (var i = 0; i < k; i++)
    //         {
    //             s1 += buf[i];
    //             s2 += s1;
    //         }
    //         s1 %= Base;
    //         s2 %= Base;
    //         buf = buf[k..];
    //     }
    //     return (s2 << 16) | s1;
    // }
}
