#pragma	once

#include "CommonHeaders.hpp"

#if !defined(PROD)
namespace phoenix::content
{
	bool load_game();
	void unload_game();

	bool load_engine_shaders(std::unique_ptr<u8[]>& shaders, u64& size);
}
#endif