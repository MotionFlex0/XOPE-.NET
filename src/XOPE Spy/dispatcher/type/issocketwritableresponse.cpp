#include "../../definitions/definitions.h"

namespace dispatcher
{
	NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(IsSocketWritableResponse,
		jobId, writable, timedOut, error, lastError);

	IsSocketWritableResponse::IsSocketWritableResponse(
		Guid jobId,
		bool writable,
		bool timedOut,
		bool error,
		int lastError
	) : IMessageResponse(UiMessageType::JOB_RESPONSE_SUCCESS, jobId),
		writable(writable),
		timedOut(timedOut),
		error(error),
		lastError(lastError) 
	{ 
	}

	void IsSocketWritableResponse::serializeToJson(json& j)
	{
		IMessageResponse::serializeToJson(j);
		to_json(j, *this);
	}

};