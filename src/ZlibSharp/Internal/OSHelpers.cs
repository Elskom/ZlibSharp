// Copyright (c) 2021~2026, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Internal;

[ExcludeFromCodeCoverage]
internal static class OSHelpers
{
    internal static readonly Dictionary<string, OSPlatform> AdditionalOSPlatforms = new()
    {
        { "IOS", OSPlatform.Create("IOS") },
        { "MacCatalyst", OSPlatform.Create("MACCATALYST") },
        { "FreeBSD", OSPlatform.Create("FREEBSD") },
        { "Android", OSPlatform.Create("ANDROID") },
    };

    internal static bool nativeLibraryLoaded = false;
    internal static NativeDllLoadContext loadContext = new();

    internal static int MaxArrayLength =>
#if NET
        Array.MaxLength;
#else
        0X7FFFFFC7; // 2,147,483,591
#endif

    internal static string RuntimeIdentifier =>
            AppContext.GetData("RUNTIME_IDENTIFIER") as string ?? (Environment.OSVersion.Platform, RuntimeInformation.ProcessArchitecture) switch
            {
                (PlatformID.Win32NT, Architecture.X86) => "win-x86",
                (PlatformID.Win32NT, Architecture.X64) => "win-x64",
                (PlatformID.Win32NT, Architecture.Arm64) => "win-arm64",
                (PlatformID.Unix, Architecture.X86) => "linux-x86",
                (PlatformID.Unix, Architecture.X64) => "linux-x64",
                (PlatformID.Unix, Architecture.Arm64) => "linux-arm64",
                (PlatformID.MacOSX, Architecture.X64) => "osx-x64",
                (PlatformID.MacOSX, Architecture.Arm64) => "osx-arm64",
                _ => throw new InvalidOperationException("Platform not handled in the RuntimeIdentifier helper for netstandard2.0."),
            };

    internal static bool IsWindows() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    internal static bool IsLinux() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    internal static bool IsFreeBSD() =>
        RuntimeInformation.IsOSPlatform(AdditionalOSPlatforms["FreeBSD"]);

    internal static bool IsAndroid() =>
        RuntimeInformation.IsOSPlatform(AdditionalOSPlatforms["Android"]);

    internal static bool IsMacOS() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

    internal static bool IsMacCatalyst() =>
        RuntimeInformation.IsOSPlatform(AdditionalOSPlatforms["MacCatalyst"]);

    internal static bool IsIOS() =>
        RuntimeInformation.IsOSPlatform(AdditionalOSPlatforms["IOS"]) && !IsMacCatalyst();

    internal static string ZlibLibFileNameByCurrentOS
        => (IsWindows(), IsLinux() || IsFreeBSD() || IsAndroid(), IsMacOS() || IsMacCatalyst()) switch
    {
        (true, false, false) => $"{UnsafeNativeMethods.ZlibLibFileName}.dll",
        (false, true, false) => $"lib{UnsafeNativeMethods.ZlibLibFileName}.so",
        (false, false, true) => $"lib{UnsafeNativeMethods.ZlibLibFileName}.dylib",
        _ => throw new PlatformNotSupportedException("Zlib is probably not supported on this platform."),
    };

    internal static void LoadNativeLibrary()
    {
        if (!nativeLibraryLoaded.Equals(true))
        {
            // On all OS's except iOS, load the native library using the custom NativeDllLoadContext.
            if (!IsIOS())
            {
                _ = loadContext.LoadNativeDll(ZlibLibFileNameByCurrentOS);
            }

            nativeLibraryLoaded = true;
        }
    }
}
