#include "functions.h"
#include "../application.h"

// TODO: Add support for packet injection into WSARecv function
// This function will merge the buffers into one before using the filter
int WSAAPI Functions::Hooked_WSARecv(SOCKET s, LPWSABUF lpBuffers, DWORD dwBufferCount, LPDWORD lpNumberOfBytesRecvd, LPDWORD lpFlags, LPWSAOVERLAPPED lpOverlapped, LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine)
{
    Application& app = Application::getInstance();

    int ret{ 0 };

    std::vector<std::pair<CHAR*, size_t>> updatedBuffers;
    std::unique_ptr<client::WSARecvFunctionCallMessage> message;

    do
    {
        ret = app.getHookManager()->get_ofunction<WSARecv>()(s, lpBuffers, dwBufferCount, lpNumberOfBytesRecvd, lpFlags, lpOverlapped, lpCompletionRoutine);

        DWORD bytesLeftToProcess = *lpNumberOfBytesRecvd;

        message.reset(new client::WSARecvFunctionCallMessage);
        message->functionName = HookedFunction::WSARECV;
        message->socket = s;
        message->bufferCount = dwBufferCount;
        message->ret = ret;
        message->tunneled = app.isSocketTunneled(s);
        if (ret == SOCKET_ERROR)
        {
            message->lastError = WSAGetLastError();
            break;
        }

        if (ret == 0 && bytesLeftToProcess > 0)
        {
            for (DWORD i = 0; i < dwBufferCount && bytesLeftToProcess > 0; i++)
            {
                DWORD bytesToRead = min(bytesLeftToProcess, lpBuffers[i].len);
                Packet packet(lpBuffers[i].buf, lpBuffers[i].buf + bytesToRead);
                PacketFilter::ReplaceState replaceState = app.getPacketFilter().findAndReplace(FilterableFunction::WSARECV, s, packet);

                // Sends the original packet data to UI
                message->buffers.push_back({
                    .length = bytesToRead,
                    .dataB64 = PacketDataJsonWrapper{ lpBuffers[i].buf, bytesToRead },
                    .modified = replaceState == PacketFilter::ReplaceState::MODIFIED_PACKET,
                    .dropPacket = replaceState == PacketFilter::ReplaceState::DROP_PACKET
                    });

                if (replaceState == PacketFilter::ReplaceState::NO_CHANGE)
                {
                    updatedBuffers.push_back({ lpBuffers[i].buf, bytesToRead });
                }
                else if (replaceState == PacketFilter::ReplaceState::MODIFIED_PACKET)
                {
                    DWORD maxWriittableDataLen = min(packet.size(), lpBuffers[i].len);
                    memcpy(lpBuffers[i].buf, packet.data(), maxWriittableDataLen);
                    updatedBuffers.push_back({ lpBuffers[i].buf, bytesToRead });
                }

                bytesLeftToProcess -= bytesToRead;

            }

            // If lpBuffers needs to be updated as all the packet have not been dropped, then break loop 
            if (updatedBuffers.size() > 0)
            {
                break;
            }
            else if (message->buffers.size() > 0)
            {
                app.sendToUI(std::move(*message));
            }

        }

    // Breaks if ret == -1, ret and bytesRecvd are both 0 or there is data in the buffer
    } while (!(ret == 0 && *lpNumberOfBytesRecvd == 0));


    for (int i = 0; i < updatedBuffers.size(); i++)
    {
        if (lpBuffers[i].buf != updatedBuffers[i].first)
        {
            auto& [buf, len] = updatedBuffers[i];
            lpBuffers[i].buf = buf;
            lpBuffers[i].len = len;

        }
    }

    app.sendToUI(std::move(*message));

    if (message->ret == SOCKET_ERROR && message->lastError != WSAEWOULDBLOCK)
        app.removeSocketFromSet(s);

    return ret;
}