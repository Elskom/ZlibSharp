// Copyright (c) 2021, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

internal static class UnsafeNativeMethods
{
    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern unsafe byte* zlibVersion();

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern unsafe ZlibResult deflateInit_(ZStream* zs, ZlibCompressionLevel compressionLevel, byte* zlibVersion, int streamSize);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern unsafe ZlibResult inflateInit_(ZStream* zs, byte* zlibVersion, int streamSize);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern unsafe ZlibResult inflate(ZStream* zs, ZlibFlushStrategy flush);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern unsafe ZlibResult deflate(ZStream* zs, ZlibFlushStrategy flush);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern unsafe ZlibResult inflateEnd(ZStream* zs);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern unsafe ZlibResult deflateEnd(ZStream* zs);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern unsafe ulong adler32(ulong adler, byte* buf, uint len);
}
