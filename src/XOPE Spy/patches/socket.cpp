#include "../application.h"
#include "functions.h"

SOCKET WSAAPI Functions:: Hooked_Socket(int af, int type, int protocol)
{
    Application& app = Application::getInstance();
    SOCKET ret = app.getHookManager()->get_ofunction<socket>()(af, type, protocol);

    if (ret != INVALID_SOCKET)
        app.setSocketIpVersion(ret, af);

    return ret;
}