#pragma once

namespace game_project
{
	class character_script :public phoenix::script::entity_script
	{
	public:
		constexpr explicit character_script(phoenix::game_entity::entity entity) :phoenix::script::entity_script(entity) {}
		void update(float dt) override;
	};
}