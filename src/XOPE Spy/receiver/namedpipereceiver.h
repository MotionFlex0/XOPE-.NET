#pragma once
#include <atomic>
#include <queue>
#include <sstream>
#include <vector>
#include "../definitions/definitions.h" // only definitions.h will import winsock2/windows to prevent redef errors
#include "../hook/hookmgr.hpp"
#include "../dispatcher/namedpipedispatcher.h"
#include "../packet/type.h"
#include "ireceiver.h"

class NamedPipeReceiver : IReceiver
{

public:
	NamedPipeReceiver(std::string pipeName);

	bool isPipeBroken() const;
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
	HANDLE _pipeServerThreadId = NULL;

};