#pragma once

#include "Core.hpp"
#include "Window.hpp"
#include "Haxe/LayerStack.hpp"
#include "Haxe/Events/Event.hpp"
#include "Haxe/Events/ApplicationEvent.hpp"

#include "Haxe/ImGui/ImGuiLayer.hpp"

namespace Haxe
{
	class HAXE_API Application
	{
	public:
		Application();
		virtual ~Application();

		void Run();
		void OnEvent(Event& e);

		void PushLayer(Layer* layer);
		void PushOverlay(Layer* layer);

		inline Window& GetWindow() { return *m_Window; }

		inline static Application& Get() { return *s_Instance; }
	private:
		bool OnWindowClose(WindowCloseEvent& e);
		std::unique_ptr<Window> m_Window;
		ImGuiLayer* m_ImGuiLayer;
		bool m_Running = true;
		LayerStack m_LayerStack;
	private:
		static Application* s_Instance;
	};

	Application* CreateApplication();
}

