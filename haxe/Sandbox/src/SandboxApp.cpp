#include <Haxe.hpp>

class ExampleLayer : public Haxe::Layer
{
public:
	ExampleLayer()
		: Layer("Example")
	{
	}

	void OnUpdate() override
	{
		HX_INFO("ExampleLayer::Update");
	}

	void OnEvent(Haxe::Event& event) override
	{
		HX_TRACE("{0}", event);
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