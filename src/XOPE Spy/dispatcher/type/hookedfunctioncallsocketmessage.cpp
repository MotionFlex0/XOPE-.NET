#include "../../definitions/definitions.h"
#include "../../utils/stringconverter.h"

namespace dispatcher
{
	HookedFunctionCallSocketMessage::HookedFunctionCallSocketMessage() : 
		IMessage(UiMessageType::HOOKED_FUNCTION_CALL) 
	{ 
	};

	void HookedFunctionCallSocketMessage::populateWithSockaddr(SOCKET s, const sockaddr_storage* destSaStor)
	{
		int oldWsaErrorCode = WSAGetLastError();

		// TODO: Will not work as connect has yet to be called
		sockaddr_storage sourceSaStor;
		int sourceSaStorSize = sizeof(sourceSaStor);
		int sourceRet = getsockname(s, (sockaddr*)&sourceSaStor, &sourceSaStorSize);
		int error = WSAGetLastError();

		this->addrFamily = destSaStor->ss_family;

		if (destSaStor->ss_family == AF_INET)
		{
			const sockaddr_in* destSin = reinterpret_cast<const sockaddr_in*>(destSaStor);
			sockaddr_in* sourceSin = reinterpret_cast<sockaddr_in*>(&sourceSaStor);

			if (sourceRet == 0)
			{
				this->sourceAddr = StringConverter::IpAddressV4ToString(sourceSin);
				this->sourcePort = ntohs(sourceSin->sin_port);
				this->destAddr = StringConverter::IpAddressV4ToString(destSin);
				this->destPort = ntohs(destSin->sin_port);
			}
		}
		else if (destSaStor->ss_family == AF_INET6)
		{
			const sockaddr_in6* destSin = reinterpret_cast<const sockaddr_in6*>(destSaStor);
			sockaddr_in6* sourceSin = reinterpret_cast<sockaddr_in6*>(&sourceSaStor);

			if (sourceRet == 0)
			{
				this->sourceAddr = StringConverter::IpAddressV6ToString(sourceSin);
				this->sourcePort = ntohs(sourceSin->sin6_port);
				this->destAddr = StringConverter::IpAddressV6ToString(destSin);
				this->destPort = ntohs(destSin->sin6_port);
			}
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
			sourcePort:int,
			sourceAddr:string,
			destPort:int,
			destAddr:strung,
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
			j["sourceAddr"] = hfcm.sourceAddr;
			j["sourcePort"] = hfcm.sourcePort;			
			j["destAddr"] = hfcm.destAddr;
			j["destPort"] = hfcm.destPort;
			j["tunneling"] = hfcm.tunneling;
		}
	}

	void HookedFunctionCallSocketMessage::serializeToJson(json& j)
	{
		IMessage::serializeToJson(j);
		to_json(j, *this);
	}
};