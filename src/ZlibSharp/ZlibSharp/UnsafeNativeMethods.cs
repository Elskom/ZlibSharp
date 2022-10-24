// Copyright (c) 2021~2022, Els_kom org.
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
    internal static extern unsafe ZlibPInvokeResult deflateInit_(ZStream* zs, ZlibCompressionLevel compressionLevel, byte* zlibVersion, int streamSize);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern unsafe ZlibPInvokeResult inflateInit_(ZStream* zs, byte* zlibVersion, int streamSize);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern unsafe ZlibPInvokeResult inflate(ZStream* zs, ZlibFlushStrategy flush);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern unsafe ZlibPInvokeResult deflate(ZStream* zs, ZlibFlushStrategy flush);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern unsafe ZlibPInvokeResult inflateEnd(ZStream* zs);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern unsafe ZlibPInvokeResult deflateEnd(ZStream* zs);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
    internal static extern unsafe ulong adler32(ulong adler, byte* buf, uint len);
}
