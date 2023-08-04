#include "CommonHeaders.hpp"
#include "D3D12Interface.hpp"
#include "D3D12Core.hpp"
#include "Graphics\GraphicsPlatformInterface.hpp"

namespace phoenix::graphics::d3d12
{
	void get_platform_interface(platform_interface& pi)
	{
		pi.initialize = core::initialize;
		pi.shutdown = core::shutdown;
		pi.render = core::render;
	}
}