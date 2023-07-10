#pragma once

#ifdef HX_PLATFORM_WINDOWS
extern Haxe::Application* Haxe::CreateApplication();
int main(int argc, char** argv)
{
	Haxe::Log::Init();
	HX_CORE_WARN("Initialized Log!");
	int a = 5;
	HX_INFO("Hello! Var={0}", a);

	auto app = Haxe::CreateApplication();
	app->Run();
	delete app;
}
#endif // HX_PLATFORM_WINDOWS
