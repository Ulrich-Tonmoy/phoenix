#pragma once

#include "Haxe/Layer.hpp"

#include "Haxe/Events/ApplicationEvent.hpp"
#include "Haxe/Events/KeyEvent.hpp"
#include "Haxe/Events/MouseEvent.hpp"

namespace Haxe
{
	class HAXE_API ImGuiLayer : public Layer
	{
	public:
		ImGuiLayer();
		~ImGuiLayer();

	private:
		virtual void OnAttach() override;
		virtual void OnDetach() override;
		virtual void OnImGuiRender() override;

		void Begin();
		void End();
	private:
		float m_Time = 0.0f;
	};

}