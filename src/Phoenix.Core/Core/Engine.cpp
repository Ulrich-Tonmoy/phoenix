#if !defined(PROD)
#include "..\Content\ContentLoader.hpp"
#include "..\Components\Script.hpp"

#include <thread>

bool engine_initialize()
{
	bool result{ phoenix::content::load_game() };
	return result;
}

void engine_update()
{
	phoenix::script::update(10.f);
	std::this_thread::sleep_for(std::chrono::milliseconds(10));
}

void engine_shutdown()
{
	phoenix::content::unload_game();
}
#endif