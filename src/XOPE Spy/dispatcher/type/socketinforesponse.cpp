#include "../../definitions/definitions.h"

namespace dispatcher
{
	NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(SocketInfoResponse, jobId, addr, port, addrFamily, protocol);

	SocketInfoResponse::SocketInfoResponse(Guid jobId, std::string addr, int port, int addrFamily, int protocol) : 
		IMessageResponse(UiMessageType::JOB_RESPONSE_SUCCESS, jobId),
		addr(addr),
		port(port),
		addrFamily(addrFamily),
		protocol(protocol) 
	{ 
	}

	void SocketInfoResponse::serializeToJson(json& j)
	{
		IMessageResponse::serializeToJson(j);
		to_json(j, *this);
	}

};