#include "functions.h"
#include "../application.h"

int WINAPI Functions::Hooked_Send(SOCKET s, const char* buf, int len, int flags)
{
    Application& app = Application::getInstance();
    int ret = app.getHookManager()->get_ofunction<send>()(s, buf, len, flags);

    client::HookedFunctionCallPacketMessage hfcm;
    hfcm.functionName = HookedFunction::SEND;
    hfcm.socket = s;
    hfcm.packetDataB64 = client::IMessage::convertBytesToB64String(buf, len);
    hfcm.packetLen = len;
    hfcm.ret = ret;

    if (ret == SOCKET_ERROR)
        hfcm.lastError = WSAGetLastError();

    app.sendToUI(hfcm);

    return ret;
}