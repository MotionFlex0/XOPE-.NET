#include "../../definitions/definitions.h"

namespace dispatcher
{
	HookedFunctionCallSocketMessage::HookedFunctionCallSocketMessage() : 
		IMessage(UiMessageType::HOOKED_FUNCTION_CALL) 
	{ 
	};

	void HookedFunctionCallSocketMessage::serializeToJson(json& j)
	{
		IMessage::serializeToJson(j);
		to_json(j, *this);
	}

	void HookedFunctionCallSocketMessage::populateWithSockaddr(const sockaddr_storage* sockaddr)
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

	inline void to_json(json& j, const HookedFunctionCallSocketMessage& hfcm)
	{
		/* json
		{
			functionName:int,
			socket:int,
			ret:int
			lastError:int,
			protocol:int, //TODO: currently not implemented. need to call getsockopt(..) to get the type
			port:int,
			addr:scr/byte,
			addrType:int,
			tunneling:bool
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