#pragma once

#include "CommonHeaders.hpp"
#include "..\Platform\Window.hpp"

namespace phoenix::graphics
{
	class surface
	{

	};

	struct render_surface
	{
		platform::window window{};
		surface surface{};
	};
}