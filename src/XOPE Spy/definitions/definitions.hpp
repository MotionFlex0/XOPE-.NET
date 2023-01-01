#pragma once

#include <WinSock2.h>
#include <WS2tcpip.h>
#include <Windows.h>
#include <zlib.h>
#include "../nlohmann/json.hpp"
#include "../utils/base64.h"
#include "../utils/guid.h"
#include "../utils/assert.h"
#include "../utils/packetjsonwrapper.h"

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

		virtual void serializeToJson(json& j)
		{
			to_json(j, *this);
		}

		//NLOHMANN_DEFINE_TYPE_INTRUSIVE(IMessage, messageType);

		friend void to_json(json& j, IMessage& mes)
		{
			j["messageType"] = mes.messageType;
		}

		//static std::string convertBytesToCompressedB64(const char* bytes, const unsigned int len)
		//{
		//	if (len < 1)
		//		return { };

		//	std::array<Bytef, 32768> compressedBuf;
		//	uLong compressedBufSize = sizeof(compressedBuf);
		//	int err = compress(compressedBuf.data(), &compressedBufSize, reinterpret_cast<const unsigned char*>(bytes), len);
		//	x_assert(err == Z_OK, "failed to compress bytes");

		//	//return base64_encode(std::string(reinterpret_cast<const char*>(compressedBuf.data()), compressedBufSize));
		//	return { reinterpret_cast<const char*>(compressedBuf.data()), compressedBufSize };
		//}
	};

	struct IMessageResponse : IMessage
	{
		IMessageResponse(UiMessageType m, Guid jobId) : IMessage(m), jobId(jobId) { }
		Guid jobId;

		void serializeToJson(json& j) override
		{
			IMessage::serializeToJson(j);
			to_json(j, *this);
		}

		//NLOHMANN_DEFINE_TYPE_INTRUSIVE(IMessageResponse, jobId);
	};

	struct HookedFunctionCallPacketMessage : IMessage
	{
		HookedFunctionCallPacketMessage() : IMessage(UiMessageType::HOOKED_FUNCTION_CALL) { };

		//TODO: Maybe use __FUNCTION__ instead of enums for HookedFunction
		HookedFunction functionName;
		SOCKET socket;
		int packetLen;
		PacketDataJsonWrapper packetDataB64;
		int ret;
		bool modified = false;
		bool tunneled = false;
		bool dropPacket = false;
		int lastError = -1;

		void serializeToJson(json& j) override
		{
			IMessage::serializeToJson(j);
			to_json(j, *this);
		}

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(HookedFunctionCallPacketMessage, functionName,
			socket, 
			packetLen, 
			packetDataB64,
			ret,
			modified,
			tunneled,
			dropPacket,
			lastError);
	};

	struct WSASendFunctionCallMessage : IMessage
	{
		struct Buffer 
		{
			size_t length;
			PacketDataJsonWrapper dataB64;
			size_t bytesSent;
			bool modified = false;
			bool dropPacket = false;

			NLOHMANN_DEFINE_TYPE_INTRUSIVE(Buffer, length, dataB64, modified, dropPacket);
		};

		WSASendFunctionCallMessage() : IMessage(UiMessageType::HOOKED_FUNCTION_CALL) { };

		HookedFunction functionName;
		SOCKET socket;
		std::vector<Buffer> buffers;
		int bufferCount;
		int ret;
		bool tunneled = false;
		int lastError = -1;

		void serializeToJson(json& j) override
		{
			IMessage::serializeToJson(j);
			to_json(j, *this);
		}

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(WSASendFunctionCallMessage, functionName,
			socket,
			bufferCount,
			buffers,
			ret,
			tunneled,
			lastError);
	};

	struct WSARecvFunctionCallMessage : IMessage
	{
		struct Buffer
		{
			size_t length;
			PacketDataJsonWrapper dataB64;
			bool modified = false;
			bool dropPacket = false;

			NLOHMANN_DEFINE_TYPE_INTRUSIVE(Buffer, length, dataB64, modified, dropPacket);
		};

		WSARecvFunctionCallMessage() : IMessage(UiMessageType::HOOKED_FUNCTION_CALL) { };

		HookedFunction functionName;
		SOCKET socket;
		std::vector<Buffer> buffers;
		int bufferCount;
		int ret;
		bool tunneled = false;
		int lastError = -1;

		void serializeToJson(json& j) override
		{
			IMessage::serializeToJson(j);
			to_json(j, *this);
		}

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(WSARecvFunctionCallMessage, functionName,
			socket,
			bufferCount,
			buffers,
			tunneled,
			ret,
			lastError);
	};

	struct InfoMessage : IMessage
	{
		InfoMessage(std::string info) : IMessage(UiMessageType::INFO_MESSAGE), infoMessage(info) { }

		std::string infoMessage;
		
		void serializeToJson(json& j) override
		{
			IMessage::serializeToJson(j);
			to_json(j, *this);
		}

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(InfoMessage, infoMessage);
	};

	// Only use for messages passed to extern SendMessageToLog
	struct ExternalMessage : IMessage
	{
		ExternalMessage(std::string info, std::string moduleName = "") : IMessage(UiMessageType::EXTERNAL_MESSAGE), externalMessage(info), moduleName(moduleName) { }

		std::string externalMessage;
		std::string moduleName;

		void serializeToJson(json& j) override
		{
			IMessage::serializeToJson(j);
			to_json(j, *this);
		}

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(ExternalMessage, externalMessage, moduleName);
	};

	struct ErrorMessage : IMessage
	{
		ErrorMessage(std::string errMsg) : IMessage(UiMessageType::ERROR_MESSAGE), errorMessage(errMsg) { }
		
		std::string errorMessage;

		void serializeToJson(json& j) override
		{
			IMessage::serializeToJson(j);
			to_json(j, *this);
		}
		
		NLOHMANN_DEFINE_TYPE_INTRUSIVE(ErrorMessage, errorMessage);
	};

	struct ErrorMessageResponse : IMessageResponse
	{
		ErrorMessageResponse(Guid jobId, std::string errMsg) 
			: IMessageResponse(UiMessageType::JOB_RESPONSE_ERROR, jobId), errorMessage(errMsg) { }

		std::string errorMessage;

		void serializeToJson(json& j) override
		{
			IMessageResponse::serializeToJson(j);
			to_json(j, *this);
		}

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(ErrorMessageResponse, jobId, errorMessage);
	};

	struct PongMessageResponse : IMessageResponse
	{
		PongMessageResponse(Guid jobId) : IMessageResponse(UiMessageType::JOB_RESPONSE_SUCCESS, jobId) { }

		std::string data = "PONG!";

		void serializeToJson(json& j) override
		{
			IMessageResponse::serializeToJson(j);
			to_json(j, *this);
		}

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(PongMessageResponse, jobId, data);
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

		void serializeToJson(json& j) override
		{
			IMessageResponse::serializeToJson(j);
			to_json(j, *this);
		}

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(SocketInfoResponse, jobId, addr, port, addrFamily, protocol);
	};

	struct HookedFunctionCallSocketMessage : IMessage
	{
		HookedFunctionCallSocketMessage() : IMessage(UiMessageType::HOOKED_FUNCTION_CALL) { };

		HookedFunction functionName;
		SOCKET socket;
		int ret;
		int lastError = -1;
		bool tunneling = false;

		uint16_t addrFamily = -1;
		std::string addr = "";
		uint16_t port = -1;

		void serializeToJson(json& j) override
		{
			IMessage::serializeToJson(j);
			to_json(j, *this);
		}

		void populateWithSockaddr(const sockaddr_storage* sockaddr)
		{
			int oldWsaErrorCode = WSAGetLastError();

			addrFamily = sockaddr->ss_family;

			if (sockaddr->ss_family == AF_INET)
			{
				const sockaddr_in* sa = reinterpret_cast<const sockaddr_in*>(sockaddr);

				char addr[INET_ADDRSTRLEN];
				int addrSize = sizeof(addr);
				int sinSize = sizeof(sockaddr_in);

				WSAAddressToStringA((LPSOCKADDR)sa, sinSize, NULL, addr, (LPDWORD)&addrSize);
				std::replace(addr, addr + sizeof(addr), ':', '\x00');
				this->addr = addr;
				port = ntohs(sa->sin_port);
			}
			else if (sockaddr->ss_family == AF_INET6)
			{
				const sockaddr_in6* sa = reinterpret_cast<const sockaddr_in6*>(sockaddr);

				char addr[INET6_ADDRSTRLEN];
				int addrSize = sizeof(addr);
				int sinSize = sizeof(sockaddr_in6);

				WSAAddressToStringA((LPSOCKADDR)sa, sinSize, NULL, addr, (LPDWORD)&addrSize);

				std::string address{ addr };
				auto colonPos = address.find_last_of(':');
				if (colonPos != std::string::npos)
					address.erase(colonPos);

				this->addr = address;
				port = ntohs(sa->sin6_port);
			}
			WSASetLastError(oldWsaErrorCode);

		}

		friend void to_json(json& j, const HookedFunctionCallSocketMessage& hfcm)
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


			to_json(j, (IMessage&)hfcm);
			j["functionName"] = hfcm.functionName;
			j["socket"] = hfcm.socket;
			j["ret"] = hfcm.ret;
			j["lastError"] = hfcm.lastError;
			if (hfcm.functionName != HookedFunction::CLOSE)
			{
				j["protocol"] = -1;
				j["addrFamily"] = hfcm.addrFamily;
				j["addr"] = hfcm.addr;
				j["port"] = hfcm.port;
				j["tunneling"] = hfcm.tunneling;

			}
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

		void serializeToJson(json& j) override
		{
			IMessageResponse::serializeToJson(j);
			to_json(j, *this);
		}

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(IsSocketWritableResponse, 
			jobId, writable, timedOut, error, lastError);
	};

	struct AddPacketFilterResponse : IMessageResponse
	{
		AddPacketFilterResponse(
			Guid jobId,
			Guid filterId
		) : IMessageResponse(UiMessageType::JOB_RESPONSE_SUCCESS, jobId),
			filterId(filterId) { }

		Guid filterId;

		void serializeToJson(json& j) override
		{
			IMessageResponse::serializeToJson(j);
			to_json(j, *this);
		}

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(AddPacketFilterResponse,
			jobId, filterId);
	};

	struct GenericPacketFilterResponse : IMessageResponse
	{
		GenericPacketFilterResponse(
			Guid jobId
		) : IMessageResponse(UiMessageType::JOB_RESPONSE_SUCCESS, jobId)
		{ }

		void serializeToJson(json& j) override
		{
			IMessageResponse::serializeToJson(j);
			to_json(j, *this);
		}

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(GenericPacketFilterResponse,
			jobId);
	};

	struct ConnectedSuccessMessage : IMessage
	{
		ConnectedSuccessMessage(std::string spyPipeServerName) : IMessage(UiMessageType::CONNECTED_SUCCESS), spyPipeServerName(spyPipeServerName) { }

		std::string spyPipeServerName;

		void serializeToJson(json& j) override
		{
			IMessage::serializeToJson(j);
			to_json(j, *this);
		}

		NLOHMANN_DEFINE_TYPE_INTRUSIVE(ConnectedSuccessMessage,
			spyPipeServerName);
	};
}