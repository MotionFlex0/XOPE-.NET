#include "functions.h"
#include "../application.h"

int WSAAPI Functions::Hooked_Recv(SOCKET s, char* buf, int len, int flags)
{
    Application& app = Application::getInstance();

    int bytesRead { 0 };

    //Loop runs once if socket is non-blocking or indifinately if it is blocking
    //do
    //{
    //    if (app.shouldSocketClose(s))
    //    {
    //        bytesRead = 0;
    //        break;
    //    }
    //    else if (app.recvPacketsToInjectCount(s) > 0)
    //    {
    //        Packet packet = *app.getNextRecvPacketToInject(s);
    //        size_t bytesToCopy = min(packet.size(), len);
    //        std::copy_n(packet.begin(), bytesToCopy, buf);
    //        bytesRead = bytesToCopy;
    //        break;
    //    }

    //    fd_set fds;
    //    FD_ZERO(&fds);
    //    FD_SET(s, &fds);

    //    TIMEVAL timeout;
    //    timeout.tv_sec = 0;
    //    timeout.tv_usec = 10000;

    //    int selectRet = select(0, &fds, nullptr, nullptr, &timeout);
    //    if (selectRet > 0)
    //    {
    //        bytesRead = app.getHookManager()->get_ofunction<recv>()(s, buf, len, flags);
    //        break;
    //    }

    //    if (app.isSocketNonBlocking(s))
    //    {
    //        bytesRead = SOCKET_ERROR;
    //        WSASetLastError(WSAEWOULDBLOCK);
    //        break;
    //    }

    //} while (true);

    bytesRead = app.getHookManager()->get_ofunction<recv>()(s, buf, len, flags);

        
    client::HookedFunctionCallPacketMessage hfcm;
    hfcm.functionName = HookedFunction::RECV;
    hfcm.socket = s;
    hfcm.packetLen = bytesRead;
    hfcm.ret = bytesRead;
    hfcm.tunneled = app.isSocketTunneled(s);

    if (bytesRead < 1)
    {
        if (bytesRead == SOCKET_ERROR)
        {
            hfcm.lastError = WSAGetLastError();
            if (hfcm.lastError != WSAEWOULDBLOCK)
                app.removeSocketFromSet(s);
        }
        app.sendToUI(std::move(hfcm));

        return bytesRead;
    }

    Packet packet(buf, buf + bytesRead);
    bool modified = app.getPacketFilter().findAndReplace(FilterableFunction::RECV, s, packet);
    
    hfcm.packetDataB64 = { buf, bytesRead };
    hfcm.modified = modified;
    app.sendToUI(std::move(hfcm));

    if (modified)
    {
        size_t newBytesRead = min(packet.size(), static_cast<size_t>(len));
        memcpy(buf, packet.data(), newBytesRead);
        return static_cast<int>(newBytesRead);
    }
    else
        return bytesRead;
}