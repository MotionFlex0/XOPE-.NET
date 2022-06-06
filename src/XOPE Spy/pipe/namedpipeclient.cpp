#include "namedpipeclient.h"

NamedPipeClient::NamedPipeClient(const char* pipePath)
{
    HANDLE h = CreateFileA(pipePath, GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, NULL);
    this->_pipe = h;
}

void NamedPipeClient::flushOutBuffer()
{
    if (isPipeBroken() || _outBuffer.empty())
        return;

    std::unique_lock<std::mutex> outBufferLock(_mutex, std::defer_lock);
    while (!_outBuffer.empty())
    {
        outBufferLock.lock();
        OutMessage out = std::move(_outBuffer.front());
        _outBuffer.pop();
        outBufferLock.unlock();

        DWORD bytesWritten { 0 };
        BOOL res = WriteFile(_pipe, out.data.get(), out.length, &bytesWritten, NULL);
        if (!res)
        {
            DWORD lastError = GetLastError();
            if (lastError == ERROR_PIPE_NOT_CONNECTED || lastError == ERROR_BAD_PIPE)
            {
                pipeBroken = true;
                return;
            }
        }
    }
}

void NamedPipeClient::close()
{
    if (isPipeBroken())
        return;

    CloseHandle(_pipe);
    _pipe = INVALID_HANDLE_VALUE;
}


bool NamedPipeClient::isPipeBroken()
{
    return _pipe == INVALID_HANDLE_VALUE || pipeBroken;
}
