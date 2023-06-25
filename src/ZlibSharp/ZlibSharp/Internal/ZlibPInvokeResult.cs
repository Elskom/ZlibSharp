// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Internal;

internal enum ZlibPInvokeResult
{
    VersionError = -6,
    BufError,
    MemError,
    DataError,
    StreamError,
    ErrNo,
    Ok,
    StreamEnd,
    NeedDict,
}
