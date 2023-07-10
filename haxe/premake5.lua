workspace "Haxe"
	architecture "x64"
	startproject "Sandbox"

	configurations
	{
		"Debug",
		"Release",
		"Dist"
	}

outputdir = "%{cfg.buildcfg}-%{cfg.system}-%{cfg.architecture}"

IncludeDir = {}
IncludeDir["GLFW"] = "Haxe/vendor/GLFW/include"
IncludeDir["Glad"] = "Haxe/vendor/Glad/include"
IncludeDir["ImGui"] = "Haxe/vendor/imgui"

include "Haxe/vendor/GLFW"
include "Haxe/vendor/Glad"
include "Haxe/vendor/imgui"

project "Haxe"
	location "Haxe"
	kind "SharedLib"
	language "C++"
	staticruntime "off"

	targetdir ("bin/" .. outputdir .. "/%{prj.name}")
	objdir ("bin-int/" .. outputdir .. "/%{prj.name}")
	
	pchheader "hxpch.hpp"
	pchsource "Haxe/src/hxpch.cpp"

	files
	{
		"%{prj.name}/src/**.hpp",
		"%{prj.name}/src/**.cpp"
	}

	includedirs
	{
		"%{prj.name}/src",
		"%{prj.name}/vendor/spdlog/include",
		"%{IncludeDir.GLFW}",
		"%{IncludeDir.Glad}",
		"%{IncludeDir.ImGui}"
	}
	
	links 
	{ 
		"GLFW",
		"Glad",
		"ImGui",
		"opengl32.lib"
	}

	filter "system:windows"
		cppdialect "C++17"
		systemversion "latest"

		defines
		{
			"HX_PLATFORM_WINDOWS",
			"HX_BUILD_DLL",
			"GLFW_INCLUDE_NONE"
		}

		postbuildcommands
		{
			("{COPY} %{cfg.buildtarget.relpath} ../bin/" .. outputdir .. "/Sandbox")
		}

	filter "configurations:Debug"
		defines "HX_DEBUG"
		runtime "Debug"
		symbols "On"

	filter "configurations:Release"
		defines "HX_RELEASE"
		runtime "Release"
		optimize "On"

	filter "configurations:Dist"
		defines "HX_DIST"
		runtime "Release"
		optimize "On"

project "Sandbox"
	location "Sandbox"
	kind "ConsoleApp"
	language "C++"
	staticruntime "off"

	targetdir ("bin/" .. outputdir .. "/%{prj.name}")
	objdir ("bin-int/" .. outputdir .. "/%{prj.name}")

	files
	{
		"%{prj.name}/src/**.hpp",
		"%{prj.name}/src/**.cpp"
	}

	includedirs
	{
		"Haxe/vendor/spdlog/include",
		"Haxe/src"
	}

	filter "system:windows"
		cppdialect "C++17"
		systemversion "latest"

		defines
		{
			"HX_PLATFORM_WINDOWS"
		}

		links
		{
			"Haxe"
		}

	filter "configurations:Debug"
		defines "HX_DEBUG"
		runtime "Debug"
		symbols "On"

	filter "configurations:Release"
		defines "HX_RELEASE"
		runtime "Release"
		optimize "On"

	filter "configurations:Dist"
		defines "HX_DIST"
		runtime "Release"
		optimize "On"