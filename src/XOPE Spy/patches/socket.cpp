#include "../application.h"
#include "functions.h"

SOCKET WSAAPI Functions:: Hooked_Socket(int af, int type, int protocol)
{
    Application& app = Application::getInstance();
    SOCKET ret = app.getHookManager()->get_ofunction<socket>()(af, type, protocol);

    if (ret != INVALID_SOCKET)
        app.setSocketIpVersion(ret, af);

    app.sendToUI(client::ErrorMessage("socket number is: " + std::to_string(ret) + " | af: " + std::to_string(af) +
        " | type: " + std::to_string(type) + " | protocol: " + std::to_string(protocol)));

    return ret;
}