// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Internal;

using ZlibSharp.Interfaces;

internal struct Crc32Hash : ICompressionHash
{
    // <inheritdoc/>
    public readonly uint ComputeHash(ReadOnlySpan<byte> source)
        => unchecked((uint)(ZlibHelper.GetCrc32(source) & 0xFFFFFFFF));
}
