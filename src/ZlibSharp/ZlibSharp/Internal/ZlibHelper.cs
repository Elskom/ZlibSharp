// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Internal;

using ZlibSharp.Exceptions;

[ExcludeFromCodeCoverage]
internal static unsafe class ZlibHelper
{
    private static bool zlibResolverAdded;

    internal static uint Compress(ReadOnlySpan<byte> source, Span<byte> dest, ZlibCompressionLevel compressionLevel, ZlibWindowBits windowBits, ZlibCompressionStrategy strategy, out uint adler32)
    {
        if (!zlibResolverAdded)
        {
            AddNativeResolver();
        }

        ZStream stream = default;
        var streamPtr = &stream;

        //We skipped initialization
        // streamPtr->zalloc = null;
        // streamPtr->zfree = null;
        // streamPtr->opaque = null;
        fixed (byte* sourcePtr = source)
        fixed (byte* destPtr = dest)
        {
            streamPtr->next_in = sourcePtr;
            streamPtr->avail_in = (uint)source.Length;
            streamPtr->next_out = destPtr;
            streamPtr->avail_out = (uint)dest.Length;
            InitializeDeflate(streamPtr, compressionLevel, windowBits, strategy);
            while (UnsafeNativeMethods.deflate(streamPtr, ZlibFlushStrategy.NoFlush) == ZlibPInvokeResult.Ok)
            {
                if (streamPtr->avail_in == 0)
                {
                    _ = UnsafeNativeMethods.deflate(streamPtr, ZlibFlushStrategy.Finish);
                }
            }

            adler32 = unchecked((uint)(MemoryZlib.ZlibGetAdler32(dest) & 0xFFFFFFFF));
            DeflateEnd(streamPtr);
            return (uint)stream.total_out.Value;
        }
    }

    //Decompress returns avail_in, allowing users to reallocate and continue decompressing remaining data
    //should Dest buffer be under-allocated
    internal static uint Decompress(ReadOnlySpan<byte> source, Span<byte> dest, out uint bytesWritten, out uint adler32, ZlibWindowBits windowBits)
    {
        if (!zlibResolverAdded)
        {
            AddNativeResolver();
        }

        ZStream stream;
        var streamPtr = &stream;

        // We skipped initialization
        streamPtr->zalloc = null;
        streamPtr->zfree = null;
        streamPtr->opaque = null;
        fixed (byte* sourcePtr = source)
        fixed (byte* destPtr = dest)
        {
            streamPtr->next_in = sourcePtr;
            streamPtr->avail_in = (uint)source.Length;
            streamPtr->next_out = destPtr;
            streamPtr->avail_out = (uint)dest.Length;
            InitializeInflate(streamPtr, windowBits);
            while (Inflate(streamPtr, ZlibFlushStrategy.NoFlush) == ZlibPInvokeResult.Ok)
            {
                if (streamPtr->avail_in == 0)
                {
                    _ = Inflate(streamPtr, ZlibFlushStrategy.Finish);
                }
            }

            bytesWritten = (uint)streamPtr->total_out.Value;
            adler32 = unchecked((uint)(MemoryZlib.ZlibGetAdler32(dest) & 0xFFFFFFFF));
            InflateEnd(streamPtr);
            return streamPtr->avail_in;
        }
    }

    private static void InitializeInflate(ZStream* streamPtr, ZlibWindowBits windowBits)
    {
        var result = UnsafeNativeMethods.inflateInit2_(streamPtr, windowBits);
        if (result is not ZlibPInvokeResult.Ok)
        {
            throw new NotUnpackableException($"{nameof(InitializeInflate)} failed - ({result}) {Marshal.PtrToStringUTF8((nint)streamPtr->msg)}");
        }
    }

    private static void InitializeDeflate(ZStream* streamPtr, ZlibCompressionLevel compressionLevel, ZlibWindowBits windowBits, ZlibCompressionStrategy strategy)
    {
        var result = UnsafeNativeMethods.deflateInit2_(streamPtr, compressionLevel, windowBits, strategy);
        if (result is not ZlibPInvokeResult.Ok)
        {
            throw new NotPackableException($"{nameof(InitializeDeflate)} failed - ({result}) {Marshal.PtrToStringUTF8((nint)streamPtr->msg)}");
        }
    }

    private static ZlibPInvokeResult Inflate(ZStream* streamPtr, ZlibFlushStrategy flush)
    {
        var result = UnsafeNativeMethods.inflate(streamPtr, flush);
        return result is not ZlibPInvokeResult.Ok and not ZlibPInvokeResult.StreamEnd
            ? throw new NotUnpackableException($"{nameof(Inflate)} failed - ({result}) {Marshal.PtrToStringUTF8((nint)streamPtr->msg)}")
            : result;
    }

    private static void InflateEnd(ZStream* streamPtr)
    {
        var result = UnsafeNativeMethods.inflateEnd(streamPtr);
        if (result is not ZlibPInvokeResult.Ok)
        {
            throw new NotUnpackableException($"{nameof(InflateEnd)} failed - ({result}) {Marshal.PtrToStringUTF8((nint)streamPtr->msg)}");
        }
    }

    private static void DeflateEnd(ZStream* streamPtr)
    {
        var result = UnsafeNativeMethods.deflateEnd(streamPtr);
        if (result is not ZlibPInvokeResult.Ok)
        {
            throw new NotPackableException($"{nameof(DeflateEnd)} failed - ({result}) {Marshal.PtrToStringUTF8((nint)streamPtr->msg)}");
        }
    }

    private static void AddNativeResolver()
    {
        NativeLibrary.SetDllImportResolver(typeof(ZlibHelper).Assembly,
            (name, assembly, path) =>
            {
                nint handle = IntPtr.Zero;

                // check if name is zlib otherwise, fallback to default import resolver.
                if (name == "zlib")
                {
                    if (OperatingSystem.IsWindows())
                    {
                        _ = NativeLibrary.TryLoad("zlibwapi.dll", assembly, path, out handle);
                    }
                    else if (OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD())
                    {
                        if (!NativeLibrary.TryLoad("libz.so", assembly, path, out handle))
                        {
                            // pick up zlib from "sudo apt install zlib1g" or
                            // "sudo apt install zlib1g-dev".
                            _ = NativeLibrary.TryLoad($"libz.so.{MemoryZlib.NativeZlibVersion}", assembly, path, out handle);
                        }
                    }
                    else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
                    {
                        if (!NativeLibrary.TryLoad("libz.dylib", assembly, path, out handle))
                        {
                            if (!NativeLibrary.TryLoad($"libz.{MemoryZlib.NativeZlibVersion}.dylib", assembly, path, out handle))
                            {
                                // fall back on homebrew zlib.
                                _ = NativeLibrary.TryLoad($"/usr/local/Cellar/zlib/{MemoryZlib.NativeZlibVersion}/lib/libz.{MemoryZlib.NativeZlibVersion}.dylib", out handle);
                            }
                        }
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
