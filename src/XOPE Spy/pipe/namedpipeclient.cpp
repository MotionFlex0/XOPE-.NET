#include "namedpipeclient.h"

NamedPipeClient::NamedPipeClient(const char* pipePath)
{
    HANDLE h = CreateFileA(pipePath, GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, NULL);
    if (h != INVALID_HANDLE_VALUE)
        this->_pipe = h;
    else
        MessageBoxA(NULL, "COULD NOT CONNECT TO PIPE", "ERROR", MB_OK);
}

NamedPipeClient::~NamedPipeClient()
{
    if (isValid())
        CloseHandle(_pipe);
}

void NamedPipeClient::flushOutBuffer()
{
    std::lock_guard<std::mutex> lock(_mutex);

    for (auto& out : _outBuffer)
    {
        DWORD bytesWritten { 0 };
        WriteFile(_pipe, out.data.get(), out.length, &bytesWritten, NULL);
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
}


bool NamedPipeClient::isValid()
{
    return _pipe != INVALID_HANDLE_VALUE;
}
