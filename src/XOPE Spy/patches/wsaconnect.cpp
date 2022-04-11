#include <Ws2tcpip.h>
#include "functions.h"
#include "../application.h"

//10035 returns as this would be blocking.
//https://docs.microsoft.com/en-gb/windows/win32/api/mswsock/nc-mswsock-lpfn_connectex?redirectedfrom=MSDN
//TODO: remove 10035 and all0ow the UI to ask if the socket is now established (SO_CONNECT_TIME)
int WSAAPI Functions::Hooked_WSAConnect(SOCKET s, const sockaddr* name, int namelen, LPWSABUF lpCallerData, LPWSABUF lpCalleeData, LPQOS lpSQOS, LPQOS lpGQOS)
{
    Application& app = Application::getInstance();

    client::HookedFunctionCallSocketMessage hfcm;
    hfcm.functionName = HookedFunction::WSACONNECT;
    hfcm.socket = s;
    hfcm.sockaddr = reinterpret_cast<const sockaddr_storage*>(name);

    int port{ 0 };
    if (hfcm.sockaddr->ss_family == AF_INET)
        port = ntohs(reinterpret_cast<const sockaddr_in*>(hfcm.sockaddr)->sin_port);
    else if (hfcm.sockaddr->ss_family == AF_INET6)
        port = ntohs(reinterpret_cast<const sockaddr_in6*>(hfcm.sockaddr)->sin6_port);

    if (app.isTunnelingEnabled() && app.isPortTunnelable(port))
    {
        app.startTunnelingSocket(s);

        // This will connect to the UI sink instead of the real server
        sockaddr_storage sinkService;
        if (app.getSocketIpVersion(s) == AF_INET6)
        {
            sockaddr_in6* originalSockAddr = (sockaddr_in6*)name;

            sockaddr_in6* sink = (sockaddr_in6*)&sinkService;
            sink->sin6_family = AF_INET6;
            inet_pton(AF_INET6, "::1", &sink->sin6_addr);
            sink->sin6_port = htons(10102);
            sink->sin6_scope_id = originalSockAddr->sin6_scope_id;
            sink->sin6_flowinfo = originalSockAddr->sin6_flowinfo;
            sink->sin6_scope_struct = originalSockAddr->sin6_scope_struct;
        }
        else
        {
            sockaddr_in* sink = (sockaddr_in*)&sinkService;
            sink->sin_family = AF_INET;
            sink->sin_addr.s_addr = inet_addr("127.0.0.1");
            sink->sin_port = htons(10101);
        }

        hfcm.ret = app.getHookManager()->get_ofunction<WSAConnect>()
            (s, (SOCKADDR*)&sinkService, sizeof(sinkService), lpCallerData, lpCalleeData, lpSQOS, lpGQOS);

        hfcm.tunneling = true;
    }
    else
        hfcm.ret = app.getHookManager()->get_ofunction<WSAConnect>()
            (s, name, namelen, lpCallerData, lpCalleeData, lpSQOS, lpGQOS);


    if (hfcm.ret == SOCKET_ERROR)
        hfcm.lastError = WSAGetLastError();

    app.sendToUI(hfcm);

    return hfcm.ret;
}