// Copyright (c) 2021~2026, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Internal;

[ExcludeFromCodeCoverage]
internal static unsafe class ZlibHelper
{
    internal static ulong Compress(ReadOnlySpan<byte> source, Span<byte> dest, ZlibCompressionLevel compressionLevel, ZlibWindowBits windowBits, ZlibCompressionStrategy strategy, out ZlibStatus status)
    {
        OSHelpers.LoadNativeLibrary();
        CompressDecompressArgs args = default;
        var argsPtr = &args;
        fixed (byte* sourcePtr = source)
        fixed (byte* destPtr = dest)
        {
            argsPtr->source = sourcePtr;
            argsPtr->source_length = (uint)source.Length;
            argsPtr->dest = destPtr;
            argsPtr->dest_length = (uint)dest.Length;
            argsPtr->compressionLevel = compressionLevel;
            argsPtr->windowBits = windowBits;
            argsPtr->strategy = strategy;
            _ = UnsafeNativeMethods.Compress(argsPtr);
            status = argsPtr->status;
            return argsPtr->bytesWritten.ToUInt64();
        }
    }

    //Decompress returns avail_in, allowing users to reallocate and continue decompressing remaining data
    //should Dest buffer be under-allocated
    internal static uint Decompress(ReadOnlySpan<byte> source, Span<byte> dest, out ulong bytesWritten, out ZlibStatus status, ZlibWindowBits windowBits)
    {
        OSHelpers.LoadNativeLibrary();
        CompressDecompressArgs args = default;
        var argsPtr = &args;
        fixed (byte* sourcePtr = source)
        fixed (byte* destPtr = dest)
        {
            argsPtr->source = sourcePtr;
            argsPtr->source_length = (uint)source.Length;
            argsPtr->dest = destPtr;
            argsPtr->dest_length = (uint)dest.Length;
            argsPtr->windowBits = windowBits;
            var result = UnsafeNativeMethods.Decompress(argsPtr);
            bytesWritten = argsPtr->bytesWritten.ToUInt64();
            status = argsPtr->status;
            return result;
        }
    }
}
