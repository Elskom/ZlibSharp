// Copyright (c) 2021~2026, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]

namespace ZlibSharp.Internal;

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
    [LibraryImport(ZlibLibFileName, EntryPoint = "FreeOutput")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial uint _FreeOutput(byte* dest);

    [LibraryImport(ZlibLibFileNameInternal, EntryPoint = "Compress")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int InternalCompress(CompressDecompressArgs* args);
    [LibraryImport(ZlibLibFileNameInternal, EntryPoint = "Decompress")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial uint InternalDecompress(CompressDecompressArgs* args);
    [LibraryImport(ZlibLibFileNameInternal, EntryPoint = "FreeOutput")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial uint InternalFreeOutput(byte* dest);
#else
    [DllImport(ZlibLibFileName, EntryPoint = "Compress", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int _Compress(CompressDecompressArgs* args);
    [DllImport(ZlibLibFileName, EntryPoint = "Decompress", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint _Decompress(CompressDecompressArgs* args);
    [DllImport(ZlibLibFileName, EntryPoint = "FreeOutput", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint _FreeOutput(byte* dest);

    [DllImport(ZlibLibFileNameInternal, EntryPoint = "Compress", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int InternalCompress(CompressDecompressArgs* args);
    [DllImport(ZlibLibFileNameInternal, EntryPoint = "Decompress", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint InternalDecompress(CompressDecompressArgs* args);
    [DllImport(ZlibLibFileNameInternal, EntryPoint = "FreeOutput", CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint InternalFreeOutput(byte* dest);
#endif

    [ExcludeFromCodeCoverage]
    internal static int Compress(CompressDecompressArgs* args)
        => OSHelpers.IsIOS() ? InternalCompress(args) : _Compress(args);

    [ExcludeFromCodeCoverage]
    internal static uint Decompress(CompressDecompressArgs* args)
        => OSHelpers.IsIOS() ? InternalDecompress(args) : _Decompress(args);

    [ExcludeFromCodeCoverage]
    internal static uint FreeOutput(byte* dest)
        => OSHelpers.IsIOS() ? InternalFreeOutput(dest) : _FreeOutput(dest);
}
