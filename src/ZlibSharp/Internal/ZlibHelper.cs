// Copyright (c) 2021~2026, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Internal;

internal static unsafe class ZlibHelper
{
    internal static ulong Compress(ReadOnlySpan<byte> source, ZlibCompressionLevel compressionLevel, ZlibWindowBits windowBits, ZlibCompressionStrategy strategy, out CompressionSpan<byte> dest, out ZlibStatus status)
    {
        OSHelpers.LoadNativeLibrary();
        CompressDecompressArgs args = default;
        var argsPtr = &args;
        fixed (byte* sourcePtr = source)
        {
            argsPtr->source = sourcePtr;
            argsPtr->source_length = (uint)source.Length;
            argsPtr->compressionLevel = compressionLevel;
            argsPtr->windowBits = windowBits;
            argsPtr->strategy = strategy;
            _ = UnsafeNativeMethods.Compress(argsPtr);
            var bytesWritten = argsPtr->bytesWritten.ToUInt64();
            dest = CompressionSpan<byte>.Create(argsPtr->dest, bytesWritten);
            UnsafeNativeMethods.FreeOutput(argsPtr->dest);
            status = argsPtr->status;
            return bytesWritten;
        }
    }

    //Decompress returns avail_in, allowing users to reallocate and continue decompressing remaining data
    //should Dest buffer be under-allocated
    internal static uint Decompress(ReadOnlySpan<byte> source, out CompressionSpan<byte> dest, out ulong bytesWritten, out ZlibStatus status, ZlibWindowBits windowBits)
    {
        OSHelpers.LoadNativeLibrary();
        CompressDecompressArgs args = default;
        var argsPtr = &args;
        fixed (byte* sourcePtr = source)
        {
            argsPtr->source = sourcePtr;
            argsPtr->source_length = (uint)source.Length;
            argsPtr->windowBits = windowBits;
            var result = UnsafeNativeMethods.Decompress(argsPtr);
            bytesWritten = argsPtr->bytesWritten.ToUInt64();
            dest = CompressionSpan<byte>.Create(argsPtr->dest, bytesWritten);
            UnsafeNativeMethods.FreeOutput(argsPtr->dest);
            status = argsPtr->status;
            return result;
        }
    }
}
