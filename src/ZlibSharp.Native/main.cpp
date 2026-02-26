#include <string.h>
#include <stdlib.h>
#include <stdio.h>
#include <cstdint>
#include <utility>
#include <vector>
#include "main.h"

ZLIBSHARP_NATIVE_EXTERN int Compress(pcompress_decompress_args args)
{
  args->reserved = new internal_members();
  if (!args->reserved)
  {
    args->status = Z_MEM_ERROR;
    return Z_ERRNO;
  }

  args->reserved->stream = new z_stream();
  if (!args->reserved->stream)
  {
    args->status = Z_MEM_ERROR;
    return Z_ERRNO;
  }

  args->reserved->stream->zalloc = Z_NULL;
  args->reserved->stream->zfree = Z_NULL;
  args->reserved->stream->opaque = Z_NULL;
  args->reserved->stream->next_in = args->source;
  args->reserved->stream->avail_in = args->source_length;
  args->reserved->stream->next_out = Z_NULL;
  args->reserved->stream->avail_out = 0;
  args->status = deflateInit2(args->reserved->stream, args->compressionLevel, Z_DEFLATED, args->windowBits, 8, args->strategy);
  if (args->status != Z_OK)
  {
    return Z_ERRNO;
  }

  do
  {
    int flush = args->reserved->stream->avail_in > 0 ? Z_NO_FLUSH : Z_FINISH;
    if (args->reserved->stream->avail_out == 0)
    {
      size_t index = args->reserved->chunks.size();
      args->reserved->chunks.resize(index + CHUNK_SIZE); // Increase the size of the vector to accommodate a new chunk.
      args->reserved->stream->next_out = args->reserved->chunks.data() + index;
      args->reserved->stream->avail_out = CHUNK_SIZE;
    }

    args->status = deflate(args->reserved->stream, flush);
    if (args->status != Z_OK && args->status != Z_STREAM_END)
    {
      break;
    }

    // if (args->reserved->stream->avail_in == 0)
    // {
    //   args->status = deflate(args->reserved->stream, Z_FINISH);
    //   if (args->status != Z_OK && args->status != Z_STREAM_END)
    //   {
    //     break;
    //   }
    // }
  }
  while (args->status == Z_OK);

  args->bytesWritten = args->reserved->stream->total_out;
  deflateEnd(args->reserved->stream);
  args->dest = (Byte *)malloc(args->bytesWritten);
  if (!args->dest)
  {
    args->status = Z_MEM_ERROR;
    return Z_ERRNO;
  }

  memcpy(args->dest, args->reserved->chunks.data(), args->bytesWritten);
  args->reserved->chunks.clear();
  delete args->reserved->stream;
  delete args->reserved;
  return args->status;
}

ZLIBSHARP_NATIVE_EXTERN uInt Decompress(pcompress_decompress_args args)
{
  args->reserved = new internal_members();
  if (!args->reserved)
  {
    args->status = Z_MEM_ERROR;
    return Z_ERRNO;
  }

  args->reserved->stream = new z_stream();
  if (!args->reserved->stream)
  {
    args->status = Z_MEM_ERROR;
    return Z_ERRNO;
  }

  args->reserved->stream->zalloc = Z_NULL;
  args->reserved->stream->zfree = Z_NULL;
  args->reserved->stream->opaque = Z_NULL;
  args->reserved->stream->next_in = args->source;
  args->reserved->stream->avail_in = args->source_length;
  args->reserved->stream->next_out = Z_NULL;
  args->reserved->stream->avail_out = 0;
  args->status = inflateInit2(args->reserved->stream, args->windowBits);
  if (args->status != Z_OK)
  {
    return Z_ERRNO;
  }

  do
  {
    int flush = args->reserved->stream->avail_in > 0 ? Z_NO_FLUSH : Z_FINISH;
    if (args->reserved->stream->avail_out == 0)
    {
      size_t index = args->reserved->chunks.size();
      args->reserved->chunks.resize(index + CHUNK_SIZE); // Increase the size of the vector to accommodate a new chunk.
      args->reserved->stream->next_out = args->reserved->chunks.data() + index;
      args->reserved->stream->avail_out = CHUNK_SIZE;
    }

    args->status = inflate(args->reserved->stream, flush);
    if (args->status != Z_OK)
    {
      // Break on Stream End, but also break on any error.
      break;
    }

    // Hopefully this catches the dest buffer being undersized,
    // yet returns Z_OK status anyways.
    // The above comments for this commented out code is
    // outdated since switching to chunked decompression.
    // if (args->status == Z_OK && stream->avail_out == 0 && stream->avail_in > 0)
    // {
    //   args->status = Z_BUF_ERROR;
    //   break;
    // }

    // if (stream->avail_in == 0)
    // {
    //   args->status = inflate(stream, Z_FINISH);
    //   if (args->status != Z_OK && args->status != Z_STREAM_END)
    //   {
    //     break;
    //   }
    // }
  }
  while (args->status == Z_OK);

  // args->status = inflate(stream, Z_FINISH);
  // while (args->status == Z_OK && args->status != Z_STREAM_END);

  args->bytesWritten = args->reserved->stream->total_out;
  uInt result = args->reserved->stream->avail_in;
  inflateEnd(args->reserved->stream);
  args->dest = (Byte *)malloc(args->bytesWritten);
  if (!args->dest)
  {
    args->status = Z_MEM_ERROR;
    return Z_ERRNO;
  }

  memcpy(args->dest, args->reserved->chunks.data(), args->bytesWritten);
  args->reserved->chunks.clear();
  delete args->reserved->stream;
  delete args->reserved;
  return result;
}

ZLIBSHARP_NATIVE_EXTERN void FreeOutput(Byte *dest)
{
  free(dest);
}
