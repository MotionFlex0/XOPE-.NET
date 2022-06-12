#include "functions.h"
#include "../application.h"

int WSAAPI Functions::Hooked_Send(SOCKET s, const char* buf, int len, int flags)
{
    Application& app = Application::getInstance();
    
    Packet packet(buf, buf + len);
    bool modified = app.getPacketFilter().findAndReplace(FilterableFunction::SEND, s, packet);
    
    int bytesSent{ 0 };
    if (app.shouldSocketClose(s))
    {
        bytesSent = SOCKET_ERROR;
        WSASetLastError(WSAECONNRESET);
    }
    else if (app.isSocketTunneled(s))
    {
        if (!app.wasSocketIdSentToSink(s))
        {
            int32_t s32 = s & 0xFFFFFFFF;
            app.getHookManager()->get_ofunction<send>()(s, (char*)&s32, sizeof(s32), NULL);
            app.socketIdSentToSink(s);
        }
        bytesSent = len;
    }
    else 
        bytesSent = app.getHookManager()->get_ofunction<send>()(s, (char*)packet.data(), static_cast<int>(packet.size()), flags);

    client::HookedFunctionCallPacketMessage hfcm;
    hfcm.functionName = HookedFunction::SEND;
    hfcm.socket = s;
    hfcm.packetLen = len;
    hfcm.modified = modified;
    hfcm.ret = bytesSent;

    if (bytesSent == SOCKET_ERROR)
        hfcm.lastError = WSAGetLastError();
    else if (bytesSent > 0)
        hfcm.packetDataB64 = client::IMessage::convertBytesToB64String(buf, len);

    app.sendToUI(hfcm);

    if (hfcm.ret == SOCKET_ERROR && hfcm.lastError != WSAEWOULDBLOCK)
        app.removeSocketFromSet(s);

    if (!modified || bytesSent < 1)
        return bytesSent;
    else
        return len;
}