#include <Windows.h>

int WINAPI Hooked_Connect(SOCKET s, const sockaddr* name, int namelen)
{
    int res = hookmgr->get_ofunction<connect>()(s, name, namelen);
    if (res == 0)
    {
        client::HookedFunctionCallSocketMessage hfcm;
        hfcm.functionName = HookedFunction::CONNECT;
        hfcm.socket = s;
        hfcm.addr = (sockaddr_in*)name;
        namedPipe->send(hfcm);
    }
    return res;
}