#include "functions.h"
#include "../application.h"

int WINAPI Functions::Hooked_Send(SOCKET s, const char* buf, int len, int flags)
{
    Application& app = Application::getInstance();
    
    Packet packet(buf, buf + len);
    bool modified = app.getPacketFilter().findAndReplace(FilterableFunction::SEND, s, packet);
    
    int ret = app.getHookManager()->get_ofunction<send>()(s, (char*)packet.data(), packet.size(), flags);

    client::HookedFunctionCallPacketMessage hfcm;
    hfcm.functionName = HookedFunction::SEND;
    hfcm.socket = s;
    hfcm.packetLen = len;
    hfcm.modified = modified;
    hfcm.ret = ret;

    if (ret == SOCKET_ERROR)
        hfcm.lastError = WSAGetLastError();
    else if (ret > 0)
        hfcm.packetDataB64 = client::IMessage::convertBytesToB64String(buf, len);

    app.sendToUI(hfcm);

    if (!modified || ret < 1)
        return ret;
    else
        return len;
}