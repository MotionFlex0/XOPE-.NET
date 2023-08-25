#include "functions.h"
#include "../application.h"
#include "../job/jobmessagetype.h"

int WSAAPI Functions::Hooked_Recv(SOCKET s, char* buf, int len, int flags)
{
    Application& app = Application::getInstance();

    Packet packet;
    int bytesRead { 0 };
    PacketFilter::ReplaceState replaceState;
    bool modified = false;
    bool dropPacket = false;
    bool intercepted = false;

    // BUG: Currently, it is only possible to inject packets after "recv" has return at least once
    //      when using a blocking socket. This will not be an issue for non-blocking sockets using 
    //      "select", as the readibility of the socket has been spoofed when a packet needs to be injecetd.         
    if (app.getOpenSocketsRepo()->recvPacketsToInjectCount(s) > 0)
    {
        packet = *app.getOpenSocketsRepo()->getNextRecvPacketToInject(s);
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
                replaceState = app.getPacketFilter()->findAndReplace(FilterableFunction::RECV, s, packet);
                modified = replaceState == PacketFilter::ReplaceState::MODIFIED_PACKET;
                dropPacket = replaceState == PacketFilter::ReplaceState::DROP_PACKET;

                if (!dropPacket && app.getConfig()->isInterceptorEnabled())
                {
                    IncomingMessage im = app
                        .sendToUI(dispatcher::InterceptorRequest(HookedFunction::RECV, s, packet))
                        ->wait();

                    if (im.type == SpyMessageType::JOB_RESPONSE_SUCCESS)
                    {
                        auto jobType = im.rawJsonData["JobResponseType"].get<SpyJobResponseType>();
                        if (jobType == SpyJobResponseType::INTERCEPTOR_FORWARD_PACKET)
                        {
                            std::string data = base64_decode(im.rawJsonData["Data"].get<std::string>());

                            if (data.length() == im.rawJsonData["Length"].get<int>())
                            {
                                packet.assign(data.begin(), data.end());
                                modified = true;
                            }
                        }
                        else if (jobType == SpyJobResponseType::INTERCEPTOR_DROP_PACKET)
                        {
                            dropPacket = true;
                        }
                        intercepted = true;
                    }
                }

                if (dropPacket) // If the packet has been dropped, then continue loop
                {
                    dispatcher::HookedFunctionCallPacketMessage hfcm;
                    hfcm.functionName = HookedFunction::RECV;
                    hfcm.socket = s;
                    hfcm.packetLen = bytesRead;
                    hfcm.ret = bytesRead;
                    hfcm.tunneled = app.getOpenSocketsRepo()->isSocketTunneled(s);
                    hfcm.packetDataB64 = std::move(packet);
                    hfcm.modified = false;
                    hfcm.dropPacket = true;
                    app.sendToUI(std::move(hfcm));
                    continue;
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
    hfcm.tunneled = app.getOpenSocketsRepo()->isSocketTunneled(s);

    if (bytesRead < 1)
    {
        if (bytesRead == SOCKET_ERROR)
        {
            hfcm.lastError = WSAGetLastError();
            if (hfcm.lastError != WSAEWOULDBLOCK)
                app.getOpenSocketsRepo()->remove(s);
        }
        app.sendToUI(std::move(hfcm));

        return bytesRead;
    }
    
    hfcm.packetDataB64 = { buf, bytesRead };
    hfcm.modified = modified;
    hfcm.dropPacket = dropPacket;
    app.sendToUI(std::move(hfcm));

    if (modified)
    {
        size_t newBytesRead = min(packet.size(), static_cast<size_t>(len));
        memcpy(buf, packet.data(), newBytesRead);
        return static_cast<int>(newBytesRead);
    }
    else
        return bytesRead;
}