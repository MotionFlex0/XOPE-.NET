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
	ConnectNamedPipe(_pipe, NULL);
	
	DWORD bytesRead{ 0 };
	DWORD bytesAvailable{ 0 };

	const int storageSize = 65536;
	char* storageBuf = new char[storageSize]{ 0 };
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
			if (ReadFile(_pipe, storageBuf, min(bytesAvailable, storageSize-1), &bytesRead, NULL))
			{
				*(storageBuf + bytesRead) = 0;
				json message = json::parse(storageBuf);

				SpyMessageType type = message["Type"].get<SpyMessageType>();

				std::lock_guard lock(_lock);
				_incomingMessages.push({ type, message });
			}
		}
		std::this_thread::sleep_for(std::chrono::milliseconds(500));
	}

	delete[] storageBuf;
	_pipeBroken = true;
}

void NamedPipeServer::shutdownServer()
{
	_stopServer = true;
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
