#pragma once

#include "../../../utils/guid.h"
#include "imessage.h"

// A response to message sent by the UI
// UI Request --> Spy processes message and sent response to UI --> UI invokes callback function
struct IMessageResponse : IMessage
{
	IMessageResponse(UiMessageType m, Guid jobId);
	Guid jobId;

	void serializeToJson(json& j) override;

	//NLOHMANN_DEFINE_TYPE_INTRUSIVE(IMessageResponse, jobId);
};