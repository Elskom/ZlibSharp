#!/usr/bin/env pwsh
function DotNet-Pack {
    if ([bool](Get-Command "dotnet" -ErrorAction SilentlyContinue))
    {
        dotnet pack -c Release ZlibSharp.Native.csproj
    }
    else
    {
        Write-Host "This build script requires that the .NET SDK v10.0.100+ to be installed and in the PATH."
    }
}

if ($IsWindows -eq $null -and $IsLinux -eq $null -and $IsMacOS -eq $null)
{
    Write-Host "This build script requires that it be run from inside of Powershell Core."
}
else
{
    if (-not (Test-Path -Path "zlib"))
    {
        git clone https://github.com/madler/zlib.git
    }
    else
    {
        Write-Host "zlib already cloned. Skipping the clone."
    }
    if ($IsWindows)
    {
        if ([bool](Get-Command "msbuild" -ErrorAction SilentlyContinue))
        {
            msbuild ZlibSharp.Native.slnx -p:Configuration=Release -p:Platform=Win32
            msbuild ZlibSharp.Native.slnx -p:Configuration=Release -p:Platform=x64
            msbuild ZlibSharp.Native.slnx -p:Configuration=Release -p:Platform=ARM64
            DotNet-Pack
        }
        else
        {
            Write-Host "This build script requires that it be run inside of a Developer Powershell Instance for Visual Studio 2022/2026."
        }
    }
    if ($IsLinux)
    {
        # GitHub actions can sometimes fail when not doing this first prior
        # to trying to install gcc-multilib, g++-multilib, and libc6-dev-i386.
        apt-get update -y
        apt-get install gcc-multilib g++-multilib libc6-dev-i386 -y
        cd zlib
        ./configure
        cd ..
        mkdir -p runtimes/linux-x86/native
        mkdir -p runtimes/linux-x64/native
        mkdir -p runtimes/linux-arm/native
        mkdir -p runtimes/linux-arm64/native
        gcc -O3 -Wall -m32 -shared -o runtimes/linux-x86/native/libZlibSharp.Native.so -DZLIBSHARPNATIVE_EXPORTS -I./zlib zlib/adler32.c zlib/compress.c zlib/crc32.c zlib/deflate.c zlib/gzclose.c zlib/gzlib.c zlib/gzread.c zlib/gzwrite.c zlib/infback.c zlib/inffast.c zlib/inflate.c zlib/inftrees.c zlib/trees.c zlib/uncompr.c zlib/zutil.c main.c -fPIC
        gcc -O3 -Wall -m64 -shared -o runtimes/linux-x64/native/libZlibSharp.Native.so -DZLIBSHARPNATIVE_EXPORTS -I./zlib zlib/adler32.c zlib/compress.c zlib/crc32.c zlib/deflate.c zlib/gzclose.c zlib/gzlib.c zlib/gzread.c zlib/gzwrite.c zlib/infback.c zlib/inffast.c zlib/inflate.c zlib/inftrees.c zlib/trees.c zlib/uncompr.c zlib/zutil.c main.c -fPIC
        apt-get install gcc-arm-linux-gnueabihf -y
        arm-linux-gnueabihf-gcc -O3 -Wall -shared -o runtimes/linux-arm/native/libZlibSharp.Native.so -DZLIBSHARPNATIVE_EXPORTS -I./zlib zlib/adler32.c zlib/compress.c zlib/crc32.c zlib/deflate.c zlib/gzclose.c zlib/gzlib.c zlib/gzread.c zlib/gzwrite.c zlib/infback.c zlib/inffast.c zlib/inflate.c zlib/inftrees.c zlib/trees.c zlib/uncompr.c zlib/zutil.c main.c -fPIC
        apt-get install gcc-aarch64-linux-gnu -y
        aarch64-linux-gnu-gcc -O3 -Wall -shared -o runtimes/linux-arm64/native/libZlibSharp.Native.so -DZLIBSHARPNATIVE_EXPORTS -I./zlib zlib/adler32.c zlib/compress.c zlib/crc32.c zlib/deflate.c zlib/gzclose.c zlib/gzlib.c zlib/gzread.c zlib/gzwrite.c zlib/infback.c zlib/inffast.c zlib/inflate.c zlib/inftrees.c zlib/trees.c zlib/uncompr.c zlib/zutil.c main.c -fPIC
        DotNet-Pack
    }
    if ($IsMacOS)
    {
        cd zlib
        ./configure
        cd ..
        mkdir -p runtimes/osx-x64/native
        mkdir -p runtimes/osx-arm64/native
        if ([bool](Get-Command "gcc" -ErrorAction SilentlyContinue))
        {
            gcc -O3 -Wall -m64 -dynamiclib -o runtimes/osx-x64/native/libZlibSharp.Native.dylib -DZLIBSHARPNATIVE_EXPORTS -I./zlib zlib/adler32.c zlib/compress.c zlib/crc32.c zlib/deflate.c zlib/gzclose.c zlib/gzlib.c zlib/gzread.c zlib/gzwrite.c zlib/infback.c zlib/inffast.c zlib/inflate.c zlib/inftrees.c zlib/trees.c zlib/uncompr.c zlib/zutil.c main.c -fPIC
            gcc -O3 -Wall -arch arm64 -dynamiclib -o runtimes/osx-arm64/native/libZlibSharp.Native.dylib -DZLIBSHARPNATIVE_EXPORTS -I./zlib zlib/adler32.c zlib/compress.c zlib/crc32.c zlib/deflate.c zlib/gzclose.c zlib/gzlib.c zlib/gzread.c zlib/gzwrite.c zlib/infback.c zlib/inffast.c zlib/inflate.c zlib/inftrees.c zlib/trees.c zlib/uncompr.c zlib/zutil.c main.c -fPIC
            DotNet-Pack
        }
        else
        {
            Write-Host "This build script requires that gcc be installed with 'brew install gcc' after installing the command line tools with 'xcode-select --install' followed by installing homebrew with 'curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh' and then running that install script."
        }
    }
}
