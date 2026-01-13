#include "main.h"
#include <string.h>
#include <stdlib.h>
#include <zlib.h>

ZLIBSHARP_NATIVE_EXTERN unsigned int Compress(pcompress_decompress_args args)
{
  z_streamp stream = (z_streamp)malloc(sizeof(z_stream));
  if (!stream)
  {
    args->status = Z_MEM_ERROR;
    return 0;
  }

  stream->zalloc = Z_NULL;
  stream->zfree = Z_NULL;
  stream->opaque = Z_NULL;
  stream->next_in = args->source;
  stream->avail_in = (uInt)strlen(args->source);
  stream->next_out = args->dest;
  stream->avail_out = (uInt)strlen(args->dest);
  args->status = deflateInit2(stream, args->compressionLevel, Z_DEFLATED, args->windowBits, 8, args->strategy);
  if (args->status != Z_OK)
  {
    return 0;
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

  int endStatus = deflateEnd(stream);
  if (args->status != Z_OK && args->status != Z_STREAM_END)
  {
    return 0;
  }

  args->status = endStatus;
  unsigned int result = stream->total_out;
  free(stream);
  return result;
}

ZLIBSHARP_NATIVE_EXTERN unsigned int Decompress(pcompress_decompress_args args)
{
  z_streamp stream = (z_streamp)malloc(sizeof(z_stream));
  if (!stream)
  {
    args->status = Z_MEM_ERROR;
    return 0;
  }

  stream->zalloc = Z_NULL;
  stream->zfree = Z_NULL;
  stream->opaque = Z_NULL;
  stream->next_in = args->source;
  stream->avail_in = (uInt)strlen(args->source);
  stream->next_out = args->dest;
  stream->avail_out = (uInt)strlen(args->dest);
  args->status = inflateInit2(stream, args->windowBits);
  if (args->status != Z_OK)
  {
    return 0;
  }

  do
  {
    args->status = inflate(stream, Z_NO_FLUSH);
    if (args->status != Z_OK && args->status != Z_STREAM_END)
    {
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
  while (args->status == Z_OK);

  int endStatus = inflateEnd(stream);
  if (args->status != Z_OK && args->status != Z_STREAM_END)
  {
    return 0;
  }

  args->status = endStatus;
  args->bytesWritten = stream->total_out;
  unsigned int result = stream->avail_in;
  free(stream);
  return result;
}

ZLIBSHARP_NATIVE_EXTERN unsigned long Crc32_ComputeHash(const unsigned char *source)
{
  return crc32(crc32(0L, Z_NULL, 0), source, (uInt)strlen(source));
}

ZLIBSHARP_NATIVE_EXTERN unsigned long Adler32_ComputeHash(const unsigned char *source)
{
  return adler32(adler32(0L, Z_NULL, 0), source, (uInt)strlen(source));
}
