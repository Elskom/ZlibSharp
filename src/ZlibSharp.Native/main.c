#include <string.h>
#include <stdlib.h>
#include <stdio.h>
#include "main.h"

ZLIBSHARP_NATIVE_EXTERN int Compress(pcompress_decompress_args args)
{
  z_streamp stream = (z_streamp)malloc(sizeof(z_stream));
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
  stream->next_out = args->dest;
  stream->avail_out = args->dest_length;
  args->status = deflateInit2(stream, args->compressionLevel, Z_DEFLATED, args->windowBits, 8, args->strategy);
  if (args->status != Z_OK)
  {
    return Z_ERRNO;
  }

  do
  {
    args->status = deflate(stream, Z_NO_FLUSH);
    if (args->status != Z_OK && args->status != Z_STREAM_END)
    {
      break;
    }

    if (stream->avail_in == 0)
    {
      args->status = deflate(stream, Z_FINISH);
      if (args->status != Z_OK && args->status != Z_STREAM_END)
      {
        break;
      }
    }
  }
  while (args->status == Z_OK);

  deflateEnd(stream);
  args->bytesWritten = stream->total_out;
  free(stream);
  return args->status;
}

ZLIBSHARP_NATIVE_EXTERN uInt Decompress(pcompress_decompress_args args)
{
  z_streamp stream = (z_streamp)malloc(sizeof(z_stream));
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
  stream->next_out = args->dest;
  stream->avail_out = args->dest_length;
  args->status = inflateInit2(stream, args->windowBits);
  if (args->status != Z_OK)
  {
    return Z_ERRNO;
  }

  do
  {
    args->status = inflate(stream, Z_NO_FLUSH);
    if (args->status != Z_OK && args->status != Z_STREAM_END)
    {
      break;
    }

    // Hopefully this catches the dest buffer being undersized,
    // yet returns Z_OK status anyways.
    if (args->status == Z_OK && stream->avail_out == 0 && stream->avail_in > 0)
    {
      args->status = Z_BUF_ERROR;
      break;
    }

    if (stream->avail_in == 0)
    {
      args->status = inflate(stream, Z_FINISH);
      if (args->status != Z_OK && args->status != Z_STREAM_END)
      {
        break;
      }
    }
  }
  while (args->status == Z_OK && args->status != Z_STREAM_END);

  inflateEnd(stream);
  args->bytesWritten = stream->total_out;
  uInt result = stream->avail_in;
  free(stream);
  return result;
}
