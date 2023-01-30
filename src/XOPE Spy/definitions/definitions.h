#pragma once

#include <WinSock2.h>
#include <WS2tcpip.h>
#include <Windows.h>
#include "../nlohmann/json.hpp"
#include "../utils/base64.h"
#include "../utils/guid.h"
#include "../utils/assert.h"
#include "../utils/packetjsonwrapper.h"

#include "../receiver/spymessagetype.h"
#include "../dispatcher/uimessagetype.h"

#include "../dispatcher/type/impl/imessage.h"
#include "../dispatcher/type/impl/imessageresponse.h"
#include "../dispatcher/type/impl/imessagewithresponse.h"

#pragma warning(disable: 4996)

using nlohmann::json;

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

namespace dispatcher
{
	struct AddPacketFilterResponse : IMessageResponse
	{
		AddPacketFilterResponse(Guid jobId, Guid filterId);

		Guid filterId;

		void serializeToJson(json& j) override;
	};

	struct ConnectedSuccessMessage : IMessage 
	{
		ConnectedSuccessMessage(std::string spyPipeServerName);

		std::string spyPipeServerName;

		void serializeToJson(json& j) override;
	};
	
	struct ErrorMessage : IMessage
	{
		ErrorMessage(std::string errMsg);

		std::string errorMessage;

		void serializeToJson(json& j) override;
	};
	
	struct ErrorMessageResponse : IMessageResponse
	{
		ErrorMessageResponse(Guid jobId, std::string errMsg);

		std::string errorMessage;

		void serializeToJson(json& j) override;
	};

	// Only use for messages passed to extern SendMessageToLog
	struct ExternalMessage : IMessage 
	{
		ExternalMessage(std::string info, std::string moduleName = "");

		std::string externalMessage;
		std::string moduleName;

		void serializeToJson(json& j) override;
	};

	struct GenericPacketFilterResponse : IMessageResponse
	{
		GenericPacketFilterResponse(Guid jobId);

		void serializeToJson(json& j) override;
	};

	struct HookedFunctionCallPacketMessage : IMessage
	{
		HookedFunctionCallPacketMessage();

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

		void serializeToJson(json& j) override;
	};
	
	struct HookedFunctionCallSocketMessage : IMessage
	{
		HookedFunctionCallSocketMessage();

		HookedFunction functionName;
		SOCKET socket;
		int ret;
		int lastError = -1;
		bool tunneling = false;

		uint16_t addrFamily = -1;
		std::string addr = "";
		uint16_t port = -1;

		void serializeToJson(json& j) override;

		void populateWithSockaddr(const sockaddr_storage* sockaddr);
	};

	struct InfoMessage : IMessage
	{
		InfoMessage(std::string info);

		std::string infoMessage;

		void serializeToJson(json& j) override;
	};

	struct IsSocketWritableResponse : IMessageResponse 
	{
		IsSocketWritableResponse(
			Guid jobId,
			bool writable,
			bool timedOut = false,
			bool error = false,
			int lastError = -1
		);

		bool writable;
		bool timedOut = false;
		bool error = false;
		int lastError = -1;

		void serializeToJson(json& j) override;
	};

	struct PongMessageResponse : IMessageResponse
	{
		PongMessageResponse(Guid jobId);

		std::string data = "PONG!";

		void serializeToJson(json& j) override;
	};

	struct SocketInfoResponse : IMessageResponse
	{
		SocketInfoResponse(Guid jobId, std::string addr, int port, int addrFamily, int protocol);

		std::string addr;
		int port;
		int addrFamily;
		int protocol;

		void serializeToJson(json& j) override;
	};

	struct WSARecvFunctionCallMessage : IMessage
	{
		struct Buffer
		{
			size_t length;
			PacketDataJsonWrapper dataB64;
			bool modified = false;
			bool dropPacket = false;
		};

		WSARecvFunctionCallMessage();

		HookedFunction functionName;
		SOCKET socket;
		std::vector<Buffer> buffers;
		int bufferCount;
		int ret;
		bool tunneled = false;
		int lastError = -1;

		void serializeToJson(json& j) override;
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
		};

		WSASendFunctionCallMessage();

		HookedFunction functionName;
		SOCKET socket;
		std::vector<Buffer> buffers;
		int bufferCount;
		int ret;
		bool tunneled = false;
		int lastError = -1;

		void serializeToJson(json& j) override;
	};












}