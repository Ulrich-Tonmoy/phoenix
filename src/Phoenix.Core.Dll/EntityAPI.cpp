#include "Common.hpp"
#include "CommonHeaders.hpp"
#include "Id.hpp"
#include "..\Phoenix.Core\Components\Entity.hpp"
#include "..\Phoenix.Core\Components\Transform.hpp"
#include "..\Phoenix.Core\Components\Script.hpp"

using namespace phoenix;

namespace
{
	struct transform_component
	{
		f32 position[3];
		f32 rotation[3];
		f32 scale[3];

		transform::init_info to_init_info()
		{
			using namespace DirectX;
			transform::init_info info{};
			memcpy(&info.position[0], &position[0], sizeof(f32) * _countof(position));
			memcpy(&info.scale[0], &scale[0], sizeof(f32) * _countof(scale));
			XMFLOAT3A rot{ &rotation[0] };
			XMVECTOR quat{ XMQuaternionRotationRollPitchYawFromVector(XMLoadFloat3A(&rot)) };
			XMFLOAT4A rot_quat{};
			XMStoreFloat4A(&rot_quat, quat);
			memcpy(&info.rotation[0], &rot_quat.x, sizeof(f32) * _countof(info.rotation));
			return info;
		}
	};

	struct script_component
	{
		script::detail::script_creator script_creator;

		script::init_info to_init_info()
		{
			script::init_info info{};
			info.script_creator = script_creator;
			return info;
		}
	};

	struct game_entity_descriptor
	{
		transform_component transform;
		script_component script;
	};

	game_entity::entity entity_from_id(id::id_type id)
	{
		return game_entity::entity{ game_entity::entity_id(id) };
	}
}

EDITOR_INTERFACE id::id_type CreateGameEntity(game_entity_descriptor* e)
{
	assert(e);
	game_entity_descriptor& desc{ *e };
	transform::init_info transform_info{desc.transform.to_init_info()};
	script::init_info script_info{desc.script.to_init_info()};
	game_entity::entity_info entity_info
	{
		&transform_info,
			& script_info,

	};
	return game_entity::create(entity_info).get_id();
}

EDITOR_INTERFACE void RemoveGameEntity(id::id_type id)
{
	assert(id::is_valid(id));
	game_entity::remove(game_entity::entity_id{id});
}