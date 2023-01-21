#include "functions.h"
#include "../application.h"

int WSAAPI Functions::Hooked_WSASend(SOCKET s, LPWSABUF lpBuffers, DWORD dwBufferCount, LPDWORD lpNumberOfBytesSent, DWORD dwFlags, LPWSAOVERLAPPED lpOverlapped, LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine)
{
    Application& app = Application::getInstance();
    
    client::WSASendFunctionCallMessage hookCallMessage;
    hookCallMessage.functionName = HookedFunction::WSASEND;
    hookCallMessage.socket = s;
    hookCallMessage.bufferCount = dwBufferCount;
    hookCallMessage.tunneled = app.isSocketTunneled(s);
    
    // Required for the lifetime of WSASend(...)
    int updatedBufferCount{ 0 };
    std::vector<Packet> modifiedPackets{ dwBufferCount };
    std::vector<WSABUF> updatedBuffers{ dwBufferCount };
    for (DWORD i = 0; i < dwBufferCount; i++)
    {
        Packet packet(lpBuffers[i].buf, lpBuffers[i].buf + lpBuffers[i].len);
        PacketFilter::ReplaceState replaceState = app.getPacketFilter().findAndReplace(FilterableFunction::WSASEND, s, packet);

        hookCallMessage.buffers.push_back(
        {
            .length = (size_t)lpBuffers[i].len,
            .dataB64 = Packet(lpBuffers[i].buf, lpBuffers[i].buf + lpBuffers[i].len),
            .modified = replaceState == PacketFilter::ReplaceState::MODIFIED_PACKET,
            .dropPacket = replaceState == PacketFilter::ReplaceState::DROP_PACKET
        });

        if (replaceState == PacketFilter::ReplaceState::DROP_PACKET)
            continue;

        if (replaceState == PacketFilter::ReplaceState::MODIFIED_PACKET)
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

        updatedBufferCount++;
    }

    if (app.shouldSocketClose(s))
    {
        hookCallMessage.ret = SOCKET_ERROR;
        WSASetLastError(WSAECONNRESET);
    }
    else
    {
        if (app.isSocketTunneled(s) && !app.wasSocketIdSentToSink(s))
        {
            int32_t s32 = s & 0xFFFFFFFF;
            app.getHookManager()->get_ofunction<send>()(s, (char*)&s32, sizeof(s32), NULL);
            app.emitSocketIdSentToSink(s);
        }

        // updatedBuffers.size() is not necessarily equal to updatedBufferCount as some packets may have been dropped
        hookCallMessage.ret = app.getHookManager()->get_ofunction<WSASend>()(s, updatedBuffers.data(), updatedBufferCount, 
            lpNumberOfBytesSent, dwFlags, lpOverlapped, lpCompletionRoutine);
    }

    if (hookCallMessage.ret == 0)
    {
        int bytesSentIndex = 0;
        for (DWORD i = 0; i < dwBufferCount; i++)
            hookCallMessage.buffers[i].bytesSent = hookCallMessage.buffers[i].dropPacket ? 0 : lpNumberOfBytesSent[bytesSentIndex++];

        // When packets have been dropped, the length and index of values in lpNumberOfBytesSent will not match
        //  as less packets was sent. So we have to update lpNumberOfBytesSent by copying the values from hookCallMessage.buffers[..].bytesSent
        if (bytesSentIndex != dwBufferCount)
        {
            for (DWORD i = 0; i < dwBufferCount; i++)
            {
                if (hookCallMessage.buffers[i].modified || hookCallMessage.buffers[i].dropPacket)
                    lpNumberOfBytesSent[i] = hookCallMessage.buffers[i].length;
                else
                    lpNumberOfBytesSent[i] = hookCallMessage.buffers[i].bytesSent;
            }
        }
    }
    else if (hookCallMessage.ret == SOCKET_ERROR)
        hookCallMessage.lastError = WSAGetLastError();

    app.sendToUI(std::move(hookCallMessage));

    if (hookCallMessage.ret == SOCKET_ERROR && hookCallMessage.lastError != WSAEWOULDBLOCK)
        app.removeSocketFromSet(s);

    return hookCallMessage.ret;
}