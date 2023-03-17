#pragma once
#include <optional>
#include <queue>
#include "../definitions/definitions.h"
#include "../packet/type.h"

struct SocketData
{
	std::optional<bool> isBlocking = std::nullopt;
	std::optional<int> ipVersion = std::nullopt; //AF_INET;
	bool isTunneled = false;
	bool socketIdSentToSink = true;
};