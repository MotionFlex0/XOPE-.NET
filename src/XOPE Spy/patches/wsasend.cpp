#include "functions.h"
#include "../application.h"

int WSAAPI Functions::Hooked_WSASend(SOCKET s, LPWSABUF lpBuffers, DWORD dwBufferCount, LPDWORD lpNumberOfBytesSent, DWORD dwFlags, LPWSAOVERLAPPED lpOverlapped, LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine)
{
    Application& app = Application::getInstance();
    
    client::WSASendFunctionCallMessage message;
    message.functionName = HookedFunction::WSASEND;
    message.socket = s;
    message.bufferCount = dwBufferCount;
    message.tunneled = app.isSocketTunneled(s);
    
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
            updatedBuffers[i].len = static_cast<ULONG>(modifiedPackets.back().size());
        }
        else
        {
            updatedBuffers[i].buf = lpBuffers[i].buf;
            updatedBuffers[i].len = lpBuffers[i].len;
        }
        
        message.buffers.push_back(
        { 
            .length = (size_t)lpBuffers[i].len,
            .dataB64 = Packet(lpBuffers[i].buf, lpBuffers[i].buf+lpBuffers[i].len) 
        });
    }

    if (app.shouldSocketClose(s))
    {
        message.ret = SOCKET_ERROR;
        WSASetLastError(WSAECONNRESET);
    }
    //else if (app.isSocketTunneled(s))
    //{
    //    if (!app.wasSocketIdSentToSink(s))
    //    {
    //        int32_t s32 = s & 0xFFFFFFFF;
    //        app.getHookManager()->get_ofunction<send>()(s, (char*)&s32, sizeof(s32), NULL);
    //        app.emitSocketIdSentToSink(s);
    //    }

    //    message.ret = 0;
    //    for (DWORD i = 0; i < dwBufferCount; i++)
    //        lpNumberOfBytesSent[i] = lpBuffers[i].len;
    //}
    else
    {
        if (app.isSocketTunneled(s) && !app.wasSocketIdSentToSink(s))
        {
            int32_t s32 = s & 0xFFFFFFFF;
            app.getHookManager()->get_ofunction<send>()(s, (char*)&s32, sizeof(s32), NULL);
            app.emitSocketIdSentToSink(s);
        }

        message.ret = app.getHookManager()->get_ofunction<WSASend>()(s, updatedBuffers.data(), dwBufferCount, 
            lpNumberOfBytesSent, dwFlags, lpOverlapped, lpCompletionRoutine);
    }

    if (message.ret == 0)
    {
        for (DWORD i = 0; i < dwBufferCount; i++)
            message.buffers[i].bytesSent = lpNumberOfBytesSent[i];
    }
    else if (message.ret == SOCKET_ERROR)
        message.lastError = WSAGetLastError();

    app.sendToUI(std::move(message));

    if (message.ret == SOCKET_ERROR && message.lastError != WSAEWOULDBLOCK)
        app.removeSocketFromSet(s);

    return message.ret;
}