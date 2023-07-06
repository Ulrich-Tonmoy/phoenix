#include "hxpch.hpp"
#include "Application.hpp"
#include "Events/ApplicationEvent.hpp"
#include "Log.hpp"

namespace Haxe
{
	Application::Application()
	{

	}
	Application::~Application()
	{

	}

	void Application::Run()
	{
		WindowResizeEvent e(1280, 720);
		if (e.IsInCategory(EventCategoryApplication))
		{
			HX_TRACE(e);
		}
		if (e.IsInCategory(EventCategoryInput))
		{
			HX_TRACE(e);
		}

		while (true);
	}
}