#pragma once

#include "Haxe/Renderer/GraphicsContext.hpp"

struct GLFWwindow;

namespace Haxe
{
	class OpenGLContext : public GraphicsContext
	{
	public:
		OpenGLContext(GLFWwindow* windowHandle);

		virtual void Init() override;
		virtual void SwapBuffers() override;
	private:
		GLFWwindow* m_WindowHandle;
	};

}