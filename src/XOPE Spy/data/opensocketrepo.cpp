#include "opensocketrepo.h"

void OpenSocketsRepo::add(SOCKET socket)
{
	_socketsData.tryAdd(socket, {});
	_recvPacketsToInject.tryAdd(socket, {});
}

void OpenSocketsRepo::remove(SOCKET socket)
{
	_socketsData.tryRemove(socket);
	_recvPacketsToInject.tryRemove(socket);
}

bool OpenSocketsRepo::contains(SOCKET socket)
{
	return _socketsData.contains(socket);
}

void OpenSocketsRepo::markSocketAsTunnelled(SOCKET socket)
{
	_socketsData.tryUpdateValueFunc(socket, [](SocketData& sd)
		{
			sd.isTunneled = true;
			sd.socketIdSentToSink = false;
		});
}

void OpenSocketsRepo::stopTunnelingSocket(SOCKET socket)
{
	_socketsData.tryUpdateValueFunc(socket, [](SocketData& sd)
		{
			sd.isTunneled = false;
		});
}

bool OpenSocketsRepo::isSocketTunneled(SOCKET socket)
{
	return _socketsData.contains(socket) && _socketsData[socket].isTunneled;
}

bool OpenSocketsRepo::wasSocketIdSentToSink(SOCKET socket)
{
	return _socketsData[socket].socketIdSentToSink;
}

void OpenSocketsRepo::emitSocketIdSentToSink(SOCKET socket)
{
	_socketsData.tryUpdateValueFunc(socket, [](SocketData& sd)
		{
			sd.socketIdSentToSink = true;
		});
}

std::optional<bool> OpenSocketsRepo::isSocketNonBlocking(SOCKET socket)
{
	return _socketsData[socket].isBlocking;
}

void OpenSocketsRepo::setSocketToNonBlocking(SOCKET socket)
{
	_socketsData.tryUpdateValueFunc(socket, 
		[](SocketData& sd)
		{
			sd.socketIdSentToSink = true;
		});
}

void OpenSocketsRepo::setSocketIpVersion(SOCKET socket, int ipVersion)
{
	_socketsData.tryUpdateValueFunc(socket,
		[=](SocketData& sd)
		{
			sd.ipVersion = ipVersion;
		});
}

std::optional<int> OpenSocketsRepo::getSocketIpVersion(SOCKET socket)
{
	if (!_socketsData.contains(socket))
		return std::nullopt;

	return _socketsData[socket].ipVersion;
}


void OpenSocketsRepo::addRecvPacketToInject(SOCKET socket, const Packet& packet)
{
	if (!_recvPacketsToInject.contains(socket))
		_recvPacketsToInject.tryAdd(socket, std::queue<Packet>());

	_recvPacketsToInject.tryUpdateValueFunc(socket,
		[&](std::queue<Packet>& packets)
		{
			packets.push(packet);
		});
}

std::optional<Packet> OpenSocketsRepo::getNextRecvPacketToInject(SOCKET socket)
{
	if (!_recvPacketsToInject.contains(socket) || recvPacketsToInjectCount(socket) < 1)
		return std::nullopt;

	std::optional<Packet> nextRecvPacket = std::nullopt;

	_recvPacketsToInject.tryUpdateValueFunc(socket,
		[&](std::queue<Packet>& packetQueue)
		{
			if (packetQueue.empty())
				return;
			
			nextRecvPacket = std::move(packetQueue.front());
			packetQueue.pop();
		});

	return nextRecvPacket;
}

size_t OpenSocketsRepo::recvPacketsToInjectCount(SOCKET socket)
{
	if (!_recvPacketsToInject.contains(socket))
		return 0;

	return _recvPacketsToInject[socket].size();
}

void OpenSocketsRepo::clearInjectableRecvPackets(SOCKET socket)
{
	if (!_recvPacketsToInject.contains(socket))
		return;

	_recvPacketsToInject.tryUpdateValueFunc(socket,
		[=](std::queue<Packet>& packetQueue)
		{
			packetQueue = {};
		});
}