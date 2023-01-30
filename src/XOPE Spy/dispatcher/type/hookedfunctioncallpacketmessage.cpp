#include "../../definitions/definitions.h"

namespace dispatcher
{
	NLOHMANN_DEFINE_TYPE_NON_INTRUSIVE(HookedFunctionCallPacketMessage, functionName,
		socket,
		packetLen,
		packetDataB64,
		ret,
		modified,
		tunneled,
		dropPacket,
		lastError);

	HookedFunctionCallPacketMessage::HookedFunctionCallPacketMessage() : IMessage(UiMessageType::HOOKED_FUNCTION_CALL) { };

	void HookedFunctionCallPacketMessage::serializeToJson(json& j)
	{
		IMessage::serializeToJson(j);
		to_json(j, *this);
	}

};