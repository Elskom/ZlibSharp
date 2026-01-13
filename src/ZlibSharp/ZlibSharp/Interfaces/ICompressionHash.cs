// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Interfaces;

/// <summary>
/// Interface for compression hash algorithms.
/// </summary>
/// <remarks>
/// Provides a way to provide user defined hash algorithms for the compression integrity checks.
/// The default implementation provides both of the Adler32 and Crc32 hash algorithms.
/// </remarks>
public interface ICompressionHash
{
    /// <summary>
    /// Computes a Hash of the data.
    /// </summary>
    /// <param name="source">The input data to hash.</param>
    /// <returns>The computed Hash.</returns>
    public uint ComputeHash(ReadOnlySpan<byte> source);
}
