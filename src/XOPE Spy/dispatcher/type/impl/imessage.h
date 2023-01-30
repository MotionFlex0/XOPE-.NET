#pragma once

#include "../../../nlohmann/json.hpp"
#include "../../uimessagetype.h"

// Not sure how I feel about this being here. Maybe only add this to the neccessary source files instead
using nlohmann::json;

struct IMessage
{
	IMessage(UiMessageType m);
	UiMessageType messageType;

	virtual void serializeToJson(json& j);

	//NLOHMANN_DEFINE_TYPE_INTRUSIVE(IMessage, messageType);

	friend void to_json(json& j, IMessage& mes);
};