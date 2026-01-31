// Copyright (c) 2021~2022, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.


[assembly: FluentAssertions.Extensibility.AssertionEngineInitializer(
    typeof(ZlibSharp.Tests.AssertionEngineInitializer),
    nameof(ZlibSharp.Tests.AssertionEngineInitializer.AcknowledgeSoftWarning))]

namespace ZlibSharp.Tests;

[ExcludeFromCodeCoverage]
public static class AssertionEngineInitializer
{
    public static void AcknowledgeSoftWarning()
        => License.Accepted = true;
}
