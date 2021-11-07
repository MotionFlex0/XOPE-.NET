#include "functions.h"
#include "../application.h"

int WINAPI Functions::Hooked_WSASend(SOCKET s, LPWSABUF lpBuffers, DWORD dwBufferCount, LPDWORD lpNumberOfBytesSent, DWORD dwFlags, LPWSAOVERLAPPED lpOverlapped, LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine)
{
    Application& app = Application::getInstance();

    int ret = app.getHookManager()->get_ofunction<WSASend>()(s, lpBuffers, dwBufferCount, lpNumberOfBytesSent, dwFlags, lpOverlapped, lpCompletionRoutine);

    client::HookedFunctionCallPacketMessage hfcm;
    hfcm.functionName = HookedFunction::WSASEND;
    hfcm.socket = s;
    hfcm.packetDataB64 = client::IMessage::convertBytesToB64String(lpBuffers[0].buf, lpBuffers[0].len);
    hfcm.packetLen = lpBuffers[0].len;
    hfcm.ret = ret;
    app.sendToUI(hfcm);

    return ret;
}