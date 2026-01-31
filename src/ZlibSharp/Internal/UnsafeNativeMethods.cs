// Copyright (c) 2021~2026, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]

namespace ZlibSharp.Internal;

[ExcludeFromCodeCoverage]
internal static unsafe partial class UnsafeNativeMethods
{
    internal const string ZlibLibFileName = "ZlibSharp.Native";
    internal const string ZlibLibFileNameInternal = "__Internal";

#if NET
    [LibraryImport(ZlibLibFileName, EntryPoint = "Compress")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int _Compress(CompressDecompressArgs* args);

    [LibraryImport(ZlibLibFileName, EntryPoint = "Decompress")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial uint _Decompress(CompressDecompressArgs* args);

    [LibraryImport(ZlibLibFileNameInternal, EntryPoint = "Compress")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int InternalCompress(CompressDecompressArgs* args);

    [LibraryImport(ZlibLibFileNameInternal, EntryPoint = "Decompress")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial uint InternalDecompress(CompressDecompressArgs* args);
#else
    [DllImport(ZlibLibFileName, EntryPoint = "Compress", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int _Compress(CompressDecompressArgs* args);
    [DllImport(ZlibLibFileName, EntryPoint = "Decompress", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint _Decompress(CompressDecompressArgs* args);

    [DllImport(ZlibLibFileNameInternal, EntryPoint = "Compress", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int InternalCompress(CompressDecompressArgs* args);

    [DllImport(ZlibLibFileNameInternal, EntryPoint = "Decompress", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint InternalDecompress(CompressDecompressArgs* args);
#endif

    internal static int Compress(CompressDecompressArgs* args)
        => OSHelpers.IsIOS() ? InternalCompress(args) : _Compress(args);

    internal static uint Decompress(CompressDecompressArgs* args)
        => OSHelpers.IsIOS() ? InternalDecompress(args) : _Decompress(args);
}
