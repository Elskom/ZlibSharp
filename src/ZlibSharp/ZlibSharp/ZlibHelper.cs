// Copyright (c) 2021, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

internal static class ZlibHelper
{
    internal static unsafe bool InitializeInflate(ref ZStream zs)
    {
        fixed (ZStream* pzs = &zs)
        {
            if (UnsafeNativeMethods.inflateInit_(
                pzs,
                UnsafeNativeMethods.zlibVersion(),
                sizeof(ZStream)) != ZlibResult.Ok)
            {
                throw new NotUnpackableException("inflateInit failed while trying to decompress.");
            }

            return true;
        }
    }

    internal static unsafe bool InitializeDeflate(ref ZStream zs, ZlibCompressionLevel compressionLevel)
    {
        fixed (ZStream* pzs = &zs)
        {
            if (UnsafeNativeMethods.deflateInit_(
                pzs,
                compressionLevel,
                UnsafeNativeMethods.zlibVersion(),
                sizeof(ZStream)) != ZlibResult.Ok)
            {
                throw new NotPackableException("deflateInit failed while trying to compress.");
            }

            return true;
        }
    }

    internal static unsafe bool FinalizeInflate(ref ZStream zs)
    {
        fixed (ZStream* pzs = &zs)
        {
            return UnsafeNativeMethods.inflateEnd(pzs) == ZlibResult.Ok;
        }
    }

    internal static unsafe bool FinalizeDeflate(ref ZStream zs)
    {
        fixed (ZStream* pzs = &zs)
        {
            return UnsafeNativeMethods.deflateEnd(pzs) == ZlibResult.Ok;
        }
    }

    internal static unsafe ZlibResult Decompress(ref ZStream zs, byte[] source, byte[] dest, ZlibFlushStrategy flush)
    {
        ZlibResult ret;
        fixed (byte* psource = source)
        fixed (byte* pdest = dest)
        fixed (ZStream* pzs = &zs)
        {
            zs.next_in = psource;
            zs.avail_in = (uint)source.Length;
            zs.next_out = pdest;
            zs.avail_out = (uint)dest.Length;
            ret = UnsafeNativeMethods.inflate(pzs, flush);
        }

        if ((ulong)dest.LongLength < zs.total_out.Value)
        {
            // error?
        }

        if (ret != ZlibResult.StreamEnd)
        {
            throw new NotUnpackableException($"Exception during zlib decompression: ({ret}) {Marshal.PtrToStringUTF8((nint)zs.msg)}");
        }

        return ret;
    }

    internal static unsafe bool Compress(ref ZStream zs, byte[] source, byte[] dest, ZlibFlushStrategy flush)
    {
        ZlibResult ret;
        fixed (byte* psource = source)
        fixed (byte* pdest = dest)
        fixed (ZStream* pzs = &zs)
        {
            zs.next_in = psource;
            zs.avail_in = (uint)source.Length;
            zs.next_out = pdest;
            zs.avail_out = (uint)dest.Length;
            ret = UnsafeNativeMethods.deflate(pzs, flush);
        }

        if ((ulong)dest.LongLength < zs.total_out.Value)
        {
            // error?
        }

        if (ret is not ZlibResult.Ok and not ZlibResult.StreamEnd)
        {
            throw new NotPackableException($"deflating: ({ret}) {Marshal.PtrToStringUTF8((nint)zs.msg)}");
        }

        return ret is ZlibResult.Ok or ZlibResult.StreamEnd;
    }
}
