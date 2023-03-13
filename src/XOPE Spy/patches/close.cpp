#include "functions.h"
#include "../application.h"

int WSAAPI Functions::Hooked_CloseSocket(SOCKET s)
{
    Application& app = Application::getInstance();
    int ret = app.getHookManager()->get_ofunction<closesocket>()(s);

    app.getOpenSocketsRepo()->remove(s);


    dispatcher::HookedFunctionCallSocketMessage hfcm;
    hfcm.functionName = HookedFunction::CLOSE;
    hfcm.socket = s;
    hfcm.ret = ret;
    app.sendToUI(std::move(hfcm));
    return ret;
}