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

    if (bytesRead == SOCKET_ERROR)
        hfcm.lastError = WSAGetLastError();
    else if (bytesRead > 0)
        hfcm.packetDataB64 = client::IMessage::convertBytesToB64String(buf, bytesRead);

    app.sendToUI(hfcm);

    return bytesRead;
}