#pragma once
#include <algorithm>
#include <iostream>
#include <memory>
#include <mutex>
#include "../definitions/definitions.hpp"
#include "../nlohmann/json.hpp"
#include "../utils/util.h"

//TODO: Change the folder name from 'comms' to something more descriptive

class NamedPipeClient
{
	struct OutMessage
	{
		std::unique_ptr<uint8_t[]> data;
		int length = 0;
	};

public:

	//Should be run at the beginning of the program.
	NamedPipeClient(const char* pipePath);
	~NamedPipeClient();

	bool isPipeBroken();

	template <class T>
	bool send(T mes);
	

	void flushOutBuffer();
	//int recv(json&);
	void close();

private:

	bool pipeBroken = false;
	HANDLE _pipe = INVALID_HANDLE_VALUE;
	std::vector<OutMessage> _outBuffer;
	std::mutex _mutex;
};

template <class T>
bool NamedPipeClient::send(T mes)
{
	Util::template Derived_from < T, client::IMessage>();
	if (isPipeBroken())
		return false;

	std::lock_guard<std::mutex> lock(_mutex);

	DWORD bytesWritten{ 0 };
	json j = mes;

	std::vector<std::uint8_t> cbor = json::to_cbor(j);

	int len = static_cast<int>(cbor.size());
	auto buffer = std::make_unique<uint8_t[]>(len);
	memcpy(buffer.get(), cbor.data(), len);

	_outBuffer.push_back({ .data = std::move(buffer), .length = len });

	return true;
}