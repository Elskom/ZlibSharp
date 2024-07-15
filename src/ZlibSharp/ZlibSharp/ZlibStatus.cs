// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

/// <summary>
/// The possible status codes from zlib.
/// </summary>
public enum ZlibStatus
{
    /// <summary>
    /// Version error.
    /// </summary>
    VersionError = -6,
    /// <summary>
    /// Buffer Error (usually under-allocated buffers).
    /// </summary>
    BufError,
    /// <summary>
    /// A memory error (out of memory).
    /// </summary>
    MemError,
    /// <summary>
    /// A data error (the data is possibly corrupted).
    /// </summary>
    DataError,
    /// <summary>
    /// A stream error.
    /// </summary>
    StreamError,
    /// <summary>
    /// Some other error.
    /// </summary>
    ErrNo,
    /// <summary>
    /// Everything is alright.
    /// </summary>
    Ok,
    /// <summary>
    /// End of stream.
    /// </summary>
    StreamEnd,
    /// <summary>
    /// Need dictionary.
    /// </summary>
    NeedDict,
}
