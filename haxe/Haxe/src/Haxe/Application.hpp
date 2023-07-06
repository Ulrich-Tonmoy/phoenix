#pragma once

#include "Core.hpp"
#include "Events/Event.hpp"

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

