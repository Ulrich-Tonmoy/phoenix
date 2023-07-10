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
IncludeDir["glm"] = "Haxe/vendor/glm"

group "Deps"
	include "Haxe/vendor/GLFW"
	include "Haxe/vendor/Glad"
	include "Haxe/vendor/imgui"
group ""

project "Haxe"
	location "Haxe"
	kind "StaticLib"
	language "C++"
	cppdialect "C++17"
	staticruntime "on"

	targetdir ("bin/" .. outputdir .. "/%{prj.name}")
	objdir ("bin-int/" .. outputdir .. "/%{prj.name}")
	
	pchheader "hxpch.hpp"
	pchsource "Haxe/src/hxpch.cpp"

	files
	{
		"%{prj.name}/src/**.hpp",
		"%{prj.name}/src/**.cpp",
		"%{prj.name}/vendor/glm/glm/**.hpp",
		"%{prj.name}/vendor/glm/glm/**.inl",
	}

	defines
	{
		"_CRT_SECURE_NO_WARNINGS"
	}

	includedirs
	{
		"%{prj.name}/src",
		"%{prj.name}/vendor/spdlog/include",
		"%{IncludeDir.GLFW}",
		"%{IncludeDir.Glad}",
		"%{IncludeDir.ImGui}",
		"%{IncludeDir.glm}"
	}
	
	links 
	{ 
		"GLFW",
		"Glad",
		"ImGui",
		"opengl32.lib"
	}

	filter "system:windows"
		systemversion "latest"

		defines
		{
			"HX_PLATFORM_WINDOWS",
			"HX_BUILD_DLL",
			"GLFW_INCLUDE_NONE"
		}

	filter "configurations:Debug"
		defines "HX_DEBUG"
		runtime "Debug"
		symbols "on"

	filter "configurations:Release"
		defines "HX_RELEASE"
		runtime "Release"
		optimize "on"

	filter "configurations:Dist"
		defines "HX_DIST"
		runtime "Release"
		optimize "on"

project "Sandbox"
	location "Sandbox"
	kind "ConsoleApp"
	language "C++"
	cppdialect "C++17"
	staticruntime "on"

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
		"Haxe/src",
		"Haxe/vendor",
		"%{IncludeDir.glm}"
	}

	filter "system:windows"
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
		symbols "on"

	filter "configurations:Release"
		defines "HX_RELEASE"
		runtime "Release"
		optimize "on"

	filter "configurations:Dist"
		defines "HX_DIST"
		runtime "Release"
		optimize "on"