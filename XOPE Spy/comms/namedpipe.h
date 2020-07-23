#pragma once
#include <algorithm>
#include <memory>
#include <Windows.h>
#include "definitions.hpp"
#include "../nlohmann/json.hpp"
#include <iostream>

//TODO: Change the folder name from 'comms' to something more descriptive

//https://stackoverflow.com/questions/3175219/restrict-c-template-parameter-to-subclass
template<class T, class B> struct Derived_from {
	static void constraints(T* p) { B* pb = p; }
	Derived_from() { void(*p)(T*) = constraints; }
};

class NamedPipe
{
public:

	//Should be run at the beginning.
	NamedPipe(const char* pipePath);

	bool isValid();

	//template <class T>
	//bool send(MessageType mt, T data)
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
	//bool send(MessageType mt, char data[], DWORD len);
	template <class T>
	bool send(T mes)
	{
		Derived_from < T, client::IMessage>();
		if (!isValid())
			return 0;

		DWORD bytesWritten{ 0 };
		json j = mes;
		std::string jsonAsStr = j.dump();
		int strLen = static_cast<int>(jsonAsStr.length());
		uint8_t* buffer = new uint8_t[strLen];

		memcpy(buffer, jsonAsStr.c_str(), strLen);
		WriteFile(_pipe, buffer, strLen, &bytesWritten, NULL);

		std::cout << j.dump(4) << std::endl;

		delete[] buffer;
		return strLen == bytesWritten;
	}
	
	uint8_t readOpcode();
	std::pair<unsigned long, std::unique_ptr<uint8_t>> readBytes(int len);
	//Should be called before the end (if program does not exit)
	~NamedPipe();

	void close();

	struct Message
	{
		MessageType type;
		char* data;
		int len;
	};

private:
	HANDLE _pipe = INVALID_HANDLE_VALUE;
};