#include "../application.h"
#include "functions.h"

SOCKET WSAAPI Functions::Hooked_WSASocketW(int af, int type, int protocol, LPWSAPROTOCOL_INFOW lpProtocolInfo, GROUP g, DWORD dwFlags)
{
    Application& app = Application::getInstance();
    SOCKET ret = app.getHookManager()->get_ofunction<WSASocketW>()(af, type, protocol, lpProtocolInfo, g, dwFlags);

    if (ret != INVALID_SOCKET)
        app.setSocketIpVersion(ret, af);

    return ret;
}