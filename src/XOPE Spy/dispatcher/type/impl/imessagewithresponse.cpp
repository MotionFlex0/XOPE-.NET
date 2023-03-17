#pragma once

#include "imessagewithresponse.h"


IMessageWithResponse::IMessageWithResponse(UiMessageType m) : IMessage(m), jobId(Guid::newGuid()) 
{
}

void IMessageWithResponse::serializeToJson(json& j)
{
	IMessage::serializeToJson(j);
	to_json(j, *this);
}

void to_json(json& j, IMessageWithResponse& mes)
{
	j["jobId"] = mes.jobId;
}