#ifdef __cplusplus
extern "C"
{
#endif

#if defined(_WIN32) || defined(__CYGWIN__)
#ifdef ZLIBSHARPNATIVE_EXPORTS
#define ZLIBSHARP_NATIVE_EXTERN __declspec(dllexport)
#else
#define ZLIBSHARP_NATIVE_EXTERN __declspec(dllimport)
#endif
#else
// Preprocessor check for a compiler __attribute__. Always return 0
// if __has_attribute() macro is not defined.
#ifdef __has_attribute
#  define ZLIBSHARP__has_attribute(x) __has_attribute(x)
#else
#  define ZLIBSHARP__has_attribute(x) 0
#endif
/*
 * If we only ever used gcc >= 5, we could use __has_attribute(visibility)
 * as a cross-platform way to determine if visibility is supported. However,
 * we may still need to support gcc >= 4, as some Ubuntu LTS and Centos versions
 * have 4 < gcc < 5.
 */
#if ((defined(__GNUC__) && (__GNUC__ >= 4)) ||\
        (defined(__clang__) && ZLIBSHARP__has_attribute(visibility))) &&\
        defined(ZLIBSHARPNATIVE_EXPORTS)
#define ZLIBSHARP_NATIVE_EXTERN __attribute__ ((visibility ("default")))
#else
#define ZLIBSHARP_NATIVE_EXTERN
#endif
#endif

#include <zlib.h>

#ifdef __cplusplus
constexpr auto CHUNK_SIZE = 262144; // 256 KB chunks for decompression
#else
#define CHUNK_SIZE 262144 // 256 KB chunks for decompression
#endif

typedef struct _internal_members {
  z_streamp stream;
  std::vector<Byte> chunks;
} internal_members, *pinternal_members;

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
  pinternal_members reserved;
} compress_decompress_args, *pcompress_decompress_args;

ZLIBSHARP_NATIVE_EXTERN int Compress(pcompress_decompress_args args);
ZLIBSHARP_NATIVE_EXTERN uInt Decompress(pcompress_decompress_args args);
ZLIBSHARP_NATIVE_EXTERN void FreeOutput(Byte *dest);

#ifdef __cplusplus
}
#endif
