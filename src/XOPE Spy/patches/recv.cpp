#include "functions.h"
#include "../application.h"

int WINAPI Functions::Hooked_Recv(SOCKET s, char* buf, int len, int flags)
{
    Application& app = Application::getInstance();

    int bytesRead = app.getHookManager()->get_ofunction<recv>()(s, buf, len, flags);

    client::HookedFunctionCallPacketMessage hfcm;
    hfcm.functionName = HookedFunction::RECV;
    hfcm.socket = s;
    hfcm.packetLen = bytesRead;
    hfcm.ret = bytesRead;

    if (bytesRead < 1)
    {
        if (bytesRead == SOCKET_ERROR)
            hfcm.lastError = WSAGetLastError();
        app.sendToUI(hfcm);
        return bytesRead;
    }


    Packet packet(buf, buf + bytesRead);
    bool modified = app.getPacketFilter().findAndReplace(FilterableFunction::RECV, s, packet);
    
    hfcm.packetDataB64 = client::IMessage::convertBytesToB64String(buf, bytesRead);
    hfcm.modified = modified;
    app.sendToUI(hfcm);

    if (modified)
    {
        int newBytesRead = min(packet.size(), len);
        memcpy(buf, packet.data(), newBytesRead);
        return newBytesRead;
    }
    else
        return bytesRead;
}