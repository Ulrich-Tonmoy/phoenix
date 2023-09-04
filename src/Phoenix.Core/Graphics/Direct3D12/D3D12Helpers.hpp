#pragma once

#include "D3D12CommonHeaders.hpp"

namespace phoenix::graphics::d3d12::d3dx
{
	constexpr struct
	{
		const D3D12_HEAP_PROPERTIES default_heap{
			D3D12_HEAP_TYPE_DEFAULT,
			D3D12_CPU_PAGE_PROPERTY_UNKNOWN,
			D3D12_MEMORY_POOL_UNKNOWN,
			0,
			0
		};
	} heap_properties;
}