#pragma once

#ifdef HX_PLATFORM_WINDOWS
extern Haxe::Application* Haxe::CreateApplication();
int main(int argc, char** argv)
{
	Haxe::Log::Init();
	HX_CORE_WARN("Log Init");
	HX_CORE_FATAL("Crit");
	HX_ERROR("Err");
	HX_FATAL("Log Init");

	auto app = Haxe::CreateApplication();
	app->Run();
	delete app;
}
#endif // HX_PLATFORM_WINDOWS
