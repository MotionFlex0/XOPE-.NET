#pragma once

#include <zlib.h>
#include "assert.h"
#include "base64.h"
#include "../nlohmann/json.hpp"
#include "../packet/type.h"


class PacketDataJsonWrapper
{
public:
	PacketDataJsonWrapper() { }
	PacketDataJsonWrapper(const char* buf, int len);
	PacketDataJsonWrapper(Packet& packet);
	PacketDataJsonWrapper(Packet&& packet);

	//PacketDataJsonWrapper(PacketDataJsonWrapper&) = delete;

	const Packet& getPacket() const;

private:
	Packet _packet;
};

namespace nlohmann
{
	template<>
	struct adl_serializer<PacketDataJsonWrapper>
	{
		static PacketDataJsonWrapper from_json(const json& j)
		{
			std::string decodedPacket = base64_decode(j.get<std::string>());
			uLong decodedPacketLen = static_cast<uLong>(decodedPacket.size());

			Packet uncompressedBuf(32768);
			uLong uncompressedBufSize = static_cast<uLong>(uncompressedBuf.size());
			int err = uncompress(uncompressedBuf.data(), &uncompressedBufSize, reinterpret_cast<uint8_t*>(decodedPacket.data()), decodedPacketLen);
			x_assert(err == Z_OK, "failed to compress bytes in adl_serializer<PacketDataJsonWrapper>::from_json");

			return uncompressedBuf;
		}

		static void from_json(const json& j, PacketDataJsonWrapper& packet)
		{
			std::string p = j.get<std::string>();
			if (p.size() < 1)
				return;

			std::string decodedPacket = base64_decode(p);
			uLong decodedPacketLen = static_cast<uLong>(decodedPacket.size());

			Packet uncompressedBuf(32768);
			uLong uncompressedBufSize = static_cast<uLong>(uncompressedBuf.size());
			int err = uncompress(uncompressedBuf.data(), &uncompressedBufSize, reinterpret_cast<uint8_t*>(decodedPacket.data()), decodedPacketLen);
			x_assert(err == Z_OK, "failed to compress bytes in adl_serializer<PacketDataJsonWrapper>::from_json");
			
			packet = std::move(uncompressedBuf);
		}

		static void to_json(json& j, const PacketDataJsonWrapper& packetWrapper)
		{	
			if (packetWrapper.getPacket().size() < 1)
			{
				j = std::string{};
				return;
			}

			const uint8_t* packetBuf = packetWrapper.getPacket().data();
			uLong packetBufLen = static_cast<uLong>(packetWrapper.getPacket().size());

			std::vector<Bytef> compressedBuf(32768);
			uLong compressedBufSize = static_cast<uLong>(compressedBuf.size());
			int err = compress(compressedBuf.data(), &compressedBufSize, packetBuf, packetBufLen);
			x_assert(err == Z_OK, "failed to compress bytes in adl_serializer<PacketDataJsonWrapper>::to_json");

			j = base64_encode(std::string(reinterpret_cast<const char*>(compressedBuf.data()), compressedBufSize));
		}
	};
}