#pragma once
#include <optional>
#include <queue>
#include <WinSock2.h>
#include <WS2tcpip.h>
#include <Windows.h>
#include "../packet/type.h"

struct SocketData
{
	bool closeSocketGracefully = false;
	bool isBlocking = false;
	bool isTunneled = false;
	std::queue<Packet> recvPacketsToInject;
	int ipVersion = AF_INET;
	bool socketIdSentToSink = true;
};