#include "../../definitions/definitions.h"

namespace dispatcher
{
	NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(SocketInfoResponse, 
		jobId, sourceAddr, sourcePort, destAddr, destPort, addrFamily, protocol);

	SocketInfoResponse::SocketInfoResponse(
		Guid jobId, 
		std::string sourceAddr, 
		int sourcePort, 
		std::string destAddr, 
		int destPort, 
		int addrFamily, 
		int protocol
	) : IMessageResponse(UiMessageType::JOB_RESPONSE_SUCCESS, jobId),
		sourceAddr(sourceAddr),
		sourcePort(sourcePort),
		destAddr(destAddr),
		destPort(destPort),
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