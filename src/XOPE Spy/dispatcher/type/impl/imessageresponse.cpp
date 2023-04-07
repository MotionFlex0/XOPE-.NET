#pragma once

#include "imessageresponse.h"

NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(IMessageResponse, jobId);

IMessageResponse::IMessageResponse(UiMessageType m, Guid jobId) : IMessage(m), jobId(jobId) 
{ 
}

void IMessageResponse::serializeToJson(json& j)
{
	IMessage::serializeToJson(j);
	to_json(j, *this);
}
