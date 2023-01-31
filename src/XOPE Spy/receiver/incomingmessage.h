#pragma once
#include "../nlohmann/json.hpp"
#include "../definitions/definitions.h"

struct IncomingMessage
{
	SpyMessageType type;

	
	json rawJsonData;
};