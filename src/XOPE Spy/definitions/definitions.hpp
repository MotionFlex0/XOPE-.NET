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

#pragma warning(disable: 4996)

using nlohmann::json;

enum class UiMessageType
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
	ADD_PACKET_FITLER,
	MODIFY_PACKET_FILTER,
	TOGGLE_ACTIVATE_FILTER,
	DELETE_PACKET_FILTER,
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

enum class ReplayableFunction
{
	SEND,
	RECV,
	WSASEND,
	WSARECV
};
using FilterableFunction = ReplayableFunction;

namespace client
{
	struct IMessage
	{
		IMessage(UiMessageType m) { messageType = m; }
		UiMessageType messageType;

		inline virtual void toJson(json& j) { };
		inline virtual void fromJson(json& j) { };


		inline static std::string convertBytesToB64String(const char* bytes, const unsigned int len)
		{
			return base64_encode(std::string(bytes, len));
		}
	};

	struct IMessageResponse : IMessage
	{
		IMessageResponse(UiMessageType m, std::string jobId) : IMessage(m) { IMessageResponse::jobId = jobId; }
		std::string jobId;
	};

	struct HookedFunctionCallPacketMessage : IMessage
	{
		HookedFunctionCallPacketMessage() : IMessage(UiMessageType::HOOKED_FUNCTION_CALL) { };

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

	struct WSASendFunctionCallMessage : IMessage
	{
		struct Buffer 
		{
			size_t length;
			std::string dataB64;
			size_t bytesSent;
			bool modified = false;

			NLOHMANN_DEFINE_TYPE_INTRUSIVE(Buffer, length, dataB64, modified);
		};

		WSASendFunctionCallMessage() : IMessage(UiMessageType::HOOKED_FUNCTION_CALL) { };

		HookedFunction functionName;
		SOCKET socket;
		std::vector<Buffer> buffers;
		int bufferCount;
		int ret;
		int lastError = -1;

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(WSASendFunctionCallMessage, messageType, functionName,
			socket,
			bufferCount,
			buffers,
			ret,
			lastError);
	};

	struct WSARecvFunctionCallMessage : IMessage
	{
		struct Buffer
		{
			size_t length;
			std::string dataB64;
			bool modified = false;

			NLOHMANN_DEFINE_TYPE_INTRUSIVE(Buffer, length, dataB64, modified);
		};

		WSARecvFunctionCallMessage() : IMessage(UiMessageType::HOOKED_FUNCTION_CALL) { };

		HookedFunction functionName;
		SOCKET socket;
		std::vector<Buffer> buffers;
		int bufferCount;
		int ret;
		int lastError = -1;

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(WSARecvFunctionCallMessage, messageType, functionName,
			socket,
			bufferCount,
			buffers,
			ret,
			lastError);
	};

	struct ErrorMessage : IMessage
	{
		ErrorMessage(std::string errMsg) : IMessage(UiMessageType::ERROR_MESSAGE), errorMessage(errMsg) { }
		
		std::string errorMessage;
		
		NLOHMANN_DEFINE_TYPE_INTRUSIVE(ErrorMessage, messageType, errorMessage);
	};

	struct ErrorMessageResponse : IMessageResponse
	{
		ErrorMessageResponse(std::string jobId, std::string errMsg) 
			: IMessageResponse(UiMessageType::JOB_RESPONSE_ERROR, jobId), errorMessage(errMsg) { }

		std::string errorMessage;

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(ErrorMessageResponse, messageType, jobId, errorMessage);
	};

	struct PongMessageResponse : IMessageResponse
	{
		PongMessageResponse(std::string jobId) : IMessageResponse(UiMessageType::JOB_RESPONSE_SUCCESS, jobId) { }

		std::string data = "PONG!";

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(PongMessageResponse, messageType, jobId, data);
	};

	struct SocketInfoResponse : IMessageResponse
	{
		SocketInfoResponse(std::string jobId, std::string addr, int port, int addrFamily, int protocol) 
			: IMessageResponse(UiMessageType::JOB_RESPONSE_SUCCESS,
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
		HookedFunctionCallSocketMessage() : IMessage(UiMessageType::HOOKED_FUNCTION_CALL) { };

		HookedFunction functionName;
		SOCKET socket;
		sockaddr_in* sockaddr;
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
		) : IMessageResponse(UiMessageType::JOB_RESPONSE_SUCCESS, jobId),
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

	struct AddPacketFilterResponse : IMessageResponse
	{
		AddPacketFilterResponse(
			std::string jobId,
			std::string filterId
		) : IMessageResponse(UiMessageType::JOB_RESPONSE_SUCCESS, jobId),
			filterId(filterId) { }

		std::string filterId;

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(AddPacketFilterResponse,
			messageType, jobId, filterId);
	};

	struct GenericPacketFilterResponse : IMessageResponse
	{
		GenericPacketFilterResponse(
			std::string jobId
		) : IMessageResponse(UiMessageType::JOB_RESPONSE_SUCCESS, jobId)
		{ }

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(GenericPacketFilterResponse,
			messageType, jobId);
	};

	struct ConnectedSuccessMessage : IMessage
	{
		ConnectedSuccessMessage(std::string spyPipeServerName) : IMessage(UiMessageType::CONNECTED_SUCCESS), spyPipeServerName(spyPipeServerName) { }

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
		j.at("port").get_to(hfcm.sockaddr->sin_port);
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

	static void to_json(json& j, const HookedFunctionCallSocketMessage& hfcm)
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
			j["addrFamily"] = hfcm.sockaddr->sin_family;
			j["addr"] = std::string("0.0.0.0");
			j["port"] = ntohs(hfcm.sockaddr->sin_port);
		}
	}
}