#pragma once

#include "imessageresponse.h"

IMessageResponse::IMessageResponse(UiMessageType m, Guid jobId) : IMessage(m), jobId(jobId) 
{ 
}

void IMessageResponse::serializeToJson(json& j)
{
	IMessage::serializeToJson(j);
	to_json(j, *this);
}

//NLOHMANN_DEFINE_TYPE_INTRUSIVE(IMessageResponse, jobId);