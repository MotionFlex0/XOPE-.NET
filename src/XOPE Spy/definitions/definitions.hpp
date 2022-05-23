#pragma once

#include <WinSock2.h>
#include <WS2tcpip.h>
#include <Windows.h>
#include "../nlohmann/json.hpp"
#include "../utils/base64.h"
#include "../utils/guid.h"

#pragma warning(disable: 4996)

using nlohmann::json;

enum class UiMessageType
{
	INVALID_MESSAGE,
	PING,
	PONG,
	INFO_MESSAGE,
	EXTERNAL_MESSAGE,
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
	CLOSE_SOCKET_GRACEFULLY,	// Imitates a socket closing via recv returning 0 or send error being WSAENOTCONN
	IS_SOCKET_WRITABLE,
	REQUEST_SOCKET_INFO,
	TOGGLE_HTTP_TUNNELING,
	STOP_HTTP_TUNNELING_SOCKET,
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
		IMessageResponse(UiMessageType m, Guid jobId) : IMessage(m), jobId(jobId) { }
		Guid jobId;
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

	struct InfoMessage : IMessage
	{
		InfoMessage(std::string info) : IMessage(UiMessageType::INFO_MESSAGE), infoMessage(info) { }

		std::string infoMessage;

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(InfoMessage, messageType, infoMessage);
	};

	// Only use for messages passed to extern SendMessageToLog
	struct ExternalMessage : IMessage
	{
		ExternalMessage(std::string info) : IMessage(UiMessageType::EXTERNAL_MESSAGE), externalMessage(info) { }

		std::string externalMessage;

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(ExternalMessage, messageType, externalMessage);
	};

	struct ErrorMessage : IMessage
	{
		ErrorMessage(std::string errMsg) : IMessage(UiMessageType::ERROR_MESSAGE), errorMessage(errMsg) { }
		
		std::string errorMessage;
		
		NLOHMANN_DEFINE_TYPE_INTRUSIVE(ErrorMessage, messageType, errorMessage);
	};

	struct ErrorMessageResponse : IMessageResponse
	{
		ErrorMessageResponse(Guid jobId, std::string errMsg) 
			: IMessageResponse(UiMessageType::JOB_RESPONSE_ERROR, jobId), errorMessage(errMsg) { }

		std::string errorMessage;

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(ErrorMessageResponse, messageType, jobId, errorMessage);
	};

	struct PongMessageResponse : IMessageResponse
	{
		PongMessageResponse(Guid jobId) : IMessageResponse(UiMessageType::JOB_RESPONSE_SUCCESS, jobId) { }

		std::string data = "PONG!";

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(PongMessageResponse, messageType, jobId, data);
	};

	struct SocketInfoResponse : IMessageResponse
	{
		SocketInfoResponse(Guid jobId, std::string addr, int port, int addrFamily, int protocol) 
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
		const sockaddr_storage* sockaddr;
		int ret;
		int lastError = -1;
		bool tunneling = false;

		inline void toJson(json& j) override
		{
			IMessage::toJson(j);
				
		}
	};

	struct IsSocketWritableResponse : IMessageResponse
	{
		IsSocketWritableResponse(
			Guid jobId,
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
			Guid jobId,
			Guid filterId
		) : IMessageResponse(UiMessageType::JOB_RESPONSE_SUCCESS, jobId),
			filterId(filterId) { }

		Guid filterId;

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(AddPacketFilterResponse,
			messageType, jobId, filterId);
	};

	struct GenericPacketFilterResponse : IMessageResponse
	{
		GenericPacketFilterResponse(
			Guid jobId
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

	inline void to_json(json& j, const IMessage& hfcm)
	{
		j["messageType"] = hfcm.messageType;
	}

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

		int oldWsaErrorCode = WSAGetLastError();

		to_json(j, (IMessage)hfcm);
		j["functionName"] = hfcm.functionName;
		j["socket"] = hfcm.socket;
		j["ret"] = hfcm.ret;
		j["lastError"] = hfcm.lastError;
		if (hfcm.functionName != HookedFunction::CLOSE)
		{
			j["tunneling"] = hfcm.tunneling;
			j["protocol"] = -1;
			j["addrFamily"] = hfcm.sockaddr->ss_family;

			if (hfcm.sockaddr->ss_family == AF_INET)
			{
				const sockaddr_in* sa = reinterpret_cast<const sockaddr_in*>(hfcm.sockaddr);

				char addr[INET_ADDRSTRLEN];
				int addrSize = sizeof(addr);
				int sinSize = sizeof(sockaddr_in);
				
				WSAAddressToStringA((LPSOCKADDR)sa, sinSize, NULL, addr, (LPDWORD)&addrSize);
				std::replace(addr, addr + sizeof(addr), ':', '\x00');
				j["addr"] = addr;
				j["port"] = ntohs(sa->sin_port);
			}
			else if (hfcm.sockaddr->ss_family == AF_INET6)
			{
				const sockaddr_in6* sa = reinterpret_cast<const sockaddr_in6*>(hfcm.sockaddr);

				char addr[INET6_ADDRSTRLEN];
				int addrSize = sizeof(addr);
				int sinSize = sizeof(sockaddr_in6);

				WSAAddressToStringA((LPSOCKADDR)sa, sinSize, NULL, addr, (LPDWORD)&addrSize);

				std::string address{ addr };
				auto colonPos = address.find_last_of(':');
				if (colonPos != std::string::npos)
					address.erase(colonPos);

				j["addr"] = address;
				j["port"] = ntohs(sa->sin6_port);
			}	
		}

		WSASetLastError(oldWsaErrorCode);
	}
}