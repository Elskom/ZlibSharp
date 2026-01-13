// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Internal;

internal unsafe struct CompressDecompressArgs
{
    // shared args
    internal byte* source;
    internal byte* dest;
    internal ZlibWindowBits windowBits;
    internal ZlibStatus status;

    // compress specific args
    internal ZlibCompressionLevel compressionLevel;
    internal ZlibCompressionStrategy strategy;

    // decompress specific args
    internal uint bytesWritten;
}
