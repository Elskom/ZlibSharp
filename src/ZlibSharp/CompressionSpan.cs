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

    /// <summary>
    /// Creates a new instance of <see cref="CompressionSpan{T}"/> with the specified buffer and length.
    /// </summary>
    /// <param name="buffer">The input buffer to use.</param>
    /// <param name="length">The length of the buffer.</param>
    /// <returns></returns>
    [ExcludeFromCodeCoverage]
    public static CompressionSpan<T> Create(T[] buffer, ulong length)
        => new(buffer, length);

    /// <summary>
    /// Creates a new <see cref="CompressionSpan{T}"/> instance from the specified span.
    /// </summary>
    /// <param name="buffer">The span containing the data to be used for the compression span.</param>
    /// <returns>A <see cref="CompressionSpan{T}"/> that represents the data in the specified span.</returns>
    [ExcludeFromCodeCoverage]
    public static CompressionSpan<T> Create(Span<T> buffer)
        => Create(buffer.ToArray(), (ulong)buffer.Length);

    /// <summary>
    /// Creates a new <see cref="CompressionSpan{T}"/> instance from the specified read-only span.
    /// </summary>
    /// <param name="buffer">The read-only span containing the data to be used for the compression span.</param>
    /// <returns>A <see cref="CompressionSpan{T}"/> that represents the data in the specified read-only span.</returns>
    [ExcludeFromCodeCoverage]
    public static CompressionSpan<T> Create(ReadOnlySpan<T> buffer)
        => Create(buffer.ToArray(), (ulong)buffer.Length);

    /// <summary>
    /// Creates a new <see cref="CompressionSpan{T}"/> instance from the specified memory.
    /// </summary>
    /// <param name="buffer">The memory containing the data to be used for the compression span.</param>
    /// <returns>A <see cref="CompressionSpan{T}"/> that represents the data in the specified memory.</returns>
    [ExcludeFromCodeCoverage]
    public static CompressionSpan<T> Create(Memory<T> buffer)
        => Create(buffer.ToArray(), (ulong)buffer.Length);

    /// <summary>
    /// Creates a new <see cref="CompressionSpan{T}"/> instance from the specified read-only memory.
    /// </summary>
    /// <param name="buffer">The read-only memory containing the data to be used for the compression span.</param>
    /// <returns>A <see cref="CompressionSpan{T}"/> that represents the data in the specified read-only memory.</returns>
    [ExcludeFromCodeCoverage]
    public static CompressionSpan<T> Create(ReadOnlyMemory<T> buffer)
        => Create(buffer.ToArray(), (ulong)buffer.Length);
}
