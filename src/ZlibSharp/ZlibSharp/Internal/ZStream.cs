// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Internal;

[ExcludeFromCodeCoverage]
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct ZStream
{
    internal byte* next_in;
    internal uint avail_in;
    internal CULong total_in;

    internal byte* next_out;
    internal uint avail_out;
    internal CULong total_out;
    internal sbyte* msg;
    private readonly internal_state* state; // not visible by applications.

    internal delegate* unmanaged[Cdecl]<void*, uint, uint, void*> zalloc;
    internal delegate* unmanaged[Cdecl]<void*, void*> zfree;
    internal void* opaque;

    internal int data_type;

    internal CULong adler;
    internal CULong reserved; // reserved for future use in zlib.
}
