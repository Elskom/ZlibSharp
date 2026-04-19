#pragma once

namespace ZlibSharp
{
  class zlib_utils
  {
  public:
    ZLIBSHARP_CONSTEXPR zlib_utils() = delete;

    typedef struct _compress_decompress_args {
      // shared args
      Byte *source;
      Byte *dest;
      uInt source_length;
      int windowBits;
      int status;
      // compress specific args
      int compressionLevel;
      int strategy;
      // decompress specific args
      std::uint64_t bytesWritten;
    } compress_decompress_args, *pcompress_decompress_args;

    static ZLIBSHARP_CONSTEXPR int compress(pcompress_decompress_args args) noexcept
    {
      chunk chunks;
      z_streamp stream = new z_stream();
      if (!stream)
      {
        args->status = Z_MEM_ERROR;
        return Z_ERRNO;
      }

      stream->zalloc = Z_NULL;
      stream->zfree = Z_NULL;
      stream->opaque = Z_NULL;
      stream->next_in = args->source;
      stream->avail_in = args->source_length;
      stream->next_out = Z_NULL;
      stream->avail_out = 0;
      args->status = deflateInit2(stream, args->compressionLevel, Z_DEFLATED, args->windowBits, 8, args->strategy);
      if (args->status != Z_OK)
      {
        return Z_ERRNO;
      }

      do
      {
        int flush = stream->avail_in > 0 ? Z_NO_FLUSH : Z_FINISH;
        if (stream->avail_out == 0)
        {
          stream->next_out = chunks.resize_and_get_data();
          stream->avail_out = chunk::chunk_size();
        }

        args->status = deflate(stream, flush);
        if (args->status != Z_OK && args->status != Z_STREAM_END)
        {
          break;
        }
      } while (args->status == Z_OK);

      args->bytesWritten = stream->total_out;
      deflateEnd(stream);
      args->dest = (Byte *)malloc(args->bytesWritten);
      if (!args->dest)
      {
        args->status = Z_MEM_ERROR;
        return Z_ERRNO;
      }

      memcpy(args->dest, chunks.data(), args->bytesWritten);
      chunks.clear();
      delete stream;
      return args->status;
    }

    static ZLIBSHARP_CONSTEXPR int decompress(pcompress_decompress_args args) noexcept
    {
      chunk chunks;
      z_streamp stream = new z_stream();
      if (!stream)
      {
        args->status = Z_MEM_ERROR;
        return Z_ERRNO;
      }

      stream->zalloc = Z_NULL;
      stream->zfree = Z_NULL;
      stream->opaque = Z_NULL;
      stream->next_in = args->source;
      stream->avail_in = args->source_length;
      stream->next_out = Z_NULL;
      stream->avail_out = 0;
      args->status = inflateInit2(stream, args->windowBits);
      if (args->status != Z_OK)
      {
        return Z_ERRNO;
      }

      do
      {
        int flush = stream->avail_in > 0 ? Z_NO_FLUSH : Z_FINISH;
        if (stream->avail_out == 0)
        {
          stream->next_out = chunks.resize_and_get_data();
          stream->avail_out = chunk::chunk_size();
        }

        args->status = inflate(stream, flush);
        if (args->status != Z_OK)
        {
          // Break on Stream End, but also break on any error.
          break;
        }
      } while (args->status == Z_OK);

      args->bytesWritten = stream->total_out;
      uInt result = stream->avail_in;
      inflateEnd(stream);
      args->dest = (Byte *)malloc(args->bytesWritten);
      if (!args->dest)
      {
        args->status = Z_MEM_ERROR;
        return Z_ERRNO;
      }

      memcpy(args->dest, chunks.data(), args->bytesWritten);
      chunks.clear();
      delete stream;
      return result;
    }
  };
}
