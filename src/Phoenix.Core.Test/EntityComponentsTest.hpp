#pragma once

#include "../Phoenix.Core/Components/Entity.h"
#include "../Phoenix.Core/Components/Transform.h"

#include "Test.hpp"

using namespace phoenix;

class engine_test :public test
{
public:
	bool initialize() override { return true; }
	void run() override { }
	void shutdown() override { }
};