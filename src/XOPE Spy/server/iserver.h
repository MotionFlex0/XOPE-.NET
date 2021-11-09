#pragma once
#include <optional>
#include "incomingmessage.h"

class IServer
{
public:
	virtual std::optional<IncomingMessage> getIncomingMessage() = 0;
};