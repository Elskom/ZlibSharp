namespace ZlibSharp;

/// <summary>
/// This is a type of span where it owns the memory it refers to, and the GC can free the memory when it is no longer in use.
/// This is useful for when you want to return a span from a method, but you don't want to worry about the memory being freed while the span is still in use.
/// </summary>
public readonly struct CompressionSpan<T>
{
    private readonly T[] buffer;
    private readonly ulong bytesWritten;

    internal CompressionSpan(T[] buffer, ulong bytesWritten)
    {
        this.buffer = buffer;
        this.bytesWritten = bytesWritten;
    }

    /// <summary>
    /// Gets the total length of the data in this span.
    /// </summary>
    public ulong Length
        => this.bytesWritten;

    /// <summary>
    /// Converts this instance of <see cref="CompressionSpan{T}"/> to a <see cref="Span{T}"/>.
    /// </summary>
    /// <returns>The <see cref="Span{T}"/> instance of the data stored in this span.</returns>
    public Span<T> AsSpan()
        => this.buffer.AsSpan(0, (int)this.bytesWritten);

    /// <summary>
    /// Converts this instance of <see cref="CompressionSpan{T}"/> to a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <returns>The <see cref="ReadOnlySpan{T}"/> instance of the data stored in this span.</returns>
    public ReadOnlySpan<T> AsReadOnlySpan()
        => new ArraySegment<T>(this.buffer, 0, (int)this.bytesWritten);

    /// <summary>
    /// Converts this instance of <see cref="CompressionSpan{T}"/> to a <see cref="Memory{T}"/>.
    /// </summary>
    /// <returns>The <see cref="Memory{T}"/> instance of the data stored in this span.</returns>
    public Memory<T> AsMemory()
        => this.buffer.AsMemory(0, (int)this.bytesWritten);

    /// <summary>
    /// Converts this instance of <see cref="CompressionSpan{T}"/> to a <see cref="ReadOnlyMemory{T}"/>
    /// </summary>
    /// <returns>The <see cref="ReadOnlyMemory{T}"/> instance of the data stored in this span.</returns>
    public ReadOnlyMemory<T> AsReadOnlyMemory()
        => new ArraySegment<T>(this.buffer, 0, (int)this.bytesWritten);

    /// <summary>
    /// Converts this instance of <see cref="CompressionSpan{T}"/> to an array.
    /// This will return the entire buffer, not just the portion that is used.
    /// </summary>
    /// <returns>The array instance of the data stored in this span.</returns>
    public T[] AsArray()
        => this.buffer;

    [ExcludeFromCodeCoverage]
    internal unsafe static CompressionSpan<byte> Create(byte* buffer, ulong bytesWritten)
    {
        var tmp = new byte[Convert.ToInt32(bytesWritten)];
        fixed (byte* tmpPtr = tmp)
        {
            Buffer.MemoryCopy(buffer, tmpPtr, bytesWritten, bytesWritten);
        }

        return new(tmp, bytesWritten);
    }
}
