#pragma once
#include <mutex>
#include <unordered_map>

#include "../definitions/socketdata.h"
#include "../utils/concurrent_unordered_map.h"

/// <summary>
/// Stores information about open sockets. Will auto-add socket if it does not exist but a flag is modified for that socket id
/// </summary>
class OpenSocketsRepo
{
public:
	void add(SOCKET socket);
	void remove(SOCKET socket);
	bool contains(SOCKET socket);

	void markSocketAsTunnelled(SOCKET socket);
	void stopTunnelingSocket(SOCKET socket);
	bool isSocketTunneled(SOCKET socket);

	bool wasSocketIdSentToSink(SOCKET socket);
	void emitSocketIdSentToSink(SOCKET socket);

	//returns std::nullopt if socket's 'connect' call was not intercepted
	std::optional<bool> isSocketNonBlocking(SOCKET socket);
	void setSocketToNonBlocking(SOCKET socket);

	std::optional<int> getSocketIpVersion(SOCKET socket);
	void setSocketIpVersion(SOCKET socket, int ipVersion);

	std::optional<Packet> getNextRecvPacketToInject(SOCKET socket);
	void addRecvPacketToInject(SOCKET socket, const Packet& packet);

	size_t recvPacketsToInjectCount(SOCKET socket);
	void clearInjectableRecvPackets(SOCKET socket);

private:
	ConcurrentUnorderedMap<SOCKET, SocketData> _socketsData;
	ConcurrentUnorderedMap<SOCKET, std::queue<Packet>> _recvPacketsToInject; // recv & WSARecv
};