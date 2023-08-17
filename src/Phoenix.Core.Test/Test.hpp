#pragma once

#include <thread>
#include <chrono>
#include <string>

#define TEST_ENTITY_COMPONENTS 0
#define TEST_WINDOW 0
#define TEST_RENDERER 1

class test
{
public:
	virtual bool initialize() = 0;
	virtual void run() = 0;
	virtual void shutdown() = 0;
};

#if _WIN64
#include <Windows.h>
#include "PrimitiveTypes.hpp"

class time_it
{
public:
	using clock = std::chrono::high_resolution_clock;
	using time_stamp = std::chrono::steady_clock::time_point;

	void begin()
	{
		_start = clock::now();
	}

	void end()
	{
		auto dt = clock::now() - _start;
		_ms_avg += ((float)std::chrono::duration_cast<std::chrono::milliseconds>(dt).count() - _ms_avg) / (float)_counter;
		++_counter;

		if (std::chrono::duration_cast<std::chrono::seconds>(clock::now() - _seconds).count() >= 1)
		{
			OutputDebugStringA("Average frame (ms): ");
			OutputDebugStringA(std::to_string(_ms_avg).c_str());
			OutputDebugStringA((" " + std::to_string(_counter)).c_str());
			OutputDebugStringA(" FPS");
			OutputDebugStringA("\n");
			_ms_avg = 0.f;
			_counter = 1;
			_seconds = clock::now();
		}
	}
private:
	float _ms_avg{ 0.f };
	u32 _counter{ 1 };
	time_stamp _start;
	time_stamp _seconds{ clock::now() };
};
#endif // _WIN64