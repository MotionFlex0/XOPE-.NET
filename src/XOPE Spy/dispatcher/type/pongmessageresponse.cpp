#include "../../definitions/definitions.h"

namespace dispatcher
{
	NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(PongMessageResponse, data);

	PongMessageResponse::PongMessageResponse(Guid jobId) : IMessageResponse(UiMessageType::JOB_RESPONSE_SUCCESS, jobId) { }

	void PongMessageResponse::serializeToJson(json& j)
	{
		IMessageResponse::serializeToJson(j);
		to_json(j, *this);
	}

};