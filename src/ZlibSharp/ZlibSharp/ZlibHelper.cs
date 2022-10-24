// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

internal static unsafe class ZlibHelper
{
    private static bool zlibResolverAdded;

    internal static uint Compress(ReadOnlySpan<byte> source, Span<byte> dest, ZlibCompressionLevel compressionLevel, out uint adler32)
    {
        ZStream stream;
        var streamPtr = &stream;

        //We skipped initialization
        streamPtr->zalloc = null;
        streamPtr->zfree = null;
        fixed (byte* sourcePtr = source)
        fixed (byte* destPtr = dest)
        {
            InitializeDeflate(streamPtr, compressionLevel);
            streamPtr->next_in = sourcePtr;
            streamPtr->avail_in = (uint)source.Length;
            streamPtr->next_out = destPtr;
            streamPtr->avail_out = (uint)dest.Length;
            while (UnsafeNativeMethods.deflate(streamPtr, ZlibFlushStrategy.NoFlush) == ZlibPInvokeResult.Ok)
            {
                if (streamPtr->avail_in == 0)
                {
                    _ = UnsafeNativeMethods.deflate(streamPtr, ZlibFlushStrategy.Finish);
                }
            }

            adler32 = GetAdler32(streamPtr);
            DeflateEnd(streamPtr);
            return (uint)stream.total_out.Value;
        }
    }

    //Decompress returns avail_in, allowing users to reallocate and continue decompressing remaining data
    //should Dest buffer be under-allocated
    internal static uint Decompress(ReadOnlySpan<byte> source, Span<byte> dest, out uint bytesWritten, out uint adler32)
    {
        ZStream stream;
        var streamPtr = &stream;

        // We skipped initialization
        streamPtr->zalloc = null;
        streamPtr->zfree = null;
        fixed (byte* sourcePtr = source)
        fixed (byte* destPtr = dest)
        {
            InitializeInflate(streamPtr);
            streamPtr->next_in = sourcePtr;
            streamPtr->avail_in = (uint)source.Length;
            streamPtr->next_out = destPtr;
            streamPtr->avail_out = (uint)dest.Length;
            while (UnsafeNativeMethods.inflate(streamPtr, ZlibFlushStrategy.NoFlush) == ZlibPInvokeResult.Ok)
            {
                if (streamPtr->avail_in == 0)
                {
                    _ = UnsafeNativeMethods.inflate(streamPtr, ZlibFlushStrategy.Finish);
                }
            }

            bytesWritten = (uint)streamPtr->total_out.Value;
            adler32 = GetAdler32(streamPtr);
            InflateEnd(streamPtr);
            return streamPtr->avail_in;
        }
    }

    private static void InitializeInflate(ZStream* streamPtr)
    {
        if (!zlibResolverAdded)
        {
            AddNativeResolver();
        }

        var result = UnsafeNativeMethods.inflateInit_(streamPtr, UnsafeNativeMethods.zlibVersion(), sizeof(ZStream));
        if (result != ZlibPInvokeResult.Ok)
        {
            throw new NotUnpackableException($"{nameof(InitializeInflate)} failed - ({result}) {Marshal.PtrToStringUTF8((nint)streamPtr->msg)}");        
        }
    }

    private static void InitializeDeflate(ZStream* streamPtr, ZlibCompressionLevel compressionLevel)
    {
        if (!zlibResolverAdded)
        {
            AddNativeResolver();
        }

        var result = UnsafeNativeMethods.deflateInit_(streamPtr, compressionLevel, UnsafeNativeMethods.zlibVersion(), sizeof(ZStream));
        if (result != ZlibPInvokeResult.Ok)
        {
            throw new NotPackableException($"{nameof(InitializeDeflate)} failed - ({result}) {Marshal.PtrToStringUTF8((nint)streamPtr->msg)}");        
        }
    }

    private static void InflateEnd(ZStream* streamPtr)
    {
        var result = UnsafeNativeMethods.inflateEnd(streamPtr);
        if (result != ZlibPInvokeResult.Ok)
        {
            throw new NotUnpackableException($"{nameof(InflateEnd)} failed - ({result}) {Marshal.PtrToStringUTF8((nint)streamPtr->msg)}");
        }
    }

    private static void DeflateEnd(ZStream* streamPtr)
    {
        var result = UnsafeNativeMethods.deflateEnd(streamPtr);
        if (result != ZlibPInvokeResult.Ok)
        {
            throw new NotPackableException($"{nameof(DeflateEnd)} failed - ({result}) {Marshal.PtrToStringUTF8((nint)streamPtr->msg)}");
        }
    }

    private static uint GetAdler32(ZStream* streamPtr)
        => (uint)(streamPtr->adler.Value & 0xffff);

    private static void AddNativeResolver()
    {
        NativeLibrary.SetDllImportResolver(typeof(ZlibHelper).Assembly,
            (name, assembly, path) =>
            {
                nint handle = IntPtr.Zero;

                // check if name is zlib and the operating system is not Windows.
                // Otherwise, fallback to default import resolver.
                if (name == "zlib" && !OperatingSystem.IsWindows())
                {
                    if (OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD())
                    {
                        // require zlib from "sudo apt install zlib1g" or
                        // "sudo apt install zlib1g-dev".
                        _ = NativeLibrary.TryLoad($"libz.so.{MemoryZlib.NativeZlibVersion}", assembly, path, out handle);
                    }
                    else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
                    {
                        // require homebrew zlib.
                        _ = NativeLibrary.TryLoad($"/usr/local/Cellar/zlib/{MemoryZlib.NativeZlibVersion}/lib/libz.{MemoryZlib.NativeZlibVersion}.dylib", out handle);
                    }
                    else
                    {
                        throw new NotSupportedException("Zlib is probably not supported on mobile devices.");
                    }
                }

                return handle;
            });
        zlibResolverAdded = true;
    }
}
