// Copyright (c) 2021, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

using System.Runtime.CompilerServices;

namespace ZlibSharp;

internal static unsafe class ZlibHelper
{
    private static void InitializeInflate(ZStream* streamPtr)
    {
        if (UnsafeNativeMethods.inflateInit_(
                streamPtr,
                UnsafeNativeMethods.zlibVersion(),
                sizeof(ZStream)) != ZlibResult.Ok)
        {
            throw new NotPackableException("inflateInit failed while trying to inflate.");
        }
    }

    private static void InitializeDeflate(ZStream* streamPtr, ZlibCompressionLevel compressionLevel)
    {
        if (UnsafeNativeMethods.deflateInit_(
                streamPtr,
                compressionLevel,
                UnsafeNativeMethods.zlibVersion(),
                sizeof(ZStream)) != ZlibResult.Ok)
        {
            throw new NotPackableException("deflateInit failed while trying to deflate.");
        }
    }

    internal static unsafe void FinalizeInflate(ZStream* StreamPtr)
    {
        if (UnsafeNativeMethods.inflateEnd(StreamPtr) != ZlibResult.Ok)
        {
            throw new NotPackableException("inflateEnd failed while trying to inflate.");
        }
    }

    internal static void FinalizeDeflate(ZStream* StreamPtr)
    {
        if (UnsafeNativeMethods.deflateEnd(StreamPtr) != ZlibResult.Ok)
        {
            throw new NotPackableException("deflateEnd failed while trying to deflate.");
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint Compress(Span<byte> Source, Span<byte> Dest, ZlibCompressionLevel CompressionLevel, out uint Adler32)
    {
        return Compress(Source, Dest, CompressionLevel, ZlibFlushStrategy.NoFlush, out Adler32);
    }

    internal static uint Compress(Span<byte> Source, Span<byte> Dest, ZlibCompressionLevel CompressionLevel, ZlibFlushStrategy Flush, out uint Adler32)
    {
        ZStream Stream;

        var StreamPtr = &Stream;
        
        //We skipped initialization
        StreamPtr->zalloc = null;
        StreamPtr->zfree = null;
        
        fixed (byte* SourcePtr = Source)
        fixed (byte* DestPtr = Dest)
        {
            InitializeDeflate(StreamPtr, CompressionLevel);
            
            StreamPtr->next_in = SourcePtr;
            StreamPtr->avail_in = (uint) Source.Length;
            StreamPtr->next_out = DestPtr;
            StreamPtr->avail_out = (uint) Dest.Length;
            
            var Result = UnsafeNativeMethods.deflate(StreamPtr, Flush);

            Adler32 = GetAdler32(StreamPtr);

            FinalizeDeflate(StreamPtr);

            if (Result != ZlibResult.StreamEnd)
            {
                throw new NotPackableException($"Inflate Error - ({Result}) {Marshal.PtrToStringUTF8((nint)Stream.msg)}");
            }

            return (uint) Stream.total_out.Value;
        }
    }

    //Decompress returns avail_in, allowing users to reallocate and continue decompressing remaining data
    //should Dest buffer be under-allocated
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint Decompress(Span<byte> Source, Span<byte> Dest, out uint Adler32)
    {
        return Decompress(Source, Dest, ZlibFlushStrategy.NoFlush, out Adler32);
    }
    
    internal static uint Decompress(Span<byte> Source, Span<byte> Dest, ZlibFlushStrategy Flush, out uint Adler32)
    {
        ZStream Stream;

        var StreamPtr = &Stream;

        //We skipped initialization
        StreamPtr->zalloc = null;
        StreamPtr->zfree = null;
        
        fixed (byte* SourcePtr = Source)
        fixed (byte* DestPtr = Dest)
        {
            InitializeInflate(StreamPtr);
            
            StreamPtr->next_in = SourcePtr;
            StreamPtr->avail_in = (uint) Source.Length;
            StreamPtr->next_out = DestPtr;
            StreamPtr->avail_out = (uint) Dest.Length;
            
            var Result = UnsafeNativeMethods.deflate(StreamPtr, Flush);

            Adler32 = GetAdler32(StreamPtr);
            
            FinalizeInflate(StreamPtr);
            
            if (Result is ZlibResult.StreamEnd or ZlibResult.BufError)
            {
                return Stream.avail_in;
            }
        
            throw new NotPackableException($"Inflate Error - ({Result}) {Marshal.PtrToStringUTF8((nint) Stream.msg)}");
        }
    }

    private static uint GetAdler32(ZStream* StreamPtr)
    {
        return (uint) (StreamPtr->adler.Value & 0xffff);
    }
}
