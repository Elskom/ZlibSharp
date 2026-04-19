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
        cd zlib
        Write-Host "Applying zlib configure script with zconf.h, zconf.h.in, zutil.h, and gzguts.h patches..."
        git apply ../zlib_define_fixes.patch
        cd ..
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
        cd runtimes/linux-x86/native
        gcc -O3 -Wall -m32 -I../../../zlib -c ../../../zlib/adler32.c ../../../zlib/compress.c ../../../zlib/crc32.c ../../../zlib/deflate.c ../../../zlib/gzclose.c ../../../zlib/gzlib.c ../../../zlib/gzread.c ../../../zlib/gzwrite.c ../../../zlib/infback.c ../../../zlib/inffast.c ../../../zlib/inflate.c ../../../zlib/inftrees.c ../../../zlib/trees.c ../../../zlib/uncompr.c ../../../zlib/zutil.c -fPIC
        ar rsv libzstatic.a *.o
        rm -rf *.o
        g++ -std=c++20 -O3 -Wall -m32 -shared -o libZlibSharp.Native.so -DZLIBSHARPNATIVE_EXPORTS -I../../../zlib ../../../main.cpp libzstatic.a -fPIC -fpermissive
        cd ../../linux-x64/native
        gcc -O3 -Wall -m64 -I../../../zlib -c ../../../zlib/adler32.c ../../../zlib/compress.c ../../../zlib/crc32.c ../../../zlib/deflate.c ../../../zlib/gzclose.c ../../../zlib/gzlib.c ../../../zlib/gzread.c ../../../zlib/gzwrite.c ../../../zlib/infback.c ../../../zlib/inffast.c ../../../zlib/inflate.c ../../../zlib/inftrees.c ../../../zlib/trees.c ../../../zlib/uncompr.c ../../../zlib/zutil.c -fPIC
        ar rsv libzstatic.a *.o
        rm -rf *.o
        g++ -std=c++20 -O3 -Wall -m64 -shared -o libZlibSharp.Native.so -DZLIBSHARPNATIVE_EXPORTS -I../../../zlib ../../../main.cpp libzstatic.a -fPIC -fpermissive
        apt-get install gcc-arm-linux-gnueabihf g++-arm-linux-gnueabihf -y
        cd ../../linux-arm/native/
        arm-linux-gnueabihf-gcc -O3 -Wall -I../../../zlib -c ../../../zlib/adler32.c ../../../zlib/compress.c ../../../zlib/crc32.c ../../../zlib/deflate.c ../../../zlib/gzclose.c ../../../zlib/gzlib.c ../../../zlib/gzread.c ../../../zlib/gzwrite.c ../../../zlib/infback.c ../../../zlib/inffast.c ../../../zlib/inflate.c ../../../zlib/inftrees.c ../../../zlib/trees.c ../../../zlib/uncompr.c ../../../zlib/zutil.c -fPIC
        ar rsv libzstatic.a *.o
        rm -rf *.o
        arm-linux-gnueabihf-g++ -std=c++20 -O3 -Wall -shared -o libZlibSharp.Native.so -DZLIBSHARPNATIVE_EXPORTS -I../../../zlib ../../../main.cpp libzstatic.a -fPIC -fpermissive
        apt-get install gcc-aarch64-linux-gnu g++-aarch64-linux-gnu -y
        cd ../../linux-arm64/native/
        aarch64-linux-gnu-gcc -O3 -Wall -I../../../zlib -c ../../../zlib/adler32.c ../../../zlib/compress.c ../../../zlib/crc32.c ../../../zlib/deflate.c ../../../zlib/gzclose.c ../../../zlib/gzlib.c ../../../zlib/gzread.c ../../../zlib/gzwrite.c ../../../zlib/infback.c ../../../zlib/inffast.c ../../../zlib/inflate.c ../../../zlib/inftrees.c ../../../zlib/trees.c ../../../zlib/uncompr.c ../../../zlib/zutil.c -fPIC
        ar rsv libzstatic.a *.o
        rm -rf *.o
        aarch64-linux-gnu-g++ -std=c++20 -O3 -Wall -shared -o libZlibSharp.Native.so -DZLIBSHARPNATIVE_EXPORTS -I../../../zlib ../../../main.cpp libzstatic.a -fPIC -fpermissive
        cd ../../..
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
            cd runtimes/osx-x64/native
            gcc -O3 -Wall -m64 -I../../../zlib -c ../../../zlib/adler32.c ../../../zlib/compress.c ../../../zlib/crc32.c ../../../zlib/deflate.c ../../../zlib/gzclose.c ../../../zlib/gzlib.c ../../../zlib/gzread.c ../../../zlib/gzwrite.c ../../../zlib/infback.c ../../../zlib/inffast.c ../../../zlib/inflate.c ../../../zlib/inftrees.c ../../../zlib/trees.c ../../../zlib/uncompr.c ../../../zlib/zutil.c -fPIC
            ar rsv libzstatic.a *.o
            rm -rf *.o
            g++ -std=c++20 -O3 -Wall -m64 -dynamiclib -o libZlibSharp.Native.dylib -DZLIBSHARPNATIVE_EXPORTS -I../../../zlib ../../../main.cpp libzstatic.a -fPIC -fpermissive
            cd ../../osx-arm64/native
            gcc -O3 -Wall -arch arm64 -I../../../zlib -c ../../../zlib/adler32.c ../../../zlib/compress.c ../../../zlib/crc32.c ../../../zlib/deflate.c ../../../zlib/gzclose.c ../../../zlib/gzlib.c ../../../zlib/gzread.c ../../../zlib/gzwrite.c ../../../zlib/infback.c ../../../zlib/inffast.c ../../../zlib/inflate.c ../../../zlib/inftrees.c ../../../zlib/trees.c ../../../zlib/uncompr.c ../../../zlib/zutil.c -fPIC
            ar rsv libzstatic.a *.o
            rm -rf *.o
            g++ -std=c++20 -O3 -Wall -arch arm64 -dynamiclib -o libZlibSharp.Native.dylib -DZLIBSHARPNATIVE_EXPORTS -I../../../zlib ../../../main.cpp libzstatic.a -fPIC -fpermissive
            cd ../../..
            DotNet-Pack
        }
        else
        {
            Write-Host "This build script requires that gcc be installed with 'brew install gcc' after installing the command line tools with 'xcode-select --install' followed by installing homebrew with 'curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh' and then running that install script."
        }
    }
}
