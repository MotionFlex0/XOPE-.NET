#include "functions.h"
#include "../application.h"

int WSAAPI Functions::Hooked_Recv(SOCKET s, char* buf, int len, int flags)
{
    Application& app = Application::getInstance();

    Packet packet;
    int bytesRead { 0 };
    PacketFilter::ReplaceState replaceState;

    // BUG: Currently, it is only possible to inject packets after "recv" has return at least once
    //      when using a blocking socket. This will not be an issue for non-blocking sockets using 
    //      "select", as the readibility of the socket has been spoofed when a packet needs to be injecetd.         
    if (app.recvPacketsToInjectCount(s) > 0)
    {
        packet = *app.getNextRecvPacketToInject(s);
        size_t bytesToCopy = min(packet.size(), len);
        std::copy_n(packet.begin(), bytesToCopy, buf);
        bytesRead = static_cast<int>(bytesToCopy);
    }
    else
    {
        do
        {
            bytesRead = app.getHookManager()->get_ofunction<recv>()(s, buf, len, flags);
            if (bytesRead > 0)
            {
                packet.assign(buf, buf + bytesRead);
                replaceState = app.getPacketFilter().findAndReplace(FilterableFunction::RECV, s, packet);
                if (replaceState == PacketFilter::ReplaceState::DROP_PACKET) // If the packet has been dropped, then continue loop
                {
                    dispatcher::HookedFunctionCallPacketMessage hfcm;
                    hfcm.functionName = HookedFunction::RECV;
                    hfcm.socket = s;
                    hfcm.packetLen = bytesRead;
                    hfcm.ret = bytesRead;
                    hfcm.tunneled = app.isSocketTunneled(s);
                    hfcm.packetDataB64 = std::move(packet);
                    hfcm.modified = false;
                    hfcm.dropPacket = true;
                    app.sendToUI(std::move(hfcm));
                }
                else
                {
                    break;
                }
            }
        } while (bytesRead > 0); // If bytesRead == 0 (socket closed) or -1 (wsa error), end loop.
    }
        
    dispatcher::HookedFunctionCallPacketMessage hfcm;
    hfcm.functionName = HookedFunction::RECV;
    hfcm.socket = s;
    hfcm.packetLen = bytesRead;
    hfcm.ret = bytesRead;
    hfcm.tunneled = app.isSocketTunneled(s);

    if (bytesRead < 1)
    {
        if (bytesRead == SOCKET_ERROR)
        {
            hfcm.lastError = WSAGetLastError();
            if (hfcm.lastError != WSAEWOULDBLOCK)
                app.removeSocketFromSet(s);
        }
        app.sendToUI(std::move(hfcm));

        return bytesRead;
    }
    
    hfcm.packetDataB64 = { buf, bytesRead };
    hfcm.modified = replaceState == PacketFilter::ReplaceState::MODIFIED_PACKET;
    hfcm.dropPacket = replaceState == PacketFilter::ReplaceState::DROP_PACKET;
    app.sendToUI(std::move(hfcm));

    if (replaceState == PacketFilter::ReplaceState::MODIFIED_PACKET)
    {
        size_t newBytesRead = min(packet.size(), static_cast<size_t>(len));
        memcpy(buf, packet.data(), newBytesRead);
        return static_cast<int>(newBytesRead);
    }
    else
        return bytesRead;
}