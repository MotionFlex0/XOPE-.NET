#include "namedpipeclient.h"

NamedPipeClient::NamedPipeClient(const char* pipePath)
{
    HANDLE h = CreateFileA(pipePath, GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, NULL);
    if (h == INVALID_HANDLE_VALUE)
        MessageBoxA(NULL, "COULD NOT CONNECT TO PIPE", "ERROR", MB_OK);
    this->_pipe = h;
}

NamedPipeClient::~NamedPipeClient()
{
    if (!isPipeBroken())
        CloseHandle(_pipe);
}

void NamedPipeClient::flushOutBuffer()
{
    if (isPipeBroken())
        return;

    std::lock_guard<std::mutex> lock(_mutex);

    for (auto& out : _outBuffer)
    {
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
    _outBuffer.clear();
}

//int NamedPipeClient::recv(json& recvData)
//{
//    DWORD bytesRead { 0 };
//    DWORD bytesAvailable { 0 };
//    char buffer[65535];
//    
//    BOOL ret = PeekNamedPipe(_pipe, NULL, NULL, NULL, &bytesAvailable, NULL);
//    if (!ret)
//    {
//        DWORD lastError = GetLastError();
//        if (lastError == ERROR_PIPE_NOT_CONNECTED 
//            || lastError == ERROR_BAD_PIPE)
//            return -1;
//    }
//
//    if (bytesAvailable > 0)
//    {
//        ReadFile(_pipe, &buffer, 2047, &bytesRead, NULL);
//        buffer[bytesRead] = 0; // to make sure the json string is a null-terminated string
//        recvData = json::parse(buffer);
//        return bytesRead;
//    }
//
//    return 0;
//}

void NamedPipeClient::close()
{
    CloseHandle(_pipe);
    _pipe = INVALID_HANDLE_VALUE;
}


bool NamedPipeClient::isPipeBroken()
{
    return _pipe == INVALID_HANDLE_VALUE || pipeBroken;
}
