#pragma once

#include "../../../nlohmann/json.hpp"
#include "../../uimessagetype.h"

using nlohmann::json;

struct IMessage
{
	IMessage(UiMessageType m);
	UiMessageType messageType;

	virtual void serializeToJson(json& j);

	//NLOHMANN_DEFINE_TYPE_INTRUSIVE(IMessage, messageType);

	friend void to_json(json& j, IMessage& mes);
};