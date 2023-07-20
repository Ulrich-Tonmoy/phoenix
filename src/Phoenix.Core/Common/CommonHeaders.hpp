#pragma once
#pragma warning(disable: 4530)

// C/C++
#include <stdio.h>
#include <assert.h>
#include <typeinfo>
#include <memory>
#include <unordered_map>

#if defined(_WIN64)
#include <DirectXMath.h>
#endif

// Custom
#include "..\Utilities\Utilities.hpp"
#include "..\Utilities\MathTypes.hpp"
#include "PrimitiveTypes.hpp"
#include "Id.hpp"