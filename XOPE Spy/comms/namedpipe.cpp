#include "namedpipe.h"

NamedPipe::NamedPipe(const char* pipePath)
{
    HANDLE h = CreateFileA(pipePath, GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, NULL);
    if (h != INVALID_HANDLE_VALUE)
        this->_pipe = h;
}

NamedPipe::~NamedPipe()
{
    if (isValid())
        CloseHandle(_pipe);
}

void NamedPipe::close()
{
    CloseHandle(_pipe);
}

//bool NamedPipe::send(MessageType mt, char data[], DWORD len)
//{
//    DWORD bytesWritten{ 0 };
//    len += 1;
//    uint8_t* buffer = new uint8_t[len];
//    buffer[0] = (uint8_t)mt;
//    memcpy(buffer + 1, data, len - 1);
//    if (isValid())
//        WriteFile(_pipe, buffer, len, &bytesWritten, NULL);
//    delete buffer;
//    return len == bytesWritten;
//}
    //This is a blocking function
uint8_t NamedPipe::readOpcode()
{
    char opcode{ -1 };
    DWORD bytesRead{ 0 };
    ReadFile(_pipe, &opcode, 1, &bytesRead, NULL);
    return bytesRead & 0xFF;
}

std::pair<unsigned long, std::unique_ptr<uint8_t>> NamedPipe::readBytes(int len)
{
    DWORD bytesRead{ 0 };
    std::unique_ptr<uint8_t> buf = std::make_unique<uint8_t>(len);
    ReadFile(_pipe, buf.get(), len, &bytesRead, NULL);
    return std::make_pair(bytesRead, std::move(buf));
}

bool NamedPipe::isValid()
{
    return _pipe != INVALID_HANDLE_VALUE;
}
