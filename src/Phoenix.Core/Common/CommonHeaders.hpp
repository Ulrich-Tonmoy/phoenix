#pragma once
#pragma warning(disable: 4530)

// C/C++
#include <stdio.h>
#include <assert.h>
#include <typeinfo>
#include <memory>
#include <unordered_map>
#include <string>

#if defined(_WIN64)
#include <DirectXMath.h>
#endif

#ifndef DISABLE_COPY
#define DISABLE_COPY(T) \
		explicit T(const T&) = delete;\
		T& operator=(const T&) = delete;
#endif // !DISABLE_COPY

#ifndef DISABLE_MOVE
#define DISABLE_MOVE(T) \
		explicit T(T&&) = delete;\
		T& operator=(T&&) = delete;
#endif // !DISABLE_MOVE

#ifndef DISABLE_COPY_AND_MOVE
#define DISABLE_COPY_AND_MOVE(T) DISABLE_COPY(T) DISABLE_MOVE(T)
#endif // !DISABLE_COPY_AND_MOVE


// Custom
#include "PrimitiveTypes.hpp"
#include "..\Utilities\Math.hpp"
#include "..\Utilities\Utilities.hpp"
#include "..\Utilities\MathTypes.hpp"
#include "Id.hpp"

#ifdef _DEBUG
#define DEBUG_OP(x) x
#else
#define DEBUG_OP(x) (void(0))
#endif // DEBUG
