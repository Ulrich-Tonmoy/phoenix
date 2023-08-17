#pragma once

#include "CommonHeaders.hpp"
#include "Renderer.hpp"
#include "Platform/Window.hpp"

namespace phoenix::graphics
{
	struct platform_interface
	{
		bool(*initialize)(void);
		void(*shutdown)(void);

		struct
		{
			surface(*create)(platform::window);
			void(*remove)(surface_id);
			void(*resize)(surface_id, u32, u32);
			u32(*width)(surface_id);
			u32(*height)(surface_id);
			void(*render)(surface_id);
		}surface;
	};
}