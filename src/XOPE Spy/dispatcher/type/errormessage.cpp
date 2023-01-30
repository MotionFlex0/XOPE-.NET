#include "../../definitions/definitions.h"

namespace dispatcher
{
	NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(ErrorMessage, errorMessage);

	ErrorMessage::ErrorMessage(std::string errMsg) : 
		IMessage(UiMessageType::ERROR_MESSAGE), 
		errorMessage(errMsg) 
	{ 
	}

	void ErrorMessage::serializeToJson(json& j)
	{
		IMessage::serializeToJson(j);
		to_json(j, *this);
	}

};