// Copyright (c) 2021, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

public enum ZlibFlushStrategy
{
    NoFlush,
    PartialFlush,
    SyncFlush,
    FullFlush,
    Finish,
    Block,
    Trees,
}
