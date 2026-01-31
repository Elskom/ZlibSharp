// Copyright (c) 2021~2026, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Exceptions;

/// <summary>
/// Zlib Memory Unpacking failure error.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class NotUnpackableException : IOException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotUnpackableException"/> class with an string argrument.
    /// </summary>
    /// <param name="s">The error string.</param>
    public NotUnpackableException(string s)
        : base(s)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotUnpackableException"/> class with an string argrument
    /// and the exception that cuased this exception.
    /// </summary>
    /// <param name="s">The error string.</param>
    /// <param name="ex">The Exception that caused this Exception.</param>
    public NotUnpackableException(string s, Exception ex)
        : base(s, ex)
    {
    }
}
