#include "Common.h"
#include "CommonHeaders.h"

#ifndef WIN32_MEAN_AND_LEAN
#define WIN32_MEAN_AND_LEAN
#endif

#include <Windows.h>

using namespace phoenix;

namespace
{
	HMODULE script_dll{ nullptr };
}

EDITOR_INTERFACE u32 LoadScriptDll(const char* dll_path)
{
	if (script_dll) return FALSE;
	script_dll = LoadLibraryA(dll_path);
	assert(script_dll);

	return script_dll ? TRUE : FALSE;
}

EDITOR_INTERFACE u32 UnloadScriptDll()
{
	if (!script_dll) return FALSE;
	assert(script_dll);
	int result{ FreeLibrary(script_dll) };
	assert(result);
	script_dll = nullptr;
	return TRUE;
}
