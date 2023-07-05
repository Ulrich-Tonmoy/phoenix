#pragma	once

#include "CommonHeaders.hpp"

#if !defined(PROD)
namespace phoenix::content
{
	bool load_game();
	void unload_game();
}
#endif