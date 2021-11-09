#pragma once
#include "../nlohmann/json.hpp"
#include "../definitions/definitions.hpp"

struct IncomingMessage
{
	SpyMessageType type;

	
	json rawJsonData;
};