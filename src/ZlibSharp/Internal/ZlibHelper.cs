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
        UnsafeNativeMethods.AddNativeResolver();
        CompressDecompressArgs args = default;
        var argsPtr = &args;

        // ZStream stream = default;
        // var streamPtr = &stream;

        //We skipped initialization
        // streamPtr->zalloc = null;
        // streamPtr->zfree = null;
        // streamPtr->opaque = null;
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
            return argsPtr->bytesWritten.Value.ToUInt64();

            // streamPtr->next_in = sourcePtr;
            // streamPtr->avail_in = (uint)source.Length;
            // streamPtr->next_out = destPtr;
            // streamPtr->avail_out = (uint)dest.Length;
            // status = InitializeDeflate(streamPtr, compressionLevel, windowBits, strategy);
            // while ((status = UnsafeNativeMethods.deflate(streamPtr, ZlibFlushStrategy.NoFlush)) == ZlibStatus.Ok)
            // {
            //     if (streamPtr->avail_in == 0)
            //     {
            //         status = UnsafeNativeMethods.deflate(streamPtr, ZlibFlushStrategy.Finish);
            //     }
            // }

            // status = DeflateEnd(streamPtr);
            // return (uint)stream.total_out.Value;
        }
    }

    //Decompress returns avail_in, allowing users to reallocate and continue decompressing remaining data
    //should Dest buffer be under-allocated
    internal static uint Decompress(ReadOnlySpan<byte> source, Span<byte> dest, out ulong bytesWritten, out ZlibStatus status, ZlibWindowBits windowBits)
    {
        UnsafeNativeMethods.AddNativeResolver();
        CompressDecompressArgs args = default;
        var argsPtr = &args;

        // ZStream stream;
        // var streamPtr = &stream;

        // We skipped initialization
        // streamPtr->zalloc = null;
        // streamPtr->zfree = null;
        // streamPtr->opaque = null;
        fixed (byte* sourcePtr = source)
        fixed (byte* destPtr = dest)
        {
            argsPtr->source = sourcePtr;
            argsPtr->source_length = (uint)source.Length;
            argsPtr->dest = destPtr;
            argsPtr->dest_length = (uint)dest.Length;
            argsPtr->windowBits = windowBits;
            var result = UnsafeNativeMethods.Decompress(argsPtr);
            bytesWritten = argsPtr->bytesWritten.Value.ToUInt64();
            status = argsPtr->status;
            return result;

            // streamPtr->next_in = sourcePtr;
            // streamPtr->avail_in = (uint)source.Length;
            // streamPtr->next_out = destPtr;
            // streamPtr->avail_out = (uint)dest.Length;
            // status = InitializeInflate(streamPtr, windowBits);
            // while ((status = Inflate(streamPtr, ZlibFlushStrategy.NoFlush)) == ZlibStatus.Ok)
            // {
            //     if (streamPtr->avail_in == 0)
            //     {
            //         status = Inflate(streamPtr, ZlibFlushStrategy.Finish);
            //     }
            // }

            // bytesWritten = (uint)streamPtr->total_out.Value;
            // status = InflateEnd(streamPtr);
            // return streamPtr->avail_in;
        }
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // internal static ulong GetAdler32(ReadOnlySpan<byte> data)
    // {
    //     fixed (byte* dataPtr = data)
    //     {
    //         return UnsafeNativeMethods.adler32(
    //             UnsafeNativeMethods.adler32(0L, null, 0),
    //             dataPtr,
    //             (uint)data.Length);
    //     }
    // }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // internal static ulong GetCrc32(ReadOnlySpan<byte> data)
    // {
    //     fixed (byte* dataPtr = data)
    //     {
    //         return UnsafeNativeMethods.crc32(
    //             UnsafeNativeMethods.crc32(0L, null, 0),
    //             dataPtr,
    //             (uint)data.Length);
    //     }
    // }

    // private static void PreOperationCheck()
    // {
    //     if (!zlibResolverAdded)
    //     {
    //         AddNativeResolver();
    //     }
    // 
    //     if (!ZlibVersion().Equals(NativeZlibVersion))
    //     {
    //         UnsafeNativeMethods.ThrowInvalidOperationException();
    //     }
    // }

    // private static string ZlibVersion()
    //     => Marshal.PtrToStringUTF8((nint)UnsafeNativeMethods.zlibVersion()) ?? string.Empty;

    // private static ZlibStatus InitializeInflate(ZStream* streamPtr, ZlibWindowBits windowBits)
    // {
    //     var result = UnsafeNativeMethods.inflateInit2_(streamPtr, windowBits, UnsafeNativeMethods.zlibVersion(), sizeof(ZStream));
    //     return result is not ZlibStatus.Ok
    //         ? throw new NotUnpackableException($"{nameof(InitializeInflate)} failed - ({result}) {Marshal.PtrToStringUTF8((nint)streamPtr->msg)}")
    //         : result;
    // }

    // private static ZlibStatus InitializeDeflate(ZStream* streamPtr, ZlibCompressionLevel compressionLevel, ZlibWindowBits windowBits, ZlibCompressionStrategy strategy)
    // {
    //     var result = UnsafeNativeMethods.deflateInit2_(streamPtr, compressionLevel, ZlibCompressionMethod.Deflate, windowBits, 8, strategy, UnsafeNativeMethods.zlibVersion(), sizeof(ZStream));
    //     return result is not ZlibStatus.Ok
    //         ? throw new NotPackableException($"{nameof(InitializeDeflate)} failed - ({result}) {Marshal.PtrToStringUTF8((nint)streamPtr->msg)}")
    //         : result;
    // }

    // private static ZlibStatus Deflate(ZStream* streamPtr, ZlibFlushStrategy flush)
    // {
    //     var result = UnsafeNativeMethods.deflate(streamPtr, flush);
    //     return result is not ZlibStatus.Ok and not ZlibStatus.StreamEnd
    //         ? throw new NotPackableException($"{nameof(Deflate)} failed - ({result}) {Marshal.PtrToStringUTF8((nint)streamPtr->msg)}")
    //         : result;
    // }

    // private static ZlibStatus Inflate(ZStream* streamPtr, ZlibFlushStrategy flush)
    // {
    //     var result = UnsafeNativeMethods.inflate(streamPtr, flush);
    //     return result is not ZlibStatus.Ok and not ZlibStatus.StreamEnd
    //         ? throw new NotUnpackableException($"{nameof(Inflate)} failed - ({result}) {Marshal.PtrToStringUTF8((nint)streamPtr->msg)}")
    //         : result;
    // }

    // private static ZlibStatus InflateEnd(ZStream* streamPtr)
    // {
    //     var result = UnsafeNativeMethods.inflateEnd(streamPtr);
    //     return result is not ZlibStatus.Ok
    //         ? throw new NotUnpackableException($"{nameof(InflateEnd)} failed - ({result}) {Marshal.PtrToStringUTF8((nint)streamPtr->msg)}")
    //         : result;
    // }

    // private static ZlibStatus DeflateEnd(ZStream* streamPtr)
    // {
    //     var result = UnsafeNativeMethods.deflateEnd(streamPtr);
    //     return result is not ZlibStatus.Ok
    //         ? throw new NotPackableException($"{nameof(DeflateEnd)} failed - ({result}) {Marshal.PtrToStringUTF8((nint)streamPtr->msg)}")
    //         : result;
    // }

    // private static void AddNativeResolver()
    // {
    //     if (cookie == nint.Zero)
    //     {
    //         var libraryDirectory = $"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}runtimes{Path.DirectorySeparatorChar}{RuntimeInformation.RuntimeIdentifier}{Path.DirectorySeparatorChar}native{Path.DirectorySeparatorChar}";
    //         cookie = UnsafeNativeMethods.AddDllDirectory(libraryDirectory);
    //         Console.WriteLine(libraryDirectory);
    //     }
    // 
    //     NativeLibrary.SetDllImportResolver(typeof(UnsafeNativeMethods).Assembly,
    //         (name, assembly, path) =>
    //         {
    //             var handle = nint.Zero;
    // 
    //             // check if name is zlib otherwise, fallback to default import resolver.
    //             if (name == "zlib")
    //             {
    //                 // Try to load the custom native library first.
    //                 Debug.WriteLine($"Attempting to load native library with search path: {path}...");
    //                 if (!NativeLibrary.TryLoad(ZlibLibFileName, assembly, path, out handle))
    //                 {
    //                     throw new DllNotFoundException($"Unable to load DLL '{ZlibLibFileName}' or one of its dependencies: The specified module could not be found. (0x8007007E)");
    //                 }
                    /*
                    if (OperatingSystem.IsWindows())
                    {
                        if (!NativeLibrary.TryLoad("zlibwapi.dll", assembly, path, out handle))
                        {
                            UnsafeNativeMethods.ThrowInvalidOperationException();
                        }
                    }
                    else if (OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD() || OperatingSystem.IsAndroid())
                    {
                        if (
                            !NativeLibrary.TryLoad("libz.so", assembly, path, out handle)
                            // pick up zlib from "sudo apt install zlib1g" or
                            // "sudo apt install zlib1g-dev".
                            && !NativeLibrary.TryLoad($"libz.so.{NativeZlibVersion}", assembly, path, out handle))
                        {
                            UnsafeNativeMethods.ThrowInvalidOperationException();
                        }
                    }
                    else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
                    {
                        if (
                            !NativeLibrary.TryLoad("libz.dylib", assembly, path, out handle)
                            && !NativeLibrary.TryLoad($"libz.{NativeZlibVersion}.dylib", assembly, path, out handle)
                            // fall back on homebrew zlib.
                            && !NativeLibrary.TryLoad($"/usr/local/Cellar/zlib/{NativeZlibVersion}/lib/libz.{NativeZlibVersion}.dylib", out handle))
                        {
                            UnsafeNativeMethods.ThrowInvalidOperationException();
                        }
                    }
                    else if (OperatingSystem.IsIOS() && !OperatingSystem.IsMacCatalyst())
                    {
                        // iOS does not support dynamic libraries, so we need to link statically.
                        // And technically dotnet/runtime does bundle in zlib version 1.3.1 so it
                        // *should* already be statically linked in. The main issue with that is that
                        // it's symbols are not exported from System.IO.Compression.Native... If they
                        // were this resolver would have only checked for iOS and do this, and the
                        // other cases would have simply loaded System.IO.Compression.Native.
                        if (!NativeLibrary.TryLoad("__Internal", assembly, path, out handle))
                        {
                            UnsafeNativeMethods.ThrowInvalidOperationException();
                        }
                    }
                    else
                    {
                        throw new PlatformNotSupportedException("Zlib is probably not supported on this platform.");
                    }
                    */
    //             }
    // 
    //             return handle;
    //         });
    //     zlibResolverAdded = true;
    // }
}
