#pragma once

#include "../../../utils/guid.h"
#include "imessage.h"

// A message sent by the Spy, which expects a response from the UI
// Spy Request --> UI processes message and sent response to Spy --> Spy invokes callback function
struct IMessageWithResponse : IMessage
{
	IMessageWithResponse(UiMessageType m);
	Guid jobId;
};