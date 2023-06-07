#include "Entity.h"

namespace phoenix::game_entity
{
	namespace
	{
		std::vector<id::generation_type> generations;
	}

	entity_id create_game_entity(const entity_info& info)
	{
		assert(info.transform);

		if (!info.transform) return entity_id{ id::invalid_id };
	}
	void remove_game_entity(entity_id id)
	{

	}
	bool is_alive(entity_id id)
	{

	}
}