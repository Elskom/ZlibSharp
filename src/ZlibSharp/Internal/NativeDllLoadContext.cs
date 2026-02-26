// Copyright (c) 2021~2026, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Internal;

internal sealed class NativeDllLoadContext : AssemblyLoadContext
{
    public NativeDllLoadContext()
        : base()
    {
    }

    [ExcludeFromCodeCoverage]
    protected override Assembly Load(AssemblyName assemblyName)
    {
        if (IsLoadedToDefaultContext(assemblyName.FullName))
        {
            // return the assembly from the default context instead of reloading it (is same assembly and version).
            return GetFromDefaultContext(assemblyName.FullName)!;
        }

        return this.LoadFromAssemblyPath($"{AppContext.BaseDirectory}{assemblyName.Name}.dll");
    }

    [ExcludeFromCodeCoverage]
    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        var libraryPath = GetNativeDllPath(unmanagedDllName);
        return (libraryPath is not null, !File.Exists($"{AppContext.BaseDirectory}{unmanagedDllName}")) switch
        {
            (false, true) => IntPtr.Zero,
            (false, false) => this.LoadUnmanagedDllFromPath($"{AppContext.BaseDirectory}{unmanagedDllName}"),
            _ => this.LoadUnmanagedDllFromPath(libraryPath!),
        };
    }

    internal nint LoadNativeDll(string nativeDllFile)
        => this.LoadUnmanagedDll(nativeDllFile);

    [ExcludeFromCodeCoverage]
    private static string GetNativeDllPath(string dllFileName)
    {
        var result = Path.Combine(AppContext.BaseDirectory, "runtimes", OSHelpers.RuntimeIdentifier, "native", dllFileName);
        return !File.Exists(result) ? null! : result;
    }

    [ExcludeFromCodeCoverage]
    private static bool IsLoadedToDefaultContext(string assemblyFullName)
    {
        var result = false;
#if NET
        foreach (var assembly in Default.Assemblies)
#else
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
#endif
        {
            if (assembly.FullName is not null && assembly.FullName.Equals(assemblyFullName, StringComparison.Ordinal))
            {
                result = true;
                break;
            }
        }

        return result;
    }

    [ExcludeFromCodeCoverage]
    private static Assembly GetFromDefaultContext(string assemblyFullName)
    {
        Assembly result = null!;
#if NET
        foreach (var assembly in Default.Assemblies)
#else
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
#endif
        {
            if (assembly.FullName is not null && assembly.FullName.Equals(assemblyFullName, StringComparison.Ordinal))
            {
                result = assembly;
                break;
            }
        }

        return result;
    }
}
