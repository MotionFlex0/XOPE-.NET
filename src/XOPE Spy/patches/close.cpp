#include "functions.h"
#include "../application.h"

int WINAPI Functions::Hooked_CloseSocket(SOCKET s)
{
    Application& app = Application::getInstance();
    int ret = app.getHookManager()->get_ofunction<closesocket>()(s);

    client::HookedFunctionCallSocketMessage hfcm;
    hfcm.functionName = HookedFunction::CLOSE;
    hfcm.socket = s;
    hfcm.ret = ret;
    app.sendToUI(hfcm);
    return ret;
}