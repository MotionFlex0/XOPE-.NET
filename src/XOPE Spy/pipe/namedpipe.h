#pragma once
#include <algorithm>
#include <iostream>
#include <memory>
#include <mutex>
#include <Windows.h>
#include "../comms/definitions.hpp"
#include "../nlohmann/json.hpp"

//TODO: Change the folder name from 'comms' to something more descriptive

//https://stackoverflow.com/questions/3175219/restrict-c-template-parameter-to-subclass
template<class T, class B> struct Derived_from {
	static void constraints(T* p) { B* pb = p; }
	Derived_from() { void(*p)(T*) = constraints; }
};

class NamedPipe
{
	struct OutMessage
	{
		std::unique_ptr<uint8_t[]> data;
		int length = 0;
	};

public:

	//Should be run at the beginning.
	NamedPipe(const char* pipePath);

	bool isValid();

	//template <class T>
	//bool send(ServerMessageType mt, T data)
	//{
	//	int len = sizeof(data) + 1;
	//	DWORD bytesWritten{ 0 };
	//	uint8_t* buffer = new uint8_t[len];
	//	buffer[0] = (uint8_t)mt;
	//	memcpy(buffer + 1, &data, len - 1);
	//	if (isValid())
	//		WriteFile(_pipe, buffer, len, &bytesWritten, NULL);
	//	return len == bytesWritten;
	//}
	//bool send(ServerMessageType mt, char data[], DWORD len);
	template <class T>
	bool send(T mes)
	{
		Derived_from < T, client::IMessage>();
		if (!isValid())
			return false;

		std::lock_guard<std::mutex> lock(_mutex);

		DWORD bytesWritten { 0 };
		json j = mes;

		std::vector<std::uint8_t> cbor = json::to_cbor(j);

		//OLD Write as string
		//std::string jsonAsStr = j.dump();
		int len = static_cast<int>(cbor.size());
		//uint8_t* buffer = new uint8_t[len];
		auto buffer = std::make_unique<uint8_t[]>(len);
		memcpy(buffer.get(), cbor.data(), len);

		_outBuffer.push_back({ .data = std::move(buffer), .length = len });
		//WriteFile(_pipe, buffer, strLen, &bytesWritten, NULL);

		//std::cout << j.dump(4) << std::endl;

		//delete[] buffer;
		return true;// strLen == bytesWritten;
	}
	
	uint8_t readOpcode();
	std::pair<unsigned long, std::unique_ptr<uint8_t>> readBytes(int len);
	//Should be called before the end (if program does not exit)
	~NamedPipe();

	void flushOutBuffer();
	int recv(json&);
	void close();



	//struct Message
	//{
	//	ServerMessageType type;
	//	char* data;
	//	int len;
	//};

private:

	HANDLE _pipe = INVALID_HANDLE_VALUE;
	std::vector<OutMessage> _outBuffer;
	std::mutex _mutex;


};