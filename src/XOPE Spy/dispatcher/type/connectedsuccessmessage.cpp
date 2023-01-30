#include "../../definitions/definitions.h"

namespace dispatcher
{
	NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(ConnectedSuccessMessage,
		spyPipeServerName);

	ConnectedSuccessMessage::ConnectedSuccessMessage(std::string spyPipeServerName) : 
		IMessage(UiMessageType::CONNECTED_SUCCESS), 
		spyPipeServerName(spyPipeServerName) 
	{ 
	}

	void ConnectedSuccessMessage::serializeToJson(json& j)
	{
		IMessage::serializeToJson(j);
		to_json(j, *this);
	}
};