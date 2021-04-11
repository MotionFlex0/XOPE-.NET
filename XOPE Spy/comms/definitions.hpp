#pragma once
#include <Windows.h>

#include "../nlohmann/json.hpp"
#include "../utils/base64.h"

using nlohmann::json;

enum ServerMessageType
{
	CONNECTED_SUCCESS,
	HOOKED_FUNCTION_CALL,
	REQUEST_SOCKET_INFO,
	PING
};

enum SpyMessageType
{
	INJECT_SEND,
	INJECT_RECV,
	SHUTDOWN_RECV_THREAD
};

enum HookedFunction
{
	CONNECT,
	SEND,
	RECV,
	CLOSE,
	WSACONNECT,
	WSASEND,
	WSARECV
};

namespace client
{
	struct IMessage
	{
		IMessage(ServerMessageType m) { messageType = m; }
		ServerMessageType messageType;
	};

	struct HookedFunctionCallPacketMessage : IMessage
	{
		HookedFunctionCallPacketMessage() : IMessage(ServerMessageType::HOOKED_FUNCTION_CALL) { };

		//TODO: Maybe use __FUNCTION__ instead of enums
		HookedFunction functionName;
		SOCKET socket;
		int packetLen;
		char* packetData;
	};

	struct HookedFunctionCallSocketMessage : IMessage
	{
		HookedFunctionCallSocketMessage() : IMessage(ServerMessageType::HOOKED_FUNCTION_CALL) { };

		HookedFunction functionName;
		SOCKET socket;
		sockaddr_in* addr;
	};

	struct ConnectedSuccessMessage : IMessage
	{
		ConnectedSuccessMessage() : IMessage(ServerMessageType::CONNECTED_SUCCESS) { }
	};

	inline void from_json(const json& j, IMessage& hfcm)
	{
		j.at("messageType").get_to(hfcm.messageType);
	}


	inline void from_json(const json& j, HookedFunctionCallPacketMessage& hfcm)
	{
		from_json(j, (IMessage&)hfcm);
		j.at("functionName").get_to(hfcm.functionName);
		j.at("socket").get_to(hfcm.socket);
		j.at("packetLen").get_to(hfcm.packetLen);
		hfcm.packetData = new char[hfcm.packetLen];
		memcpy(hfcm.packetData, base64_decode(j.at("packetData").get<std::string>()).c_str(), hfcm.packetLen);
	}

	inline void from_json(const json& j, HookedFunctionCallSocketMessage& hfcm)
	{
		/* json
		{
			functionName:int,
			socket:int,
			port:int,
			protocol:int,
			addr:scr/byte,
			addrType:int
		}*/

		from_json(j, (IMessage&)hfcm);
		j.at("functionName").get_to(hfcm.functionName);
		j.at("socket").get_to(hfcm.socket);
		j.at("port").get_to(hfcm.addr->sin_port);
		//j.at("protocol").
		//hfcm.packetData = new char[hfcm.packetLen];
		//memcpy(hfcm.packetData, base64_decode(j.at("packetData").get<std::string>()).c_str(), hfcm.packetLen);
	}

	inline void to_json(json& j, const IMessage& hfcm)
	{
		j["messageType"] = hfcm.messageType;
	}

	inline void to_json(json& j, const HookedFunctionCallPacketMessage& hfcm)
	{
		to_json(j, (IMessage)hfcm);
		j["functionName"] = hfcm.functionName;
		j["socket"] = hfcm.socket;
		if (hfcm.functionName != HookedFunction::CLOSE
			&& hfcm.functionName != HookedFunction::CONNECT
			&& hfcm.functionName != HookedFunction::WSACONNECT)
		{
			j["packetLen"] = hfcm.packetLen;
			j["packetData"] = base64_encode(std::string(hfcm.packetData, hfcm.packetLen));

		}
	}

	inline void to_json(json& j, const HookedFunctionCallSocketMessage& hfcm)
	{
		/* json
		{
			functionName:int,
			socket:int,
			protocol:int, //TODO: currently not implemented. need to call getsockopt(..) to get the type
			port:int,
			addr:scr/byte,
			addrType:int
		}*/

		to_json(j, (IMessage)hfcm);
		j["functionName"] = hfcm.functionName;
		j["socket"] = hfcm.socket;
		if (hfcm.functionName != HookedFunction::CLOSE)
		{
			j["protocol"] = -1;
			j["addrFamily"] = hfcm.addr->sin_family;
			j["addr"] = hfcm.addr->sin_addr.S_un.S_addr;
			j["port"] = ((hfcm.addr->sin_port & 0xFF) << 8) | ((hfcm.addr->sin_port >> 8));
		}

	}
}

