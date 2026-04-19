#ifndef __cplusplus
#error "This file can only be included in C++ code."
#endif

#if defined(_WIN32) || defined(__CYGWIN__)
#ifdef ZLIBSHARPNATIVE_EXPORTS
#define ZLIBSHARP_NATIVE_EXTERN extern "C" __declspec(dllexport)
#else
#define ZLIBSHARP_NATIVE_EXTERN extern "C" __declspec(dllimport)
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

// vvv Copy & pasted from yvals_core.h as not all OSs that
// can build C++ code may have yvals_core.h,
// but renamed to avoid conflicts with the existing _CONSTEXPR20 macro. vvv

// [[nodiscard]] attributes on functions
#define ZLIBSHARP_NODISCARD [[nodiscard]]

// Functions/static constants that became constexpr in C++20
#if defined(_MSVC_LANG) && _MSVC_LANG >= 202002L && _MSVC_LANG > __cplusplus || __cplusplus >= 202002L
#if _MSVC_LANG > __cplusplus && __cplusplus < 202002L
#warning "In the Microsoft compiler add /Zc:__cplusplus to the compiler's additional options field to get the macro to conform with gcc/clang."
#endif
#define ZLIBSHARP_CONSTEXPR constexpr
#else // ^^^ constexpr in C++20 and later / inline (or const for static constants) (not constexpr) in C++17 and earlier vvv
#define ZLIBSHARP_CONSTEXPR inline
#endif // ^^^ inline (or const for static constants) (not constexpr) in C++17 and earlier ^^^

// ^^^ Copy & pasted from yvals_core.h, but renamed to avoid conflicts with the existing _CONSTEXPR20 macro. ^^^

#include <zlib.h>
#include "chunk.hpp"
#include "zlib_utils.hpp"

ZLIBSHARP_NATIVE_EXTERN int Compress(ZlibSharp::zlib_utils::pcompress_decompress_args args);
ZLIBSHARP_NATIVE_EXTERN uInt Decompress(ZlibSharp::zlib_utils::pcompress_decompress_args args);
ZLIBSHARP_NATIVE_EXTERN void FreeOutput(Byte *dest);
