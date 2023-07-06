#pragma once

#include "Core.hpp"

#include "spdlog/spdlog.h"
#include "spdlog/fmt/ostr.h"

namespace Haxe
{
	class HAXE_API Log
	{
	public:
		static void Init();

		inline static std::shared_ptr<spdlog::logger>& GetCoreLogger() { return s_CoreLogger; }
		inline static std::shared_ptr<spdlog::logger>& GetClientLogger() { return s_ClientLogger; }
	private:
		static std::shared_ptr<spdlog::logger> s_CoreLogger;
		static std::shared_ptr<spdlog::logger> s_ClientLogger;
	};
}

#define HX_CORE_TRACE(...) ::Haxe::Log::GetCoreLogger()->trace(__VA_ARGS__)
#define HX_CORE_INFO(...) ::Haxe::Log::GetCoreLogger()->info(__VA_ARGS__)
#define HX_CORE_WARN(...) ::Haxe::Log::GetCoreLogger()->warn(__VA_ARGS__)
#define HX_CORE_ERROR(...) ::Haxe::Log::GetCoreLogger()->error(__VA_ARGS__)
#define HX_CORE_FATAL(...) ::Haxe::Log::GetCoreLogger()->critical(__VA_ARGS__)

#define HX_TRACE(...) ::Haxe::Log::GetClientLogger()->trace(__VA_ARGS__)
#define HX_INFO(...) ::Haxe::Log::GetClientLogger()->info(__VA_ARGS__)
#define HX_WARN(...) ::Haxe::Log::GetClientLogger()->warn(__VA_ARGS__)
#define HX_ERROR(...) ::Haxe::Log::GetClientLogger()->error(__VA_ARGS__)
#define HX_FATAL(...) ::Haxe::Log::GetClientLogger()->critical(__VA_ARGS__)