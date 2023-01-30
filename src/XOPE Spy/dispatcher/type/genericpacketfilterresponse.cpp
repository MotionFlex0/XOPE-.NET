#include "../../definitions/definitions.h"

namespace dispatcher
{
	NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(GenericPacketFilterResponse,
		jobId);

	GenericPacketFilterResponse::GenericPacketFilterResponse(
		Guid jobId
	) : IMessageResponse(UiMessageType::JOB_RESPONSE_SUCCESS, jobId)
	{ 
	}

	void GenericPacketFilterResponse::serializeToJson(json& j)
	{
		IMessageResponse::serializeToJson(j);
		to_json(j, *this);
	}

};