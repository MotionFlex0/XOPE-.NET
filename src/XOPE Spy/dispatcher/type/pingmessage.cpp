#include "../../definitions/definitions.h"

namespace dispatcher
{
	NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(PingMessage,
		data);

	PingMessage::PingMessage() :
		IMessage(UiMessageType::PING)
	{
	}

	void PingMessage::serializeToJson(json& j)
	{
		IMessage::serializeToJson(j);
		to_json(j, *this);
	}
}