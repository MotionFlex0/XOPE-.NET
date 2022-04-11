#include "functions.h"
#include "../application.h"

int WSAAPI Functions::Hooked_WSARecv(SOCKET s, LPWSABUF lpBuffers, DWORD dwBufferCount, LPDWORD lpNumberOfBytesRecvd, LPDWORD lpFlags, LPWSAOVERLAPPED lpOverlapped, LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine)
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
        DWORD bytesRead = lpNumberOfBytesRecvd[i];
        Packet packet(lpBuffers[i].buf, lpBuffers[i].buf + bytesRead);
        bool modified = app.getPacketFilter().findAndReplace(FilterableFunction::WSARECV, s, packet);

        // Sends the original packet data to UI
        message.buffers.push_back({
            .length = bytesRead,
            .dataB64 = client::IMessage::convertBytesToB64String(lpBuffers[i].buf, bytesRead),
            .modified = modified
        });

        if (modified)
        {
            CHAR* oldBuf = lpBuffers[i].buf;
            CHAR* newBuf = new CHAR[packet.size()];
            memcpy(newBuf, packet.data(), packet.size());
            lpBuffers[i].buf = reinterpret_cast<CHAR*>(newBuf);
            lpBuffers[i].len = static_cast<ULONG>(packet.size());
            
            free(oldBuf);
        }
    }

    app.sendToUI(message);

    if (message.ret == SOCKET_ERROR && message.lastError != WSAEWOULDBLOCK)
        app.removeSocketFromSet(s);

    return ret;
}