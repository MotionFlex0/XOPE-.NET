#include "../../definitions/definitions.h"

namespace dispatcher
{
	NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(InfoMessage, infoMessage);

	InfoMessage::InfoMessage(std::string info) : IMessage(UiMessageType::INFO_MESSAGE), infoMessage(info) { }

	void InfoMessage::serializeToJson(json& j)
	{
		IMessage::serializeToJson(j);
		to_json(j, *this);
	}

};