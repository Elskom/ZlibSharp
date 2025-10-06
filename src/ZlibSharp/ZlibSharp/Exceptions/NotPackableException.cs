// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Exceptions;

/// <summary>
/// Zlib Memory Packing failure error.
/// </summary>
[ExcludeFromCodeCoverage]
[Serializable]
public sealed class NotPackableException : IOException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotPackableException"/> class.
    /// </summary>
    /// <param name="s">The error string.</param>
    public NotPackableException(string s)
        : base(s)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotPackableException"/> class.
    /// </summary>
    /// <param name="s">The error string.</param>
    /// <param name="ex">The Exception that caused this Exception.</param>
    public NotPackableException(string s, Exception ex)
        : base(s, ex)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotPackableException"/> class
    /// with the specified serialization and context information.
    /// </summary>
    /// <param name="info">The data for serializing or deserializing the object.</param>
    /// <param name="context">The source and destination for the object.</param>
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    private NotPackableException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
