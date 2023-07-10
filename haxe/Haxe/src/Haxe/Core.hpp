#pragma once

#ifdef HX_PLATFORM_WINDOWS
#if HX_DYNAMIC_LINK
#ifdef HX_BUILD_DLL
#define HAXE_API __declspec(dllexport)
#else
#define HAXE_API __declspec(dllimport)
#endif // HX_BUILD_DLL
#else
#define HAXE_API
#endif // HX_DYNAMIC_LINK
#else
#error Haxe only supports windows!
#endif // HX_PLATFORM_WINDOWS

#ifdef HX_DEBUG
#define HX_ENABLE_ASSERTS
#endif

#ifdef HX_ENABLE_ASSERTS
#define HX_ASSERT(x, ...) { if(!(x)) { HX_ERROR("Assertion Failed: {0}", __VA_ARGS__); __debugbreak(); } }
#define HX_CORE_ASSERT(x, ...) { if(!(x)) { HX_CORE_ERROR("Assertion Failed: {0}", __VA_ARGS__); __debugbreak(); } }
#else
#define HX_ASSERT(x, ...)
#define HX_CORE_ASSERT(x, ...)
#endif // HX_ENABLE_ASSERTS

#define BIT(x) (1 << x)

#define HX_BIND_EVENT_FN(fn) std::bind(&fn, this, std::placeholders::_1)