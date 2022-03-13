#pragma once
#include <atomic>
#include <queue>
#include <sstream>
#include <vector>
#include "../definitions/definitions.hpp" // only definitions.hpp will import winsock2/windows to prevent redef errors
#include "../hook/hookmgr.hpp"
#include "../pipe/namedpipeclient.h"
#include "../packet/type.h"
#include "iserver.h"

class NamedPipeServer : IServer
{

public:
	NamedPipeServer(std::string pipeName);

	bool isPipeBroken();
	void run();
	void shutdownServer();

	// Inherited via IServer
	virtual std::optional<IncomingMessage> getIncomingMessage() override;

private:
	HANDLE _pipe;

	std::atomic_bool _stopServer = false;
	std::atomic_bool _pipeBroken = false;

	std::mutex _lock;
	std::queue<IncomingMessage> _incomingMessages;

};