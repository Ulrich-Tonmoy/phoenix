#pragma once

#include "Core.hpp"

#include "Window.hpp"
#include "Haxe/LayerStack.hpp"
#include "Haxe/Events/Event.hpp"
#include "Haxe/Events/ApplicationEvent.hpp"

#include "Haxe/ImGui/ImGuiLayer.hpp"

#include "Haxe/Renderer/Shader.hpp"
#include "Haxe/Renderer/Buffer.hpp"
#include "Haxe/Renderer/VertexArray.hpp"

namespace Haxe
{
	class HAXE_API Application
	{
	public:
		Application();
		virtual ~Application() = default;

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

		std::shared_ptr<Shader> m_Shader;
		std::shared_ptr<VertexArray> m_VertexArray;

		std::shared_ptr<Shader> m_BlueShader;
		std::shared_ptr<VertexArray> m_SquareVA;
	private:
		static Application* s_Instance;
	};

	Application* CreateApplication();
}

