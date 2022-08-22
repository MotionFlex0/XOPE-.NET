#include "packetjsonwrapper.h"

PacketDataJsonWrapper::PacketDataJsonWrapper(const char* buf, int len)
{
	_packet = Packet(buf, buf+len);
}

PacketDataJsonWrapper::PacketDataJsonWrapper(Packet& packet)
{
	_packet = Packet(packet);
}

PacketDataJsonWrapper::PacketDataJsonWrapper(Packet&& packet)
{
	_packet = std::move(packet);
}

const Packet& PacketDataJsonWrapper::getPacket() const
{
	return _packet;
}