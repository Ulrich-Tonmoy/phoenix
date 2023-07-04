#include "Common.hpp"
#include "CommonHeaders.hpp"
#include "..\Phoenix.Core\Components\Script.hpp"

#ifndef WIN32_MEAN_AND_LEAN
#define WIN32_MEAN_AND_LEAN
#endif

#include <Windows.h>

using namespace phoenix;

namespace
{
	HMODULE script_dll{ nullptr };
	using _get_script_creator = phoenix::script::detail::script_creator(*)(size_t);
	_get_script_creator get_script_creator{ nullptr };
	using _get_script_names = LPSAFEARRAY(*)(void);
	_get_script_names get_script_names{ nullptr };
}

EDITOR_INTERFACE u32 LoadScriptDll(const char* dll_path)
{
	if (script_dll) return FALSE;
	script_dll = LoadLibraryA(dll_path);
	assert(script_dll);

	get_script_creator = (_get_script_creator)GetProcAddress(script_dll, "get_script_creator");
	get_script_names = (_get_script_names)GetProcAddress(script_dll, "get_script_names");

	return (script_dll && get_script_creator && get_script_names) ? TRUE : FALSE;
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

EDITOR_INTERFACE script::detail::script_creator GetScriptCreator(const char* name)
{
	return (script_dll && get_script_creator) ? get_script_creator(script::detail::string_hash()(name)) : nullptr;
}

EDITOR_INTERFACE LPSAFEARRAY GetScriptNames()
{
	return (script_dll && get_script_names) ? get_script_names() : nullptr;
}