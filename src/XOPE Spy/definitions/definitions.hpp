#pragma once
#include <WinSock2.h>
#include <Windows.h>
#include "../nlohmann/json.hpp"
#include "../utils/base64.h"

//#define KV(J, VAR) J[#VAR] = VAR
//#define KV(VAR) {#VAR, VAR}
//#define KNV(NAME, VAR) {#NAME,  VAR}
//#define KNV(J, NAME, VAR) J[#NAME] = VAR
//#define B64BYTES(STR, LEN) base64_encode(std::string(STR, LEN))

using nlohmann::json;

enum class ServerMessageType
{
	INVALID_MESSAGE,
	PING,
	PONG,
	ERROR_MESSAGE,
	CONNECTED_SUCCESS,
	HOOKED_FUNCTION_CALL,
	JOB_RESPONSE_SUCCESS,
	JOB_RESPONSE_ERROR
};

enum class SpyMessageType
{
	INVALID_MESSAGE,
	PING,
	PONG,
	ERROR_MESSAGE,
	INJECT_SEND,
	INJECT_RECV,
	IS_SOCKET_WRITABLE,
	REQUEST_SOCKET_INFO,
	ADD_SEND_FITLER,
	EDIT_SEND_FILTER,
	DELETE_SEND_FILTER,
	ADD_RECV_FITLER,
	EDIT_RECV_FILTER,
	DELETE_RECV_FILTER,
	SHUTDOWN_RECV_THREAD
};

enum class HookedFunction
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

		inline virtual void toJson(json& j)
		{
			/*j +=
			{
				KV(messageType)
			};*/
			//j.at("messageType").get_to(messageType);
		}

		inline static std::string convertBytesToB64String(const char* bytes, const unsigned int len)
		{
			return base64_encode(std::string(bytes, len));
		}
	};

	struct IMessageResponse : IMessage
	{
		IMessageResponse(ServerMessageType m, std::string jobId) : IMessage(m) { IMessageResponse::jobId = jobId; }
		std::string jobId;
	};

	struct HookedFunctionCallPacketMessage : IMessage
	{
		HookedFunctionCallPacketMessage() : IMessage(ServerMessageType::HOOKED_FUNCTION_CALL) { };

		//TODO: Maybe use __FUNCTION__ instead of enums for HookedFunction
		HookedFunction functionName;
		SOCKET socket;
		int packetLen;
		std::string packetDataB64;
		int ret;
		bool modified = false;
		int lastError = -1;

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(HookedFunctionCallPacketMessage, messageType, functionName,
			socket, 
			packetLen, 
			packetDataB64,
			ret,
			modified,
			lastError);
	};

	struct ErrorMessage : IMessage
	{
		ErrorMessage(std::string errMsg) : IMessage(ServerMessageType::ERROR_MESSAGE), errorMessage(errMsg) { }
		
		std::string errorMessage;
		
		NLOHMANN_DEFINE_TYPE_INTRUSIVE(ErrorMessage, messageType, errorMessage);
	};

	struct ErrorMessageResponse : IMessageResponse
	{
		ErrorMessageResponse(std::string jobId, std::string errMsg) 
			: IMessageResponse(ServerMessageType::JOB_RESPONSE_ERROR, jobId), errorMessage(errMsg) { }

		std::string errorMessage;

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(ErrorMessageResponse, messageType, jobId, errorMessage);
	};

	struct PongMessageResponse : IMessageResponse
	{
		PongMessageResponse(std::string jobId) : IMessageResponse(ServerMessageType::JOB_RESPONSE_SUCCESS, jobId) { }

		std::string data = "PONG!";

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(PongMessageResponse, messageType, jobId, data);
	};

	struct SocketInfoResponse : IMessageResponse
	{
		SocketInfoResponse(std::string jobId, std::string addr, int port, int addrFamily, int protocol) 
			: IMessageResponse(ServerMessageType::JOB_RESPONSE_SUCCESS,
			jobId), 
			addr(addr), 
			port(port), 
			addrFamily(addrFamily), 
			protocol(protocol) { }

		std::string addr;
		int port;
		int addrFamily;
		int protocol;

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(SocketInfoResponse, messageType, jobId, addr, port, addrFamily, protocol);
	};

	struct HookedFunctionCallSocketMessage : IMessage
	{
		HookedFunctionCallSocketMessage() : IMessage(ServerMessageType::HOOKED_FUNCTION_CALL) { };

		HookedFunction functionName;
		SOCKET socket;
		sockaddr_in* addr;
		int ret;
		int lastError = -1;

		inline void toJson(json& j) override
		{
			IMessage::toJson(j);
				
		}
	};

	struct IsSocketWritableResponse : IMessageResponse
	{
		IsSocketWritableResponse(
			std::string jobId,
			bool writable,
			bool timedOut = false,
			bool error = false,
			int lastError = -1
		) : IMessageResponse(ServerMessageType::JOB_RESPONSE_SUCCESS, jobId),
			writable(writable), 
			timedOut(timedOut),
			error(error),
			lastError(lastError) { }

		bool writable;
		bool timedOut = false;
		bool error = false;
		int lastError = -1;

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(IsSocketWritableResponse, 
			messageType, jobId, writable, timedOut, error, lastError);
	};

	struct AddXFilterResponse : IMessageResponse
	{
		AddXFilterResponse(
			std::string jobId,
			std::string filterId
		) : IMessageResponse(ServerMessageType::JOB_RESPONSE_SUCCESS, jobId),
			filterId(filterId) { }

		std::string filterId;

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(AddXFilterResponse,
			messageType, jobId, filterId);
	};


	struct ConnectedSuccessMessage : IMessage
	{
		ConnectedSuccessMessage(std::string spyPipeServerName) : IMessage(ServerMessageType::CONNECTED_SUCCESS), spyPipeServerName(spyPipeServerName) { }

		std::string spyPipeServerName;

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(ConnectedSuccessMessage,
			messageType,
			spyPipeServerName);
	};

	inline void from_json(const json& j, IMessage& hfcm)
	{
		j.at("messageType").get_to(hfcm.messageType);
	}


	/*inline void from_json(const json& j, HookedFunctionCallPacketMessage& hfcm)
	{
		from_json(j, (IMessage&)hfcm);
		j.at("functionName").get_to(hfcm.functionName);
		j.at("socket").get_to(hfcm.socket);
		j.at("packetLen").get_to(hfcm.packetLen);
		hfcm.packetData = new char[hfcm.packetLen];
		memcpy(hfcm.packetData, base64_decode(j.at("packetData").get<std::string>()).c_str(), hfcm.packetLen);
	}*/

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

	//inline void to_json(json& j, const HookedFunctionCallPacketMessage& hfcm)
	//{
	//	to_json(j, (IMessage)hfcm);
	//	j["functionName"] = hfcm.functionName;
	//	j["socket"] = hfcm.socket;
	//	if (hfcm.functionName != HookedFunction::CLOSE
	//		&& hfcm.functionName != HookedFunction::CONNECT
	//		&& hfcm.functionName != HookedFunction::WSACONNECT)
	//	{
	//		j["packetLen"] = hfcm.packetLen;
	//		j["packetData"] = base64_encode(std::string(hfcm.packetData, hfcm.packetLen));

	//	}
	//}

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
		j["ret"] = hfcm.ret;
		j["lastError"] = hfcm.lastError;
		if (hfcm.functionName != HookedFunction::CLOSE)
		{
			j["protocol"] = -1;
			j["addrFamily"] = hfcm.addr->sin_family;
			j["addr"] = hfcm.addr->sin_addr.S_un.S_addr;
			j["port"] = ((hfcm.addr->sin_port & 0xFF) << 8) | ((hfcm.addr->sin_port >> 8));
		}
	}
}