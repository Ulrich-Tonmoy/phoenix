#include <Haxe.hpp>

class Sandbox :public Haxe::Application
{
public:
	Sandbox()
	{

	}

	~Sandbox()
	{

	}


};

Haxe::Application* Haxe::CreateApplication()
{
	return new Sandbox();
}