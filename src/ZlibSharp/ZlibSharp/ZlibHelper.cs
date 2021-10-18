// Copyright (c) 2021, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

internal static class ZlibHelper
{
    internal static unsafe bool InitializeInflate(ref ZStream zs)
    {
        if (SafeNativeMethods.inflateInit_(
            ref zs,
            SafeNativeMethods.zlibVersion(),
            sizeof(ZStream)) != ZlibResult.Ok)
        {
            throw new NotUnpackableException("inflateInit failed while trying to decompress.");
        }

        return true;
    }

    internal static unsafe bool InitializeDeflate(ref ZStream zs, ZlibCompressionLevel compressionLevel)
    {
        if (SafeNativeMethods.deflateInit_(
            ref zs,
            compressionLevel,
            SafeNativeMethods.zlibVersion(),
            sizeof(ZStream)) != ZlibResult.Ok)
        {
            throw new NotPackableException("deflateInit failed while trying to compress.");
        }

        return true;
    }

    internal static bool FinalizeInflate(ref ZStream zs)
    {
        return SafeNativeMethods.inflateEnd(ref zs) == ZlibResult.Ok;
    }

    internal static bool FinalizeDeflate(ref ZStream zs)
    {
        return SafeNativeMethods.deflateEnd(ref zs) == ZlibResult.Ok;
    }

    internal static unsafe ZlibResult Decompress(ref ZStream zs, ref byte[] dest, ref byte[] source, ZlibFlushStrategy flush)
    {
        zs.next_in = (byte*)source[0];
        zs.avail_in = (uint)source.Length;
        zs.next_out = (byte*)dest[0];
        zs.avail_out = (uint)dest.Length;
        var ret = SafeNativeMethods.inflate(ref zs, flush);
        if ((ulong)dest.LongLength < zs.total_out)
        {
            // error?
        }

        if (ret != ZlibResult.StreamEnd)
        {
            throw new NotUnpackableException($"Exception during zlib decompression: ({ret}) {zs.msg->ToString()}");
        }

        return ret;
    }

    internal static unsafe bool Compress(ref ZStream zs, ref byte[] dest, ref byte[] source, ZlibFlushStrategy flush)
    {
        zs.next_in = (byte*)source[0];
        zs.avail_in = (uint)source.Length;
        zs.next_out = (byte*)dest[0];
        zs.avail_out = (uint)dest.Length;
        var ret = SafeNativeMethods.deflate(ref zs, flush);
        if ((ulong)dest.LongLength < zs.total_out)
        {
            // error?
        }

        if (ret is not ZlibResult.Ok and not ZlibResult.StreamEnd)
        {
            throw new NotPackableException($"deflating: ({ret}) {zs.msg->ToString()}");
        }

        return ret is ZlibResult.Ok or ZlibResult.StreamEnd;
    }
}
