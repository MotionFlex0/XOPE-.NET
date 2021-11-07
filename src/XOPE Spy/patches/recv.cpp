#include "functions.h"
#include "../application.h"

int WINAPI Functions::Hooked_Recv(SOCKET s, char* buf, int len, int flags)
{
    Application& app = Application::getInstance();
    int bytesRead = app.getHookManager()->get_ofunction<recv>()(s, buf, len, flags);

    client::HookedFunctionCallPacketMessage hfcm;
    hfcm.functionName = HookedFunction::RECV;
    hfcm.socket = s;
    hfcm.packetDataB64 = client::IMessage::convertBytesToB64String(buf, bytesRead); //base64_encode(buf, len);
    hfcm.packetLen = bytesRead;
    hfcm.ret = bytesRead;

    if (bytesRead == SOCKET_ERROR)
        hfcm.lastError = WSAGetLastError();

    app.sendToUI(hfcm);

    return bytesRead;
}