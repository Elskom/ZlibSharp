// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Internal;

[ExcludeFromCodeCoverage]
internal static class UnsafeNativeMethods
{
    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, EntryPoint = "zlibVersion")]
    private static extern unsafe byte* zlibVersion_private();

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, EntryPoint = "deflateInit2_")]
    private static extern unsafe ZlibStatus deflateInit2__private(ZStream* zs, ZlibCompressionLevel compressionLevel, ZlibCompressionMethod method, ZlibWindowBits windowBits, int memLevel, ZlibCompressionStrategy strategy, byte *version, int streamSize);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, EntryPoint = "inflateInit2_")]
    private static extern unsafe ZlibStatus inflateInit2__private(ZStream* zs, ZlibWindowBits windowBits, byte* version, int streamSize);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, EntryPoint = "inflate")]
    private static extern unsafe ZlibStatus inflate_private(ZStream* zs, ZlibFlushStrategy flush);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, EntryPoint = "deflate")]
    private static extern unsafe ZlibStatus deflate_private(ZStream* zs, ZlibFlushStrategy flush);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, EntryPoint = "inflateEnd")]
    private static extern unsafe ZlibStatus inflateEnd_private(ZStream* zs);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, EntryPoint = "deflateEnd")]
    private static extern unsafe ZlibStatus deflateEnd_private(ZStream* zs);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, EntryPoint = "adler32")]
    private static extern unsafe ulong adler32_private(ulong adler, byte* buf, uint len);

    [DefaultDllImportSearchPaths(DllImportSearchPath.LegacyBehavior)]
    [DllImport("zlib", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, EntryPoint = "crc32")]
    private static extern unsafe ulong crc32_private(ulong crc, byte* buf, uint len);

    private static void ThrowInvalidOperationException()
        => throw new InvalidOperationException($"Zlib version '{Checks.NativeZlibVersion}' not found. Please install the proper '{RuntimeInformation.ProcessArchitecture}' version and then try again.");

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "P/Invoke.")]
    internal static unsafe byte* zlibVersion()
    {
        try
        {
            return zlibVersion_private();
        }
        catch (DllNotFoundException)
        {
            ThrowInvalidOperationException();

            // never really called but it must be here to compile this.
            return (byte*)IntPtr.Zero;
        }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "P/Invoke.")]
    internal static unsafe ZlibStatus deflateInit2_(ZStream* zs, ZlibCompressionLevel compressionLevel, ZlibWindowBits windowBits, ZlibCompressionStrategy strategy)
    {
        try
        {
            return deflateInit2__private(zs, compressionLevel, ZlibCompressionMethod.Deflate, windowBits, 8, strategy, zlibVersion(), sizeof(ZStream));
        }
        catch (DllNotFoundException)
        {
            ThrowInvalidOperationException();

            // never really called but it must be here to compile this.
            return 0;
        }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "P/Invoke.")]
    internal static unsafe ZlibStatus inflateInit2_(ZStream* zs, ZlibWindowBits windowBits)
    {
        try
        {
            return inflateInit2__private(zs, windowBits, zlibVersion(), sizeof(ZStream));
        }
        catch (DllNotFoundException)
        {
            ThrowInvalidOperationException();

            // never really called but it must be here to compile this.
            return 0;
        }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "P/Invoke.")]
    internal static unsafe ZlibStatus inflate(ZStream* zs, ZlibFlushStrategy flush)
    {
        try
        {
            return inflate_private(zs, flush);
        }
        catch (DllNotFoundException)
        {
            ThrowInvalidOperationException();

            // never really called but it must be here to compile this.
            return 0;
        }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "P/Invoke.")]
    internal static unsafe ZlibStatus deflate(ZStream* zs, ZlibFlushStrategy flush)
    {
        try
        {
            return deflate_private(zs, flush);
        }
        catch (DllNotFoundException)
        {
            ThrowInvalidOperationException();

            // never really called but it must be here to compile this.
            return 0;
        }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "P/Invoke.")]
    internal static unsafe ZlibStatus inflateEnd(ZStream* zs)
    {
        try
        {
            return inflateEnd_private(zs);
        }
        catch (DllNotFoundException)
        {
            ThrowInvalidOperationException();

            // never really called but it must be here to compile this.
            return 0;
        }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "P/Invoke.")]
    internal static unsafe ZlibStatus deflateEnd(ZStream* zs)
    {
        try
        {
            return deflateEnd_private(zs);
        }
        catch (DllNotFoundException)
        {
            ThrowInvalidOperationException();

            // never really called but it must be here to compile this.
            return 0;
        }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "P/Invoke.")]
    internal static unsafe ulong adler32(ulong adler, byte* buf, uint len)
    {
        try
        {
            return adler32_private(adler, buf, len);
        }
        catch (DllNotFoundException)
        {
            ThrowInvalidOperationException();

            // never really called but it must be here to compile this.
            return 0;
        }
    }

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "P/Invoke.")]
    internal static unsafe ulong crc32(ulong crc, byte* buf, uint len)
    {
        try
        {
            return crc32_private(crc, buf, len);
        }
        catch (DllNotFoundException)
        {
            ThrowInvalidOperationException();

            // never really called but it must be here to compile this.
            return 0;
        }
    }
}
