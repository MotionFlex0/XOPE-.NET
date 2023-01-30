#pragma once

#include "imessage.h"

using nlohmann::json;

IMessage::IMessage(UiMessageType m) 
{ 
	messageType = m; 
}

void IMessage::serializeToJson(json& j)
{
	to_json(j, *this);
}

void to_json(json& j, IMessage& mes)
{
	j["messageType"] = mes.messageType;
}
	
//NLOHMANN_DEFINE_TYPE_INTRUSIVE(IMessage, messageType);