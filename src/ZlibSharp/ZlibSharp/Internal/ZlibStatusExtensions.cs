namespace ZlibSharp.Internal;

using System.Buffers;

[ExcludeFromCodeCoverage]
internal static class ZlibStatusExtensions
{
    public static OperationStatus ToOperationStatus(this ZlibStatus status)
        => status switch
        {
            ZlibStatus.Ok or ZlibStatus.StreamEnd => OperationStatus.Done,
            ZlibStatus.BufError => OperationStatus.DestinationTooSmall,
            // ZlibStatus.DataError => OperationStatus.InvalidData,
            // ZlibStatus.MemError => OperationStatus.InvalidData,
            // ZlibStatus.StreamError => OperationStatus.InvalidData,
            // ZlibStatus.NeedDict => OperationStatus.InvalidData,
            // ZlibStatus.VersionError => throw new NotUnpackableException("zlib version mismatch!"),
            // ZlibStatus.ErrNo => OperationStatus.InvalidData,
            _ => OperationStatus.InvalidData,
        };
}
