#pragma once

#include "CommonHeaders.hpp"
#include "MathTypes.hpp"

namespace phoenix::math
{
	template<typename T>
	constexpr T clamp(T value, T min, T max)
	{
		return (value < min) ? min : (value > max) ? max : value;
	}
}