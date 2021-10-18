// Copyright (c) 2021, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

internal unsafe struct ZStream
{
    internal byte* next_in;
    internal uint avail_in;
    internal ulong total_in;

    internal byte* next_out;
    internal uint avail_out;
    internal ulong total_out;

    internal char* msg;

    private readonly IntPtr state; // not visible by applications.

    internal delegate* unmanaged[Cdecl] <void*, void*> zalloc;
    internal delegate* unmanaged[Cdecl] <void*, uint, uint> zfree;
    internal void* opaque;

    internal int data_type;

    internal ulong adler;
    internal ulong reserved; // reserved for future use in zlib.
}
