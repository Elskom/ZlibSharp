// Copyright (c) 2021, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

internal static class SafeNativeMethods
{
    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern string zlibVersion();

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern ZlibResult deflateInit_(ref ZStream zs, ZlibCompressionLevel compressionLevel, string zlibVersion, int streamSize);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal static extern ZlibResult inflateInit_(ref ZStream zs, string zlibVersion, int streamSize);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZlibResult inflate(ref ZStream zs, ZlibFlushStrategy flush);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZlibResult deflate(ref ZStream zs, ZlibFlushStrategy flush);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZlibResult inflateEnd(ref ZStream zs);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl)]
    internal static extern ZlibResult deflateEnd(ref ZStream zs);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl)]
    internal static extern unsafe ulong adler32(ulong adler, byte* buf, uint len);
}
