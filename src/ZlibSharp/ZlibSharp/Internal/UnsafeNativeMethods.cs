// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Internal;

[ExcludeFromCodeCoverage]
internal static unsafe partial class UnsafeNativeMethods
{
    // [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    // [LibraryImport("zlib", EntryPoint = "zlibVersion")]
    // [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    // internal static partial byte* zlibVersion();

    // [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    // [LibraryImport("zlib", EntryPoint = "deflateInit2_")]
    // [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    // internal static partial ZlibStatus deflateInit2_(ZStream* zs, ZlibCompressionLevel compressionLevel, ZlibCompressionMethod method, ZlibWindowBits windowBits, int memLevel, ZlibCompressionStrategy strategy, byte *version, int streamSize);

    // [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    // [LibraryImport("zlib", EntryPoint = "inflateInit2_")]
    // [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    // internal static partial ZlibStatus inflateInit2_(ZStream* zs, ZlibWindowBits windowBits, byte* version, int streamSize);

    // [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    // [LibraryImport("zlib", EntryPoint = "inflate")]
    // [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    // internal static partial ZlibStatus inflate(ZStream* zs, ZlibFlushStrategy flush);

    // [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    // [LibraryImport("zlib", EntryPoint = "deflate")]
    // [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    // internal static partial ZlibStatus deflate(ZStream* zs, ZlibFlushStrategy flush);

    // [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    // [LibraryImport("zlib", EntryPoint = "inflateEnd")]
    // [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    // internal static partial ZlibStatus inflateEnd(ZStream* zs);

    // [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    // [LibraryImport("zlib", EntryPoint = "deflateEnd")]
    // [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    // internal static partial ZlibStatus deflateEnd(ZStream* zs);

    // [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    // [LibraryImport("zlib", EntryPoint = "adler32")]
    // [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    // internal static partial ulong adler32(ulong adler, byte* buf, uint len);

    // [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    // [LibraryImport("zlib", EntryPoint = "crc32")]
    // [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    // internal static partial ulong crc32(ulong crc, byte* buf, uint len);

    // private const DllImportSearchPath searchPaths
    //     = DllImportSearchPath.UseDllDirectoryForDependencies
    //     | DllImportSearchPath.ApplicationDirectory
    //     | DllImportSearchPath.System32
    //     | DllImportSearchPath.UserDirectories;

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [LibraryImport(ZlibHelper.ZlibLibFileName, EntryPoint = "Compress")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial uint Compress(CompressDecompressArgs* args);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [LibraryImport(ZlibHelper.ZlibLibFileName, EntryPoint = "Compress")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial uint Decompress(CompressDecompressArgs* args);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [LibraryImport(ZlibHelper.ZlibLibFileName, EntryPoint = "Compress")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial ulong Crc32_ComputeHash(byte* source);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [LibraryImport(ZlibHelper.ZlibLibFileName, EntryPoint = "Compress")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial ulong Adler32_ComputeHash(byte* source);
}
