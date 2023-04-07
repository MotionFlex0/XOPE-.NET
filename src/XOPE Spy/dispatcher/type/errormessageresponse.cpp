#include "../../definitions/definitions.h"

namespace dispatcher
{
	NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(ErrorMessageResponse, errorMessage);

	ErrorMessageResponse::ErrorMessageResponse(Guid jobId, std::string errMsg)
		: IMessageResponse(UiMessageType::JOB_RESPONSE_ERROR, jobId), errorMessage(errMsg) { }

	std::string errorMessage;

	void ErrorMessageResponse::serializeToJson(json& j)
	{
		IMessageResponse::serializeToJson(j);
		to_json(j, *this);
	}

};