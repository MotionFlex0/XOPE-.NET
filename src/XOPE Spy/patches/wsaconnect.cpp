#include "functions.h"
#include "../application.h"

int WINAPI Functions::Hooked_WSAConnect(SOCKET s, const sockaddr* name, int namelen, LPWSABUF lpCallerData, LPWSABUF lpCalleeData, LPQOS lpSQOS, LPQOS lpGQOS)
{
    Application& app = Application::getInstance();

    //10035 returns as this would be blocking.
    //https://docs.microsoft.com/en-gb/windows/win32/api/mswsock/nc-mswsock-lpfn_connectex?redirectedfrom=MSDN
    //TODO: remove 10035 and all0ow the UI to ask if the socket is now established (SO_CONNECT_TIME)
    int ret = app.getHookManager()->get_ofunction<WSAConnect>()(s, name, namelen, lpCallerData, lpCalleeData, lpSQOS, lpGQOS);

    client::HookedFunctionCallSocketMessage hfcm;
    hfcm.functionName = HookedFunction::WSACONNECT;
    hfcm.socket = s;
    hfcm.sockaddr = (sockaddr_in*)name;
    hfcm.ret = ret;

    if (ret == SOCKET_ERROR)
        hfcm.lastError = WSAGetLastError();

    app.sendToUI(hfcm);

    return ret;
}