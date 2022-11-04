#pragma once

#include <algorithm>
#include <BS_thread_pool.hpp>
#include <concepts>
#include <iostream>
#include <memory>
#include <mutex>
#include <queue>

#include "../definitions/definitions.hpp"
#include "../nlohmann/json.hpp"
#include "../utils/util.h"

class NamedPipeClient
{
	struct OutMessage
	{
		std::unique_ptr<uint8_t[]> data;
		int length = 0;
	};

public:

	//Should be run at the beginning of the program.
	NamedPipeClient(const char* pipePath, std::shared_ptr<BS::thread_pool> pool);

	bool isPipeBroken();

	bool send(std::unique_ptr<client::IMessage> mes);
	
	void flushOutBuffer();
	void close();

	// Thread pool is enabled by default but when calls are made by the thread that calls 
	//  DllMain it needs to be disabled or else the program will deadlock
	void disableThreadPool(bool disable);

private:

	using OutMessage_P = std::unique_ptr<OutMessage>;

	bool pipeBroken = false;
	HANDLE _pipe = INVALID_HANDLE_VALUE;
	//std::queue<OutMessage> _outBuffer;
	std::queue <std::unique_ptr<client::IMessage>> _outBuffer;
	std::mutex _mutex;

	std::weak_ptr<BS::thread_pool> _pool;
	std::atomic<bool> _disablePool = false;

	OutMessage_P serializeMessage(std::unique_ptr<client::IMessage> message);
};
