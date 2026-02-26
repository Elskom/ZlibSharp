// Copyright (c) 2021~2026, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp.Tests;

using FluentAssertions.Collections;
using FluentAssertions.Execution;
using System.Diagnostics.Contracts;

[ExcludeFromCodeCoverage]
public static class SpanAssertionExtensions
{
    /// <summary>
    /// Returns an <see cref="GenericCollectionAssertions{T}"/> object that can be used to assert the
    /// current <see cref="Span{T}"/>.
    /// </summary>
    [Pure]
    public static GenericCollectionAssertions<T> Should<T>([NotNull] this Span<T> actualValue)
        => new(actualValue.ToArray(), AssertionChain.GetOrCreate());

    /// <summary>
    /// Returns an <see cref="GenericCollectionAssertions{T}"/> object that can be used to assert the
    /// current <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    [Pure]
    public static GenericCollectionAssertions<T> Should<T>([NotNull] this ReadOnlySpan<T> actualValue)
        => new(actualValue.ToArray(), AssertionChain.GetOrCreate());

    /// <summary>
    /// Returns an <see cref="GenericCollectionAssertions{T}"/> object that can be used to assert the
    /// current <see cref="Memory{T}"/>.
    /// </summary>
    [Pure]
    public static GenericCollectionAssertions<T> Should<T>([NotNull] this Memory<T> actualValue)
        => new(actualValue.ToArray(), AssertionChain.GetOrCreate());

    /// <summary>
    /// Returns an <see cref="GenericCollectionAssertions{T}"/> object that can be used to assert the
    /// current <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    [Pure]
    public static GenericCollectionAssertions<T> Should<T>([NotNull] this ReadOnlyMemory<T> actualValue)
        => new(actualValue.ToArray(), AssertionChain.GetOrCreate());

    /// <summary>
    /// Expects the current collection to contain all the same elements in the same order as the collection identified by
    /// <paramref name="elements" />. Elements are compared using their <see cref="object.Equals(object)" /> method.
    /// </summary>
    /// <param name="elements">A params array with the expected elements.</param>
    public static AndConstraint<GenericCollectionAssertions<T>> Equal<T>(this GenericCollectionAssertions<T> assertions, Span<T> elements)
        => assertions.Equal(elements.ToArray());

    /// <summary>
    /// Expects the current collection to contain all the same elements in the same order as the collection identified by
    /// <paramref name="elements" />. Elements are compared using their <see cref="object.Equals(object)" /> method.
    /// </summary>
    /// <param name="elements">A params array with the expected elements.</param>
    public static AndConstraint<GenericCollectionAssertions<T>> Equal<T>(this GenericCollectionAssertions<T> assertions, ReadOnlySpan<T> elements)
        => assertions.Equal(elements.ToArray());

    /// <summary>
    /// Expects the current collection to contain all the same elements in the same order as the collection identified by
    /// <paramref name="elements" />. Elements are compared using their <see cref="object.Equals(object)" /> method.
    /// </summary>
    /// <param name="elements">A params array with the expected elements.</param>
    public static AndConstraint<GenericCollectionAssertions<T>> Equal<T>(this GenericCollectionAssertions<T> assertions, Memory<T> elements)
        => assertions.Equal(elements.ToArray());

    /// <summary>
    /// Expects the current collection to contain all the same elements in the same order as the collection identified by
    /// <paramref name="elements" />. Elements are compared using their <see cref="object.Equals(object)" /> method.
    /// </summary>
    /// <param name="elements">A params array with the expected elements.</param>
    public static AndConstraint<GenericCollectionAssertions<T>> Equal<T>(this GenericCollectionAssertions<T> assertions, ReadOnlyMemory<T> elements)
        => assertions.Equal(elements.ToArray());
}
