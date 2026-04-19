#include <cstring>
#include <cstdlib>
#include <cstdio>
#include <cstdint>
#include <utility>
#include <vector>
#include "main.hpp"

ZLIBSHARP_NATIVE_EXTERN int Compress(ZlibSharp::zlib_utils::pcompress_decompress_args args)
{
  return ZlibSharp::zlib_utils::compress(args);
}

ZLIBSHARP_NATIVE_EXTERN uInt Decompress(ZlibSharp::zlib_utils::pcompress_decompress_args args)
{
  return ZlibSharp::zlib_utils::decompress(args);
}

ZLIBSHARP_NATIVE_EXTERN void FreeOutput(Byte *dest)
{
  free(dest);
}
