#include "functions.h"
#include "../application.h"
#include "../job/jobmessagetype.h"

int WSAAPI Functions::Hooked_Send(SOCKET s, const char* buf, int len, int flags)
{
    Application& app = Application::getInstance();
    
    Packet packet(buf, buf + len);
    PacketFilter::ReplaceState replaceState = app.getPacketFilter()->findAndReplace(FilterableFunction::SEND, s, packet);
    bool modified = replaceState == PacketFilter::ReplaceState::MODIFIED_PACKET;
    bool dropPacket = replaceState == PacketFilter::ReplaceState::DROP_PACKET;
    bool intercepted = false;
    
    int bytesSent{ 0 };
    if (app.getOpenSocketsRepo()->isSocketTunneled(s) && !app.getOpenSocketsRepo()->wasSocketIdSentToSink(s))
    {
        int32_t s32 = s & 0xFFFFFFFF;
        app.getHookManager()->get_ofunction<send>()(s, (char*)&s32, 4, NULL);
        app.getOpenSocketsRepo()->emitSocketIdSentToSink(s);
    }

    if (!dropPacket)
    {
        if (app.getConfig()->isInterceptorEnabled())
        {
            IncomingMessage im = app
                .sendToUI(dispatcher::InterceptorRequest(HookedFunction::SEND, s, packet))
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
            }
            intercepted = true;
        }

        if (!dropPacket)
            bytesSent = app.getHookManager()->get_ofunction<send>()(s, (char*)packet.data(), static_cast<int>(packet.size()), flags);
    }


    dispatcher::HookedFunctionCallPacketMessage hfcm;
    hfcm.functionName = HookedFunction::SEND;
    hfcm.socket = s;
    hfcm.packetLen = len;
    hfcm.modified = modified;
    hfcm.ret = bytesSent;
    hfcm.tunneled = app.getOpenSocketsRepo()->isSocketTunneled(s);
    hfcm.dropPacket = dropPacket;
    hfcm.intercepted = intercepted;

    if (bytesSent == SOCKET_ERROR)
        hfcm.lastError = WSAGetLastError();
    else if (bytesSent > 0)
        hfcm.packetDataB64 = { buf, len };

    app.sendToUI(std::move(hfcm));

    if (hfcm.ret == SOCKET_ERROR && hfcm.lastError != WSAEWOULDBLOCK)
        app.getOpenSocketsRepo()->remove(s);

    if (!dropPacket && (!modified || bytesSent < 1))
        return bytesSent;
    else
        return len;
}