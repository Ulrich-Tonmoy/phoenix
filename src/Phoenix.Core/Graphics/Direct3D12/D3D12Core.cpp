#include "D3D12Core.hpp"

using namespace Microsoft::WRL;

namespace phoenix::graphics::d3d12::core
{
	namespace
	{
		ID3D12Device8* main_device{ nullptr };
		IDXGIFactory7* dxgi_factory{ nullptr };
		constexpr D3D_FEATURE_LEVEL minimun_feature_level{ D3D_FEATURE_LEVEL_11_0 };

		bool failed_init()
		{
			shutdown();
			return false;
		}

		IDXGIAdapter4* determine_main_adapter()
		{
			IDXGIAdapter4* adapter{ nullptr };
			for (u32 i{ 0 };dxgi_factory->EnumAdapterByGpuPreference(i, DXGI_GPU_PREFERENCE_HIGH_PERFORMANCE, IID_PPV_ARGS(&adapter)) != DXGI_ERROR_NOT_FOUND;++i)
			{
				if (SUCCEEDED(D3D12CreateDevice(adapter, minimun_feature_level, __uuidof(ID3D12Device), nullptr)))
				{
					return adapter;
				}
				release(adapter);
			}
			return nullptr;
		}

		D3D_FEATURE_LEVEL get_max_feature_level(IDXGIAdapter4* adapter)
		{
			constexpr D3D_FEATURE_LEVEL feature_levels[4]
			{
				D3D_FEATURE_LEVEL_11_0,
				D3D_FEATURE_LEVEL_11_1,
				D3D_FEATURE_LEVEL_12_0,
				D3D_FEATURE_LEVEL_12_1,
			};

			D3D12_FEATURE_DATA_FEATURE_LEVELS feature_level_info{};
			feature_level_info.NumFeatureLevels = _countof(feature_levels);
			feature_level_info.pFeatureLevelsRequested = feature_levels;

			ComPtr<ID3D12Device> device;
			DXCall(D3D12CreateDevice(adapter, minimun_feature_level, IID_PPV_ARGS(&device)));
			DXCall(device->CheckFeatureSupport(D3D12_FEATURE_FEATURE_LEVELS, &feature_level_info, sizeof(feature_level_info)));
			return feature_level_info.MaxSupportedFeatureLevel;
		}
	}

	bool initialize()
	{
		if (main_device) shutdown();
		u32 dxgi_factory_flags{ 0 };

#ifdef _DEBUG
		{
			ComPtr<ID3D12Debug3> debug_interface;
			DXCall(D3D12GetDebugInterface(IID_PPV_ARGS(&debug_interface)));
			debug_interface->EnableDebugLayer();
			dxgi_factory_flags |= DXGI_CREATE_FACTORY_DEBUG;
		}
#endif // _DEBUG

		HRESULT hr{ S_OK };
		DXCall(hr = CreateDXGIFactory2(dxgi_factory_flags, IID_PPV_ARGS(&dxgi_factory)));
		if (FAILED(hr)) return failed_init();

		// which graphics card to use
		ComPtr<IDXGIAdapter4> main_adapter;
		main_adapter.Attach(determine_main_adapter());
		if (!main_adapter) return failed_init();

		D3D_FEATURE_LEVEL max_feature_level{ get_max_feature_level(main_adapter.Get()) };
		assert(max_feature_level >= minimun_feature_level);
		if (max_feature_level < minimun_feature_level) return failed_init();

		DXCall(hr = D3D12CreateDevice(main_adapter.Get(), max_feature_level, IID_PPV_ARGS(&main_device)));
		if (FAILED(hr)) return failed_init();

		NAME_D3D12_OBJECT(main_device, L"Main D3D12 Device");

#ifdef _DEBUG
		{
			ComPtr<ID3D12InfoQueue> info_queue;
			DXCall(main_device->QueryInterface(IID_PPV_ARGS(&info_queue)));

			info_queue->SetBreakOnSeverity(D3D12_MESSAGE_SEVERITY_CORRUPTION, true);
			info_queue->SetBreakOnSeverity(D3D12_MESSAGE_SEVERITY_WARNING, true);
			info_queue->SetBreakOnSeverity(D3D12_MESSAGE_SEVERITY_ERROR, true);
		}
#endif // _DEBUG

		return true;
	}

	void shutdown()
	{
		release(dxgi_factory);
#ifdef _DEBUG
		{
			{
				ComPtr<ID3D12InfoQueue> info_queue;
				DXCall(main_device->QueryInterface(IID_PPV_ARGS(&info_queue)));

				info_queue->SetBreakOnSeverity(D3D12_MESSAGE_SEVERITY_CORRUPTION, false);
				info_queue->SetBreakOnSeverity(D3D12_MESSAGE_SEVERITY_WARNING, false);
				info_queue->SetBreakOnSeverity(D3D12_MESSAGE_SEVERITY_ERROR, false);
			}
			ComPtr<ID3D12DebugDevice2> debug_device;
			DXCall(main_device->QueryInterface(IID_PPV_ARGS(&debug_device)));
			release(main_device);
			DXCall(debug_device->ReportLiveDeviceObjects(D3D12_RLDO_SUMMARY | D3D12_RLDO_DETAIL | D3D12_RLDO_IGNORE_INTERNAL));
		}
#endif // _DEBUG
		release(main_device);
	}
}