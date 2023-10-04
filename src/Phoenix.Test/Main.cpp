#include "Test.hpp"
#pragma comment(lib,"phoenix.core.lib")

#if TEST_ENTITY_COMPONENTS
#include "TestEntityComponents.hpp"
#elif TEST_WINDOW
#include "TestWindow.hpp"
#elif TEST_RENDERER
#include "TestRenderer.hpp"
#else
#error One Of the tests need to be enabled
#endif

#ifdef _WIN64
#include <Windows.h>
#include <filesystem>

std::filesystem::path set_current_directory_to_executable_path()
{
	wchar_t path[MAX_PATH];
	const u32 length{ GetModuleFileName(0,&path[0],MAX_PATH) };
	if (!length || GetLastError() == ERROR_INSUFFICIENT_BUFFER) return {};
	std::filesystem::path p{ path };
	std::filesystem::current_path(p.parent_path());
	return std::filesystem::current_path();
}

int WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR, int)
{
#if _DEBUG
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif
	set_current_directory_to_executable_path();
	engine_test test{};
	if (test.initialize())
	{
		MSG msg{};
		bool is_running{ true };
		while (is_running)
		{
			while (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
			{
				TranslateMessage(&msg);
				DispatchMessage(&msg);
				is_running &= (msg.message != WM_QUIT);
			}
			test.run();
		}
	}
	test.shutdown();
	return 0;
}
#else
int main()
{
#if _DEBUG
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif
	engine_test test{};

	if (test.initialize())
	{
		test.run();
	}
	test.shutdown();
}
#endif