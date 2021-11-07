#include "functions.h"
#include "../application.h"

int WINAPI Functions::Hooked_Connect(SOCKET s, const sockaddr* name, int namelen)
{
    Application& app = Application::getInstance();
    auto* hookmgr = app.getHookManager();
    int ret = hookmgr->get_ofunction<connect>()(s, name, namelen);

    client::HookedFunctionCallSocketMessage hfcm;
    hfcm.functionName = HookedFunction::CONNECT;
    hfcm.socket = s;
    hfcm.addr = (sockaddr_in*)name;
    hfcm.ret = ret;

    if (ret == SOCKET_ERROR)
        hfcm.lastError = WSAGetLastError();

    app.sendToUI(hfcm);

    return ret;
}