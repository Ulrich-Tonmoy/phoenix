#pragma once

#include "Core.hpp"

namespace Haxe
{
	class HAXE_API Application
	{
	public:
		Application();
		virtual ~Application();

		void Run();
	};

	Application* CreateApplication();
}

