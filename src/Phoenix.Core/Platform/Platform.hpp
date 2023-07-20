#pragma once

#include "CommonHeaders.hpp"
#include "Window.hpp"

namespace phoenix::platform
{
	struct window_init_info;

	window create_window(const window_init_info* const init_info = nullptr);
	void remove_window(window_id id);
}