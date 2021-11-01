// Copyright (c) 2021, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

internal static unsafe class ZlibHelper
{
    private static void InitializeInflate(ZStream* StreamPtr)
    {
        var Result = UnsafeNativeMethods.inflateInit_(StreamPtr, UnsafeNativeMethods.zlibVersion(), sizeof(ZStream));
        
        if (Result != ZlibResult.Ok)
        {
            throw new NotPackableException($"{nameof(InitializeInflate)} failed - ({Result}) {Marshal.PtrToStringUTF8((nint) StreamPtr->msg)}");        
        }
    }

    private static void InitializeDeflate(ZStream* StreamPtr, ZlibCompressionLevel CompressionLevel)
    {
        var Result = UnsafeNativeMethods.deflateInit_(StreamPtr, CompressionLevel, UnsafeNativeMethods.zlibVersion(), sizeof(ZStream));
        
        if (Result != ZlibResult.Ok)
        {
            throw new NotPackableException($"{nameof(InitializeDeflate)} failed - ({Result}) {Marshal.PtrToStringUTF8((nint) StreamPtr->msg)}");        
        }
    }

    internal static void InflateEnd(ZStream* StreamPtr)
    {
        var Result = UnsafeNativeMethods.inflateEnd(StreamPtr);
        
        if (Result != ZlibResult.Ok)
        {
            throw new NotPackableException($"{nameof(InflateEnd)} failed - ({Result}) {Marshal.PtrToStringUTF8((nint) StreamPtr->msg)}");
        }
    }

    private static void DeflateEnd(ZStream* StreamPtr)
    {
        var Result = UnsafeNativeMethods.deflateEnd(StreamPtr);
        
        if (Result != ZlibResult.Ok)
        {
            throw new NotPackableException($"{nameof(DeflateEnd)} failed - ({Result}) {Marshal.PtrToStringUTF8((nint) StreamPtr->msg)}");
        }
    }
    internal static uint Compress(Span<byte> Source, Span<byte> Dest, ZlibCompressionLevel CompressionLevel, out uint Adler32)
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

            ZlibResult Result;

            while (UnsafeNativeMethods.deflate(StreamPtr, ZlibFlushStrategy.NoFlush) == ZlibResult.Ok)
            {
                if (StreamPtr->avail_in == 0)
                {
                    UnsafeNativeMethods.deflate(StreamPtr, ZlibFlushStrategy.Finish);
                }
            };

            Adler32 = GetAdler32(StreamPtr);

            DeflateEnd(StreamPtr);

            return (uint) Stream.total_out.Value;
        }
    }

    //Decompress returns avail_in, allowing users to reallocate and continue decompressing remaining data
    //should Dest buffer be under-allocated

    internal static uint Decompress(Span<byte> Source, Span<byte> Dest, out uint BytesWritten, out uint Adler32)
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

            while (UnsafeNativeMethods.inflate(StreamPtr, ZlibFlushStrategy.NoFlush) == ZlibResult.Ok)
            {
                if (StreamPtr->avail_in == 0)
                {
                    UnsafeNativeMethods.inflate(StreamPtr, ZlibFlushStrategy.Finish);
                }
            }

            BytesWritten = (uint) StreamPtr->total_out.Value;
            
            Adler32 = GetAdler32(StreamPtr);

            InflateEnd(StreamPtr);

            return StreamPtr->avail_in;
        }
    }

    private static uint GetAdler32(ZStream* StreamPtr)
    {
        return (uint) (StreamPtr->adler.Value & 0xffff);
    }
}
