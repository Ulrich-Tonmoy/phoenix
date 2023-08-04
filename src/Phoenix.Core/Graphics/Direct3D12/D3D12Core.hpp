#pragma once

#include "D3D12CommonHeaders.hpp"

namespace phoenix::graphics::d3d12::core
{
	bool initialize();
	void shutdown();
	void render();

	template<typename T>
	constexpr void release(T*& resource)
	{
		if (resource)
		{
			resource->Release();
			resource = nullptr;
		}
	}
}