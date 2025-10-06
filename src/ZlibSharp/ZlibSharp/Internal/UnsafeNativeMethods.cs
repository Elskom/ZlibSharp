// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Internal;

[ExcludeFromCodeCoverage]
internal static unsafe partial class UnsafeNativeMethods
{
    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [LibraryImport("zlib", EntryPoint = "zlibVersion")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    internal static partial byte* zlibVersion();

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [LibraryImport("zlib", EntryPoint = "deflateInit2_")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    internal static partial ZlibStatus deflateInit2_(ZStream* zs, ZlibCompressionLevel compressionLevel, ZlibCompressionMethod method, ZlibWindowBits windowBits, int memLevel, ZlibCompressionStrategy strategy, byte *version, int streamSize);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [LibraryImport("zlib", EntryPoint = "inflateInit2_")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    internal static partial ZlibStatus inflateInit2_(ZStream* zs, ZlibWindowBits windowBits, byte* version, int streamSize);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [LibraryImport("zlib", EntryPoint = "inflate")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    internal static partial ZlibStatus inflate(ZStream* zs, ZlibFlushStrategy flush);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [LibraryImport("zlib", EntryPoint = "deflate")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    internal static partial ZlibStatus deflate(ZStream* zs, ZlibFlushStrategy flush);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [LibraryImport("zlib", EntryPoint = "inflateEnd")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    internal static partial ZlibStatus inflateEnd(ZStream* zs);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [LibraryImport("zlib", EntryPoint = "deflateEnd")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    internal static partial ZlibStatus deflateEnd(ZStream* zs);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [LibraryImport("zlib", EntryPoint = "adler32")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    internal static partial ulong adler32(ulong adler, byte* buf, uint len);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [LibraryImport("zlib", EntryPoint = "crc32")]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    internal static partial ulong crc32(ulong crc, byte* buf, uint len);

    internal static void ThrowInvalidOperationException()
        => throw new InvalidOperationException($"Zlib version '{ZlibHelper.NativeZlibVersion}' not found. Please install the proper '{RuntimeInformation.ProcessArchitecture}' version and then try again.");
}
