//#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
#include <iostream>
#include <string>
#include <Winsock2.h>
#include <windows.h>
#include "hook/detour.h"
#include "hook/hookmgr.hpp"
#include "utils/base64.h"
#include "utils/definition.hpp" //TODO: Improve how this works
#include "pipe/namedpipe.h"

#pragma comment(lib, "ws2_32.lib") // Not needed if WSAGetLastError is removed

void InitHooks();
void UnhookAll();
void PipeRecvThread(LPVOID param);

int WINAPI Hooked_Connect(SOCKET, const sockaddr*, int);
int WINAPI Hooked_Send(SOCKET, const char*, int, int);
int WINAPI Hooked_Recv(SOCKET, char*, int, int);
int WINAPI Hooked_CloseSocket(SOCKET);
    
int WINAPI Hooked_WSAConnect(SOCKET, const sockaddr*, int, LPWSABUF, LPWSABUF, LPQOS, LPQOS);
int WINAPI Hooked_WSASend(SOCKET, LPWSABUF, DWORD, LPDWORD, DWORD, LPWSAOVERLAPPED, LPWSAOVERLAPPED_COMPLETION_ROUTINE);
int WINAPI Hooked_WSARecv(SOCKET, LPWSABUF, DWORD, LPDWORD, LPDWORD, LPWSAOVERLAPPED, LPWSAOVERLAPPED_COMPLETION_ROUTINE);

//MidFunctionDetour connectmfd;
//MidFunctionDetour sendmfd;
//MidFunctionDetour recvmfd;
//MidFunctionDetour closesocketmfd;

//Detour32* procaddressbt;

HANDLE childThread;
std::atomic<bool> shouldChildThreadExit = false;

HookManager* hookmgr;
NamedPipe* namedPipe;


BOOL APIENTRY DllMain( HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        InitHooks();
        break;
    case DLL_PROCESS_DETACH:
        UnhookAll();
        break;
    case DLL_THREAD_ATTACH:
        break;
    case DLL_THREAD_DETACH:
        break;
    }
    return TRUE;
}

#pragma warning (disable : 4996)
void InitHooks() 
{
    //AllocConsole();

    //FILE* fpstdin = stdin;
    //FILE* fpstdout = stdout;
    //FILE* fpstderr = stderr;

    //freopen_s(&fpstdin, "conin$", "r", stdin);
    //freopen_s(&fpstdout, "conout$", "w", stdout);
    //freopen_s(&fpstderr, "conout$", "w", stderr);

    std::cout.rdbuf(nullptr);

    std::cout << "Redirected" << std::endl;
    

    HookManager::HookedFuncArgs hmargs;
    hmargs.connect = Hooked_Connect;
    //hmargs.send = Hooked_Send;
    hmargs.recv = Hooked_Recv;
    hmargs.close = Hooked_CloseSocket;
    hmargs.wsaconnect = Hooked_WSAConnect;
    hmargs.wsasend = Hooked_WSASend;
    hmargs.wsarecv = Hooked_WSARecv;

    hookmgr = new HookManager();
    hookmgr->init(hmargs);

    hookmgr->hookNewFunction(&send, Hooked_Send, SENDPATCHSIZE);

    const char* pipePath = "\\\\.\\pipe\\xopespy";
    //pipePath += std::to_string(GetCurrentProcessId());
    namedPipe = new NamedPipe(pipePath);
    if (namedPipe->isValid())
    {
        std::cout << "successfully connected to pipe: " << pipePath << std::endl;
        namedPipe->send(client::ConnectedSuccessMessage());
    }
    else
        std::cout << "failed to find pipe." << std::endl;

    childThread = CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)PipeRecvThread, NULL, 0, NULL);
}

void UnhookAll()
{
    std::cout << "Waiting for child thread to exit." << std::endl;
    shouldChildThreadExit = true;
    WaitForSingleObject(childThread, 10000);
    std::cout << "Freeing from processes..." << std::endl;
    namedPipe->close();
    hookmgr->destroy();
    delete hookmgr;
    //fclose(stdin);
    //fclose(stdout);
    //fclose(stderr);
    
    /*if (FreeConsole() == 0)
        MessageBoxA(NULL, "Failed to free console!", "ERROR", MB_OK);*/
}

void PipeRecvThread(LPVOID param)
{
    json message;
    while (!shouldChildThreadExit)
    {    
        bool res = namedPipe->recv(message);
        if (res)
            MessageBoxA(NULL, message.dump(2).c_str(), "message dump", MB_OK);

        namedPipe->flushOutBuffer();
        Sleep(100);
    }
}

int WINAPI Hooked_Connect(SOCKET s, const sockaddr* name, int namelen)
{
    int res = hookmgr->oConnect(s, name, namelen);
    std::cout << "res: " << res << std::endl;
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

int WINAPI Hooked_Send(SOCKET s, const char* buf, int len, int flags)
{
    client::HookedFunctionCallPacketMessage hfcm;
    hfcm.functionName = HookedFunction::SEND;
    hfcm.socket = s;
    hfcm.packetDataB64 = client::IMessage::convertBytesToB64String(buf, len);
    hfcm.packetLen = len;
    namedPipe->send(hfcm);
    return hookmgr->oFunction<send>(s, buf, len, flags);
}

int WINAPI Hooked_Recv(SOCKET s, char* buf, int len, int flags)
{
    int bytesRead = hookmgr->oRecv(s, buf, len, flags);
    if (bytesRead != SOCKET_ERROR) 
    {
        std::cout << "SIZE IN BYTES OF RECV: " << bytesRead << " | buf size: " << len << std::endl;
        client::HookedFunctionCallPacketMessage hfcm;
        hfcm.functionName = HookedFunction::RECV;
        hfcm.socket = s;
        hfcm.packetDataB64 = client::IMessage::convertBytesToB64String(buf, bytesRead); //base64_encode(buf, len);
        hfcm.packetLen = bytesRead;
        namedPipe->send(hfcm);
    }
    return bytesRead;
}

int WINAPI Hooked_CloseSocket(SOCKET s)
{
    client::HookedFunctionCallSocketMessage hfcm;
    hfcm.functionName = HookedFunction::CLOSE;
    hfcm.socket = s;
    namedPipe->send(hfcm);
    return hookmgr->oClose(s);
}

int WINAPI Hooked_WSAConnect(SOCKET s, const sockaddr* name, int namelen, LPWSABUF lpCallerData, LPWSABUF lpCalleeData, LPQOS lpSQOS, LPQOS lpGQOS)
{
    int res = hookmgr->oWSAConnect(s, name, namelen, lpCallerData, lpCalleeData, lpSQOS, lpGQOS);
    
    //10035 returns as this would be blocking.
    //https://docs.microsoft.com/en-gb/windows/win32/api/mswsock/nc-mswsock-lpfn_connectex?redirectedfrom=MSDN
    //TODO: remove 10035 and all0ow the UI to ask if the socket is now established (SO_CONNECT_TIME)
    if (res == 0 || (res == -1 && WSAGetLastError() == 10035))
    {
        client::HookedFunctionCallSocketMessage hfcm;
        hfcm.functionName = HookedFunction::WSACONNECT;
        hfcm.socket = s;
        hfcm.addr = (sockaddr_in*)name;
        namedPipe->send(hfcm);
    }
    return res;
}

int WINAPI Hooked_WSASend(SOCKET s, LPWSABUF lpBuffers, DWORD dwBufferCount, LPDWORD lpNumberOfBytesSent, DWORD dwFlags, LPWSAOVERLAPPED lpOverlapped, LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine)
{
    client::HookedFunctionCallPacketMessage hfcm;
    hfcm.functionName = HookedFunction::WSASEND;
    hfcm.socket = s;
    hfcm.packetDataB64 = client::IMessage::convertBytesToB64String(lpBuffers[0].buf, lpBuffers[0].len);
    hfcm.packetLen = lpBuffers[0].len;
    namedPipe->send(hfcm);
    //namedPipe->send(ServerMessageType::HOOKED_FUNCTION_CALL, lpBuffers[0].len);
    //for (unsigned int i = 0; i < dwBufferCount; i++)
        //std::cout << "[Hooked_WSASend] buffer[" << i << "] buf = " << lpBuffers[i].buf << " | len = " << lpBuffers[i].len << std::endl;
    return hookmgr->oWSASend(s, lpBuffers, dwBufferCount, lpNumberOfBytesSent, dwFlags, lpOverlapped, lpCompletionRoutine);
}

int WINAPI Hooked_WSARecv(SOCKET s, LPWSABUF lpBuffers, DWORD dwBufferCount, LPDWORD lpNumberOfBytesRecvd, LPDWORD lpFlags, LPWSAOVERLAPPED lpOverlapped, LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine)
{
    client::HookedFunctionCallPacketMessage hfcm;
    hfcm.functionName = HookedFunction::WSARECV;
    hfcm.socket = s;
    hfcm.packetDataB64 = client::IMessage::convertBytesToB64String(lpBuffers[0].buf, lpBuffers[0].len);
    hfcm.packetLen = lpBuffers[0].len;
    namedPipe->send(hfcm);
    return hookmgr->oWSARecv(s, lpBuffers, dwBufferCount, lpNumberOfBytesRecvd, lpFlags, lpOverlapped, lpCompletionRoutine);;
}
