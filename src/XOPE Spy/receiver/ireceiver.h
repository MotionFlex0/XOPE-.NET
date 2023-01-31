#pragma once
#include <optional>
#include "incomingmessage.h"

class IReceiver
{
public:
	virtual std::optional<IncomingMessage> getIncomingMessage() = 0;
};