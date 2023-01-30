#include "../../definitions/definitions.h"

namespace dispatcher 
{
	NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(AddPacketFilterResponse,
		jobId, filterId);

	AddPacketFilterResponse::AddPacketFilterResponse(
			Guid jobId,
			Guid filterId
	) : IMessageResponse(UiMessageType::JOB_RESPONSE_SUCCESS, jobId),
		filterId(filterId) { }

	void AddPacketFilterResponse::serializeToJson(json& j) 
	{
		IMessageResponse::serializeToJson(j);
		to_json(j, *this);
	}

}