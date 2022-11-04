#include "namedpipeserver.h"

NamedPipeServer::NamedPipeServer(std::string pipeName)
{
	_pipe = CreateNamedPipeA(pipeName.c_str(), PIPE_ACCESS_DUPLEX, (PIPE_TYPE_BYTE | PIPE_READMODE_BYTE | PIPE_WAIT), 1, 1024 * 4, 65535, NMPWAIT_USE_DEFAULT_WAIT, NULL);
	if (_pipe == INVALID_HANDLE_VALUE)
	{
		_pipeBroken = true;
		return;
	}
}

bool NamedPipeServer::isPipeBroken()
{
	return _pipeBroken;
}

void NamedPipeServer::run()
{
	DWORD bytesRead{ 0 };
	DWORD bytesAvailable{ 0 };

	auto isEndOfJson = [](const char& a, const char& b) { return a == '}' && b == '\x00'; };

	const int storageSize = 65535;
	std::vector<char> storageBuf(storageSize, 0xFF);
	int offset = 0;

	BOOL connected = ConnectNamedPipe(_pipe, NULL);
	if (!connected)
	{
		MessageBoxA(NULL, "CANCELLED", "TEST", MB_OK);
		CloseHandle(_pipe);
		_stopServer = true;
		_pipeBroken = true;
		return;
	}

	while (!_stopServer)
	{
		if (!PeekNamedPipe(_pipe, NULL, NULL, NULL, &bytesAvailable, NULL))
		{
			DWORD lastError = GetLastError();
			if (lastError == ERROR_PIPE_NOT_CONNECTED || lastError == ERROR_BAD_PIPE)
				break;
		}

		if (bytesAvailable > 0)
		{
			if (ReadFile(_pipe, storageBuf.data()+offset, min(bytesAvailable, storageSize-1), &bytesRead, NULL))
			{
				auto dataEndIt = storageBuf.begin() + offset + bytesRead;
				auto endOfJsonIt = std::adjacent_find(storageBuf.begin() + offset, dataEndIt, isEndOfJson);
				if (endOfJsonIt == dataEndIt)
				{
					offset += bytesRead;
					continue;
				}
	
				do
				{
					try
					{
						json message = json::parse(storageBuf.data() + offset);

						SpyMessageType type = message["Type"].get<SpyMessageType>();
						std::lock_guard lock(_lock);
						_incomingMessages.push({ type, message });
					}
					catch (std::exception e)
					{
						// TODO: Send an error message to the UI
					}

					*(endOfJsonIt + 1) = '\xff'; // 0x00 -> 0xff so the sequence "}\x00" cannot be found again
					offset = (((endOfJsonIt+2) - storageBuf.begin()));

					if (endOfJsonIt == dataEndIt - 2)
						break;

					endOfJsonIt = std::adjacent_find(storageBuf.begin() + offset, dataEndIt, isEndOfJson);

				} while (endOfJsonIt < dataEndIt-1);

				if (endOfJsonIt == dataEndIt - 2)
					offset = 0;
			}
		}
		std::this_thread::sleep_for(std::chrono::milliseconds(1));
	}

	CloseHandle(_pipe);

	_pipeBroken = true;
}

void NamedPipeServer::shutdownServer()
{
	CancelSynchronousIo(GetCurrentThread());
	_stopServer = true;
	_pipeBroken = true;
}

std::optional<IncomingMessage> NamedPipeServer::getIncomingMessage()
{
    std::lock_guard lock(_lock);
    if (_incomingMessages.size() < 1)
        return std::nullopt;

    IncomingMessage top = _incomingMessages.front();
    _incomingMessages.pop();
    return top;
}
