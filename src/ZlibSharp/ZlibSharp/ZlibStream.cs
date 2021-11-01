// Copyright (c) 2021, Els_kom org.
// https://github.com/Elskom/
// All rights reserved.
// license: MIT, see LICENSE for more details.

namespace ZlibSharp;

public class ZlibStream : Stream
{
    private ZStream zs;
    private static bool zlibResolverAdded;

    public ZlibStream(Stream input)
        : this(input, false)
    {
    }

    public ZlibStream(Stream input, bool keepOpen)
    {
        this.Compress = false;
        this.BaseStream = input;
        this.KeepOpen = keepOpen;
        this.zs = new();
        if (!zlibResolverAdded)
        {
            AddNativeResolver();
        }

        // initialize inflater.
        if (ZlibHelper.InitializeInflate(ref this.zs))
        {
            throw new InvalidOperationException("zlib decompression initialization failed.");
        }
    }

    public ZlibStream(Stream output, ZlibCompressionLevel level)
        : this(output, level, false)
    {
    }

    public ZlibStream(Stream output, ZlibCompressionLevel level, bool keepOpen)
    {
        this.Compress = true;
        this.BaseStream = output;
        this.KeepOpen = keepOpen;
        this.zs = new();
        if (!zlibResolverAdded)
        {
            AddNativeResolver();
        }

        // initialize deflater.
        if (!ZlibHelper.InitializeDeflate(ref this.zs, level))
        {
            throw new InvalidOperationException("zlib compression initialization failed.");
        }
    }

    public override bool CanRead
        => !this.Compress;

    public override bool CanSeek
        => false;

    public override bool CanWrite
        => this.Compress;

    public override long Length
        => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    // default flush mode of no flush.
    /// <summary>
    /// The flush mode to use with zlib.
    /// </summary>
    public ZlibFlushStrategy FlushMode { get; set; } = ZlibFlushStrategy.NoFlush;

    /// <summary>
    /// Gets or sets the native zlib version to use.
    ///
    /// Default: version 1.2.11
    /// </summary>
    public static string ZlibVersion { get; set; } = "1.2.11";

    /// <summary>
    /// Gets a value indicating whether the stream is finished.
    /// </summary>
    public bool IsFinished { get; private set; }

    // 1024 * 10 = 10KB. This is to ensure faster compression/decompression times.
    public int BufferSize { get; set; } = 10240;

    internal Stream BaseStream { get; }

    private bool Compress { get; }

    private bool KeepOpen { get; }

    private bool IsDisposed { get; set; }

    public override void Flush()
        => this.BaseStream.Flush();

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (this.Compress)
        {
            throw new InvalidOperationException("Read can only be used when decompressing.");
        }

        if (!this.BaseStream.CanRead)
        {
            throw new InvalidOperationException("Base stream must be able to be read to decompress with zlib.");
        }

        List<byte> bufList = new(count);
        var cnt = count;
        var read = 0;
        do
        {
            var bufBytes = new byte[this.BufferSize];
            read += this.BaseStream.Read(
                bufBytes,
                offset + read,
                this.BufferSize);
            cnt -= this.BufferSize;
            bufList.AddRange(bufBytes);
        }
        while (cnt > this.BufferSize);
        var buf = bufList.ToArray();
        _ = ZlibHelper.Decompress(ref this.zs, buffer, buf, FlushMode);
        return read;
    }

    public override long Seek(long offset, SeekOrigin origin)
        => 0;

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (!this.Compress)
        {
            throw new InvalidOperationException("Write can only be used when compressing.");
        }

        if (!this.BaseStream.CanWrite)
        {
            throw new InvalidOperationException("Base stream must be able to be written into to be read to compress with zlib.");
        }

        // avoid allocating a needless span and then another array when offset is 0.
        var buf = offset > 0 ? buffer.AsSpan(offset, count).ToArray() : buffer;
        var dest = new byte[count];
        _ = ZlibHelper.Compress(ref this.zs, dest, buf, FlushMode);
        this.BaseStream.Write(dest);
    }

    public void Finish()
    {
        var result = this.Compress ? ZlibHelper.FinalizeDeflate(ref this.zs) : ZlibHelper.FinalizeInflate(ref this.zs);
        if (!result)
        {
            throw new InvalidOperationException($"finishing zlib {(this.Compress ? string.Empty : "de")}compression failed.");
        }

        this.IsFinished = true;
    }

    /// <summary>
    /// Gets the Adler32 hash of the original data.
    /// </summary>
    /// <returns>The Adler32 hash of the original data.</returns>
    public CULong GetAdler32()
        => this.zs.adler;

    protected override void Dispose(bool disposing)
    {
        if (disposing && !this.IsDisposed)
        {
            if (this.KeepOpen)
            {
                this.BaseStream.Dispose();
            }

            if (!this.IsFinished)
            {
                // we need to tell zlib that we are done.
                try
                {
                    this.Finish();
                }
                catch
                {
                    // should never throw.
                }
            }

            this.IsDisposed = true;
        }

        base.Dispose(disposing);
    }

    private static void AddNativeResolver()
    {
        NativeLibrary.SetDllImportResolver(typeof(ZlibStream).Assembly,
            (name, assembly, path) =>
            {
                nint handle;
                // check if name is zlib and the operating system is not Windows.
                // Otherwise, fallback to default import resolver.
                if (name == "zlib" && !OperatingSystem.IsWindows())
                {
                    if (OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD())
                    {
                        // require zlib from "sudo apt install zlib1g" or
                        // "sudo apt install zlib1g-dev".
                        _ = NativeLibrary.TryLoad($"libz.so.{ZlibVersion}", assembly, path, out handle);
                    }
                    else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
                    {
                        // require homebrew zlib.
                        _ = NativeLibrary.TryLoad($"/usr/local/Cellar/zlib/{ZlibVersion}/lib/libz.{ZlibVersion}.dylib", out handle);
                    }
                    else
                    {
                        throw new NotSupportedException("Zlib is probably not supported on mobile devices.");
                    }
                }

                return handle;
            });
        zlibResolverAdded = true;
    }
}
