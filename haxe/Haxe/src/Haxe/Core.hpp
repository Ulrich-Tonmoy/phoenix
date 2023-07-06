#pragma once

#ifdef HX_PLATFORM_WINDOWS
#ifdef HX_BUILD_DLL
#define HAXE_API __declspec(dllexport)
#else
#define HAXE_API __declspec(dllimport)
#endif // HX_BUILD_DLL
#else
#error Haxe only supports windows!
#endif // HX_PLATFORM_WINDOWS
