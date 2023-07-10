#include <Haxe.hpp>

#include "imgui/imgui.h"

class ExampleLayer : public Haxe::Layer
{
public:
	ExampleLayer()
		: Layer("Example")
	{
	}

	void OnUpdate() override
	{
		if (Haxe::Input::IsKeyPressed(HX_KEY_TAB))
			HX_TRACE("Tab key is pressed (poll)!");
	}

	virtual void OnImGuiRender() override
	{
		ImGui::Begin("Test");
		ImGui::Text("Hello World");
		ImGui::End();
	}

	void OnEvent(Haxe::Event& event) override
	{
		if (event.GetEventType() == Haxe::EventType::KeyPressed)
		{
			Haxe::KeyPressedEvent& e = (Haxe::KeyPressedEvent&)event;
			if (e.GetKeyCode() == HX_KEY_TAB)
				HX_TRACE("Tab key is pressed (event)!");
			HX_TRACE("{0}", (char)e.GetKeyCode());
		}
	}

};

class Sandbox :public Haxe::Application
{
public:
	Sandbox()
	{
		PushLayer(new ExampleLayer());
	}

	~Sandbox()
	{

	}


};

Haxe::Application* Haxe::CreateApplication()
{
	return new Sandbox();
}