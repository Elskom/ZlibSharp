#pragma once

#ifndef __cplusplus
#error "This file can only be included in C++ code."
#endif

namespace ZlibSharp
{
  class chunk
  {
  public:
    ZLIBSHARP_CONSTEXPR chunk() noexcept : _data() {}

    ZLIBSHARP_CONSTEXPR Byte *resize_and_get_data() noexcept
    {
      size_t index = size();
      _data.resize(index + chunk_size()); // Increase the size of the vector to accommodate a new chunk.
      return data() + index;
    }

    ZLIBSHARP_NODISCARD ZLIBSHARP_CONSTEXPR Byte *data() noexcept
    {
      return _data.data();
    }

    ZLIBSHARP_NODISCARD ZLIBSHARP_CONSTEXPR std::vector<Byte>::size_type size() const noexcept
    {
      return _data.size();
    }

    ZLIBSHARP_NODISCARD static ZLIBSHARP_CONSTEXPR std::vector<Byte>::size_type chunk_size() noexcept
    {
      return 262144; // 256 KB chunks for compression & decompression
    }

    ZLIBSHARP_NODISCARD ZLIBSHARP_CONSTEXPR std::vector<Byte>::iterator begin() noexcept
    {
      return _data.begin();
    }

    ZLIBSHARP_NODISCARD ZLIBSHARP_CONSTEXPR std::vector<Byte>::const_iterator begin() const noexcept
    {
      return _data.begin();
    }

    ZLIBSHARP_NODISCARD ZLIBSHARP_CONSTEXPR std::vector<Byte>::iterator end() noexcept
    {
      return _data.end();
    }

    ZLIBSHARP_NODISCARD ZLIBSHARP_CONSTEXPR std::vector<Byte>::const_iterator end() const noexcept
    {
      return _data.end();
    }

    ZLIBSHARP_CONSTEXPR void clear() noexcept
    {
      _data.clear();
    }
  private:
    std::vector<Byte> _data;
  };
}
