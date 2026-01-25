// Copyright (c) 2021~2026, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

[assembly: DefaultDllImportSearchPaths(
    DllImportSearchPath.ApplicationDirectory
    | DllImportSearchPath.System32
    | DllImportSearchPath.UserDirectories)]

namespace ZlibSharp.Internal;

[ExcludeFromCodeCoverage]
internal static unsafe partial class UnsafeNativeMethods
{
    private static bool zlibResolverAdded = false;

    internal const string ZlibLibFileName = "ZlibSharp.Native";

    internal static string ZlibLibFileNameByCurrentOS
        => (OperatingSystem.IsWindows(), OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD() || OperatingSystem.IsAndroid(), OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst(), OperatingSystem.IsIOS() && !OperatingSystem.IsMacCatalyst()) switch
    {
        (true, false, false, false) => $"{ZlibLibFileName}.dll",
        (false, true, false, false) => $"lib{ZlibLibFileName}.so",
        (false, false, true, false) => $"lib{ZlibLibFileName}.dylib",
        (false, false, false, true) => "__Internal",
        _ => throw new PlatformNotSupportedException("Zlib is probably not supported on this platform."),
    };

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

    [LibraryImport(ZlibLibFileName, EntryPoint = "Compress")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial int Compress(CompressDecompressArgs* args);

    [LibraryImport(ZlibLibFileName, EntryPoint = "Decompress")]
    [UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)])]
    internal static partial uint Decompress(CompressDecompressArgs* args);

    // P/Invoke for AddDllDirectory
    [SupportedOSPlatform("windows")]
    [LibraryImport("kernel32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint AddDllDirectory(string newDirectory);

    internal static void AddNativeResolver()
    {
        if (!zlibResolverAdded.Equals(true))
        {
            var libraryDirectory = Path.Combine(AppContext.BaseDirectory, "runtimes", RuntimeInformation.RuntimeIdentifier, "native");

            // On Windows, add the native library directory to the DLL search path but only if the library directory exists.
            if (OperatingSystem.IsWindows() && Path.Exists(libraryDirectory))
            {
                _ = AddDllDirectory(libraryDirectory);
            }

            NativeLibrary.SetDllImportResolver(typeof(UnsafeNativeMethods).Assembly,
                (name, assembly, path) =>
                {
                    var handle = nint.Zero;

                    // check if name is zlib otherwise, fallback to default import resolver.
                    if (name == ZlibLibFileName)
                    {
                        // Try to load the custom native library first.
                        // This first attempt in some OS's allows to properly handle self-contained deployments of .NET applications.
                        // If the application is published as framework-dependent, then the native library can be loaded from the runtimes folder within the application base directory.
                        if (!NativeLibrary.TryLoad(ZlibLibFileNameByCurrentOS, assembly, path, out handle))
                        {
                            if (OperatingSystem.IsWindows() || (OperatingSystem.IsIOS() && !OperatingSystem.IsMacCatalyst()))
                            {
                                throw new DllNotFoundException($"Unable to load DLL '{ZlibLibFileNameByCurrentOS}' or one of its dependencies: The specified module could not be found. (0x8007007E)");
                            }

                            var fullPath = Path.Combine(libraryDirectory, ZlibLibFileNameByCurrentOS);
                            if (!NativeLibrary.TryLoad(fullPath, assembly, path, out handle))
                            {
                                throw new DllNotFoundException($"Unable to load shared library '{fullPath}' or one of its dependencies: The specified module could not be found. (0x7E)");
                            }
                        }
                    }

                    return handle;
                });
            zlibResolverAdded = true;
        }
    }
}
