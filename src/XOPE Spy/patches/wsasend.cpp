#include "functions.h"
#include "../application.h"

int WINAPI Functions::Hooked_WSASend(SOCKET s, LPWSABUF lpBuffers, DWORD dwBufferCount, LPDWORD lpNumberOfBytesSent, DWORD dwFlags, LPWSAOVERLAPPED lpOverlapped, LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine)
{
    Application& app = Application::getInstance();
    
    client::WSASendFunctionCallMessage message;
    message.functionName = HookedFunction::WSASEND;
    message.socket = s;
    message.bufferCount = dwBufferCount;
    
    // Required for the lifetime of WSASend(...)
    std::vector<Packet> modifiedPackets{ dwBufferCount };
    std::vector<WSABUF> updatedBuffers{ dwBufferCount };
    for (DWORD i = 0; i < dwBufferCount; i++)
    {
        Packet packet(lpBuffers[i].buf, lpBuffers[i].buf + lpBuffers[i].len);
        bool modified = app.getPacketFilter().findAndReplace(FilterableFunction::WSASEND, s, packet);
        
        if (modified)
        {
            modifiedPackets.push_back(std::move(packet));
            updatedBuffers[i].buf = reinterpret_cast<CHAR*>(modifiedPackets.back().data());
            updatedBuffers[i].len = static_cast<size_t>(modifiedPackets.back().size());
        }
        else
        {
            updatedBuffers[i].buf = lpBuffers[i].buf;
            updatedBuffers[i].len = lpBuffers[i].len;
        }
        
        message.buffers.push_back(
        { 
            .length = (size_t)lpBuffers[i].len,
            .dataB64 = client::IMessage::convertBytesToB64String(lpBuffers[i].buf, lpBuffers[i].len) 
        });
    }

    int ret = app.getHookManager()->get_ofunction<WSASend>()(s, updatedBuffers.data(), dwBufferCount, 
        lpNumberOfBytesSent, dwFlags, lpOverlapped, lpCompletionRoutine);
    
    message.ret = ret;
    for (DWORD i = 0; i < dwBufferCount; i++)
        message.buffers[i].bytesSent = lpNumberOfBytesSent[i];

    if (ret == SOCKET_ERROR)
        message.lastError = WSAGetLastError();

    app.sendToUI(message);

    return ret;
}