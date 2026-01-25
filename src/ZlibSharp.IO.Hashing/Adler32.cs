[assembly: CLSCompliant(true)]

namespace ZlibSharp.IO.Hashing;

/// <summary>
/// Provides an implementation of the Adler-32 non-cryptographic hash algorithm for computing checksums over binary
/// data.
/// </summary>
/// <remarks>The Adler-32 algorithm is designed for fast, lightweight integrity checking and is commonly used in
/// data compression and transmission scenarios. This class is not suitable for cryptographic purposes. Instances of
/// this class are not thread-safe; concurrent access should be synchronized externally if required.</remarks>
public sealed partial class Adler32 : NonCryptographicHashAlgorithm
{
    private const uint InitialState = 1u;
    private const int Size = sizeof(uint);
    private uint _adler = InitialState;

    /// <summary>
    /// Initializes a new instance of the <see cref="Adler32"/> class.
    /// </summary>
    public Adler32()
        : base(Size)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Adler32"/> class using the state from another instance.
    /// </summary>
    private Adler32(uint adler)
        : base(Size)
        => _adler = adler;

    /// <summary>Returns a clone of the current instance, with a copy of the current instance's internal state.</summary>
    /// <returns>A new instance that will produce the same sequence of values as the current instance.</returns>
    public Adler32 Clone()
        => new(_adler);

    /// <summary>
    /// Appends the contents of <paramref name="source"/> to the data already
    /// processed for the current hash computation.
    /// </summary>
    /// <param name="source">The data to process.</param>
    public override void Append(ReadOnlySpan<byte> source)
        => _adler = Update(_adler, source);

    /// <summary>
    /// Resets the hash computation to the initial state.
    /// </summary>
    public override void Reset()
        => _adler = InitialState;

    /// <summary>
    /// Writes the computed hash value to <paramref name="destination"/>
    /// without modifying accumulated state.
    /// </summary>
    /// <param name="destination">The buffer that receives the computed hash value.</param>
    protected override void GetCurrentHashCore(Span<byte> destination)
        // The finalization step of the CRC is to perform the ones' complement.
        => BinaryPrimitives.WriteUInt32LittleEndian(destination, _adler);

    /// <summary>
    /// Writes the computed hash value to <paramref name="destination"/>
    /// then clears the accumulated state.
    /// </summary>
    protected override void GetHashAndResetCore(Span<byte> destination)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(destination, _adler);
        _adler = InitialState;
    }

    /// <summary>
    /// Gets the current computed hash value without modifying accumulated state.
    /// </summary>
    /// <returns>
    /// The hash value for the data already provided.
    /// </returns>
    [CLSCompliant(false)]
    public uint GetCurrentHashAsUInt32()
        => _adler;

    /// <summary>
    /// Computes the Adler-32 hash of the provided data.
    /// </summary>
    /// <param name="source">The data to hash.</param>
    /// <returns>The Adler-32 hash of the provided data.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="source"/> is <see langword="null"/>.
    /// </exception>
    public static byte[] Hash(byte[] source)
    {
#if NETSTANDARD2_0
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }
#else
        ArgumentNullException.ThrowIfNull(source);
#endif
        return Hash(new ReadOnlySpan<byte>(source));
    }

    /// <summary>
    /// Computes the Adler-32 hash of the provided data.
    /// </summary>
    /// <param name="source">The data to hash.</param>
    /// <returns>The Adler-32 hash of the provided data.</returns>
    public static byte[] Hash(ReadOnlySpan<byte> source)
    {
        var ret = new byte[Size];
        var hash = HashToUInt32(source);
        BinaryPrimitives.WriteUInt32LittleEndian(ret, hash);
        return ret;
    }

    /// <summary>
    /// Attempts to compute the Adler-32 hash of the provided data into the provided destination.
    /// </summary>
    /// <param name="source">The data to hash.</param>
    /// <param name="destination">The buffer that receives the computed hash value.</param>
    /// <param name="bytesWritten">
    /// On success, receives the number of bytes written to <paramref name="destination"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="destination"/> is long enough to receive
    /// the computed hash value (4 bytes); otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryHash(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesWritten)
    {
        if (destination.Length < Size)
        {
            bytesWritten = 0;
            return false;
        }

        var hash = HashToUInt32(source);
        BinaryPrimitives.WriteUInt32LittleEndian(destination, hash);
        bytesWritten = Size;
        return true;
    }

    /// <summary>
    /// Computes the Adler-32 hash of the provided data into the provided destination.
    /// </summary>
    /// <param name="source">The data to hash.</param>
    /// <param name="destination">The buffer that receives the computed hash value.</param>
    /// <returns>
    /// The number of bytes written to <paramref name="destination"/>.
    /// </returns>
    public static int Hash(ReadOnlySpan<byte> source, Span<byte> destination)
    {
        if (destination.Length < Size)
        {
            // throw new ArgumentException("The destination span is too short.");
            typeof(NonCryptographicHashAlgorithm).GetMethod("ThrowDestinationTooShort", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)?.Invoke(null, null);
            // ThrowDestinationTooShort();
        }

        var hash = HashToUInt32(source);
        BinaryPrimitives.WriteUInt32LittleEndian(destination, hash);
        return Size;
    }

    /// <summary>
    /// Computes the Adler-32 hash of the provided data.
    /// </summary>
    /// <param name="source">The data to hash.</param>
    /// <returns>
    /// The computed Adler-32 hash.
    /// </returns>
    [CLSCompliant(false)]
    public static uint HashToUInt32(ReadOnlySpan<byte> source)
        => Update(InitialState, source);

    private const uint Base = 65521; // largest prime smaller than 65536
    private const int NMax = 5552;   // NMax is the largest n such that 255n(n+1)/2 + (n+1)(BASE-1) <= 2^32-1

    private static uint Update(uint adler, ReadOnlySpan<byte> buf)
    {
        if (buf.IsEmpty)
        {
            return 1u;
        }

        var s1 = adler & 0xFFFF;
        var s2 = (adler >> 16) & 0xFFFF;
        while (buf.Length > 0)
        {
            var k = buf.Length < NMax ? buf.Length : NMax;
            for (var i = 0; i < k; i++)
            {
                s1 += buf[i];
                s2 += s1;
            }
            s1 %= Base;
            s2 %= Base;
#if NETSTANDARD2_0
            buf = buf.Slice(k);
#else
            buf = buf[k..];
#endif
        }

        return (s2 << 16) | s1;
    }
}
