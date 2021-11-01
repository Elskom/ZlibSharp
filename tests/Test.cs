using System.IO.Compression;
using System.Runtime.InteropServices;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace ZlibSharp.Tests;

public class Test
{
    private readonly ITestOutputHelper FakeConsole;
    
    private readonly byte[] SourceString, SourceStringCompressed, SourceBuffer;

    private readonly int LengthOfCompressed;
    
    public Test(ITestOutputHelper fakeConsole)
    {
        FakeConsole = fakeConsole;
        
        SourceString = File.ReadAllBytes("SourceText.txt");

        var DestBuffer = new byte[SourceString.Length];
        
        LengthOfCompressed = (int) MemoryZlib.Compress(SourceString, DestBuffer);

        SourceStringCompressed = new byte[LengthOfCompressed];
        
        DestBuffer.AsSpan(0, LengthOfCompressed).CopyTo(SourceStringCompressed);

        SourceBuffer = new byte[SourceString.Length];
    }
    
    [Fact]
    public void DecompressionWorks()
    {
        //FakeConsole.WriteLine($"Length {CompressLength}");

        MemoryZlib.Decompress(SourceStringCompressed, SourceBuffer).Should().Be(0);

        SourceBuffer.Should().Equal(SourceString);
    }
    
    [Fact]
    public void DecompressionToUnderAllocatedBufferReturnsNonZeroValue()
    {
        const int UndersizedBufferLength = 69;

        UndersizedBufferLength.Should().BeLessThan(SourceBuffer.Length);
        
        var UndersizedDestBuffer = new byte[UndersizedBufferLength];
        
        MemoryZlib.Decompress(SourceStringCompressed, UndersizedDestBuffer).Should().NotBe(0);
    }
    
    [Fact]
    public void CompressionToUnderAllocatedBufferReturnsNonZeroValue()
    {
        const int UndersizedBufferLength = 69;

        UndersizedBufferLength.Should().BeLessThan(LengthOfCompressed);
        
        var UndersizedDestBuffer = new byte[UndersizedBufferLength];
        
        MemoryZlib.Decompress(SourceStringCompressed, UndersizedDestBuffer).Should().NotBe(0);
    }
}