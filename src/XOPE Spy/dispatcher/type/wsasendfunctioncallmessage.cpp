#include "../../definitions/definitions.h"

namespace dispatcher
{
	NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(dispatcher::WSASendFunctionCallMessage::Buffer, length, dataB64, modified, dropPacket);

	NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(WSASendFunctionCallMessage, functionName,
		socket,
		bufferCount,
		buffers,
		ret,
		tunneled,
		lastError);

	WSASendFunctionCallMessage::WSASendFunctionCallMessage() : IMessage(UiMessageType::HOOKED_FUNCTION_CALL) { };

	void WSASendFunctionCallMessage::serializeToJson(json& j)
	{
		IMessage::serializeToJson(j);
		to_json(j, *this);
	}

};