#include "../../definitions/definitions.h"

namespace dispatcher
{
	NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(dispatcher::WSARecvFunctionCallMessage::Buffer, length, dataB64, modified, dropPacket);

	NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(WSARecvFunctionCallMessage, functionName,
		socket,
		bufferCount,
		buffers,
		tunneled,
		ret,
		lastError);

	WSARecvFunctionCallMessage::WSARecvFunctionCallMessage() : IMessage(UiMessageType::HOOKED_FUNCTION_CALL) 
	{ 
	};

	void WSARecvFunctionCallMessage::serializeToJson(json& j)
	{
		IMessage::serializeToJson(j);
		to_json(j, *this);
	}
};


