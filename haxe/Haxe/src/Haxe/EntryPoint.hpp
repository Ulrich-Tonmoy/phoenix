#pragma once

#ifdef HX_PLATFORM_WINDOWS
extern Haxe::Application* Haxe::CreateApplication();
int main(int argc, char** argv)
{
	auto app = Haxe::CreateApplication();
	app->Run();
	delete app;
}
#endif // HX_PLATFORM_WINDOWS
