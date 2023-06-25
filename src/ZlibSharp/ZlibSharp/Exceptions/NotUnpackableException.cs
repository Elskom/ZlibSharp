// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Exceptions;

/// <summary>
/// Zlib Memory Unpacking failure error.
/// </summary>
[ExcludeFromCodeCoverage]
[Serializable]
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

    /// <summary>
    /// Initializes a new instance of the <see cref="NotUnpackableException"/> class
    /// with the specified serialization and context information.
    /// </summary>
    /// <param name="info">The data for serializing or deserializing the object.</param>
    /// <param name="context">The source and destination for the object.</param>
    private NotUnpackableException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
