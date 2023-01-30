#include "../../definitions/definitions.h"

// Only use for messages passed to extern SendMessageToLog
namespace dispatcher
{
	NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(ExternalMessage, externalMessage, moduleName);

	ExternalMessage::ExternalMessage(std::string info, std::string moduleName) : 
		IMessage(UiMessageType::EXTERNAL_MESSAGE), 
		externalMessage(info), 
		moduleName(moduleName) 
	{ 
	}

	void ExternalMessage::serializeToJson(json& j)
	{
		IMessage::serializeToJson(j);
		to_json(j, *this);
	}

};