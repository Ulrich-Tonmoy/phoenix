#pragma once

#include "CommonHeaders.hpp"
#include "Renderer.hpp"

namespace phoenix::graphics
{
	struct platform_interface
	{
		bool(*initialize)(void);
		void(*shutdown)(void);
	};
}