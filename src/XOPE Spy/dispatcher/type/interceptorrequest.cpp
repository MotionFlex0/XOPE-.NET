#include "../../definitions/definitions.h"

namespace dispatcher
{
	NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(InterceptorRequest,
		functionName, socket, packetDataB64);

	InterceptorRequest::InterceptorRequest(
		HookedFunction functionName,
		SOCKET socket,
		PacketDataJsonWrapper packetDataB64
	) : IMessageWithResponse(UiMessageType::INTERCEPTOR_REQUEST),
		functionName(functionName),
		socket(socket),
		packetDataB64(packetDataB64) { }

	void InterceptorRequest::serializeToJson(json& j)
	{
		IMessageWithResponse::serializeToJson(j);
		to_json(j, *this);
	}

}