#include "functions.h"
#include "../application.h"

int WINAPI Functions::Hooked_WSARecv(SOCKET s, LPWSABUF lpBuffers, DWORD dwBufferCount, LPDWORD lpNumberOfBytesRecvd, LPDWORD lpFlags, LPWSAOVERLAPPED lpOverlapped, LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine)
{
    Application& app = Application::getInstance();
    int ret = app.getHookManager()->get_ofunction<WSARecv>()(s, lpBuffers, dwBufferCount, lpNumberOfBytesRecvd, lpFlags, lpOverlapped, lpCompletionRoutine);

    client::WSARecvFunctionCallMessage message;
    message.functionName = HookedFunction::WSARECV;
    message.socket = s;
    message.bufferCount = dwBufferCount;
    message.ret = ret;
    if (ret == SOCKET_ERROR)
        message.lastError = WSAGetLastError();

    for (DWORD i = 0; i < dwBufferCount; i++)
    {
        Packet packet(lpBuffers[i].buf, lpBuffers[i].buf + lpBuffers[i].len);
        bool modified = app.getPacketFilter().findAndReplace(FilterableFunction::WSARECV, s, packet);

        message.buffers.push_back({
            .length = (size_t)lpBuffers[i].len,
            .dataB64 = client::IMessage::convertBytesToB64String(lpBuffers[i].buf, lpBuffers[i].len),
            .modified = modified
        });

        if (modified)
        {
            CHAR* oldBuf = lpBuffers[i].buf;
            CHAR* newBuf = new CHAR[packet.size()];
            memcpy(newBuf, packet.data(), packet.size());
            lpBuffers[i].buf = reinterpret_cast<CHAR*>(newBuf);
            lpBuffers[i].len = static_cast<size_t>(packet.size());
            
            delete[] oldBuf;
        }
    }

    app.sendToUI(message);

    return ret;
}