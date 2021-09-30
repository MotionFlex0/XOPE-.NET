//#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
#include <iostream>
#include <queue>
#include <string>
#include <Winsock2.h>
#include <windows.h>
#include "hook/detour.h"
#include "hook/hookmgr.hpp"
#include "nlohmann/json.hpp"
#include "utils/base64.h"
#include "utils/definition.hpp" //TODO: Improve how this works
#include "pipe/namedpipe.h"
#include "packet/filter.h"

#pragma comment(lib, "ws2_32.lib") // Not needed if WSAGetLastError is removed

void InitHooks(HMODULE);
void UnhookAll();
void PipeThread(LPVOID param);

int WINAPI Hooked_Connect(SOCKET, const sockaddr*, int);
int WINAPI Hooked_Send(SOCKET, const char*, int, int);
int WINAPI Hooked_Recv(SOCKET, char*, int, int);
int WINAPI Hooked_CloseSocket(SOCKET);
    
int WINAPI Hooked_WSAConnect(SOCKET, const sockaddr*, int, LPWSABUF, LPWSABUF, LPQOS, LPQOS);
int WINAPI Hooked_WSASend(SOCKET, LPWSABUF, DWORD, LPDWORD, DWORD, LPWSAOVERLAPPED, LPWSAOVERLAPPED_COMPLETION_ROUTINE);
int WINAPI Hooked_WSARecv(SOCKET, LPWSABUF, DWORD, LPDWORD, LPDWORD, LPWSAOVERLAPPED, LPWSAOVERLAPPED_COMPLETION_ROUTINE);

HANDLE childThread;
std::atomic<bool> shouldChildThreadExit = false;

HookManager* hookmgr;
NamedPipe* namedPipe;

PacketFilter sendPacketFilter;

BOOL APIENTRY DllMain( HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        InitHooks(hModule);
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
void InitHooks(HMODULE module) 
{
    //Shows console for debugging
    //AllocConsole();

    //FILE* fpstdin = stdin;
    //FILE* fpstdout = stdout;
    //FILE* fpstderr = stderr;

    //freopen_s(&fpstdin, "conin$", "r", stdin);
    //freopen_s(&fpstdout, "conout$", "w", stdout);
    //freopen_s(&fpstderr, "conout$", "w", stderr);

    //Redirects stdout/stderror to nothing
    std::cout.rdbuf(nullptr); 

    std::cout << "Redirected" << std::endl;

    hookmgr = new HookManager();

    hookmgr->hookNewFunction(connect, Hooked_Connect, DEFAULTPATCHSIZE);
    hookmgr->hookNewFunction(send, Hooked_Send, DEFAULTPATCHSIZE);
    hookmgr->hookNewFunction(recv, Hooked_Recv, DEFAULTPATCHSIZE);
    //hookmgr->hookNewFunction(closesocket, Hooked_CloseSocket, CLOSEPATCHSIZE);
    hookmgr->hookNewFunction(WSAConnect, Hooked_WSAConnect, DEFAULTPATCHSIZE);
    hookmgr->hookNewFunction(WSASend, Hooked_WSASend, DEFAULTPATCHSIZE);
    hookmgr->hookNewFunction(WSARecv, Hooked_WSARecv, DEFAULTPATCHSIZE);

    const char* pipePath = "\\\\.\\pipe\\xopespy";

    // TODO: Temporary fix to the server not being started by the time the pipe connection is mad
    //Sleep(2000); 

    namedPipe = new NamedPipe(pipePath);
    if (namedPipe->isValid())
    {
        std::cout << "successfully connected to pipe: " << pipePath << '\n';
        namedPipe->send(client::ConnectedSuccessMessage());
    }
    else
        MessageBoxA(NULL, "Failed to connect to named pipe!", "ERROR", MB_OK);
        //std::cout << "failed to find pipe." << '\n';

    childThread = CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)PipeThread, module, 0, NULL);
}

void UnhookAll()
{
    std::cout << "Waiting for child thread to exit.\n";
    shouldChildThreadExit = true;
    WaitForSingleObject(childThread, 10000);
    std::cout << "Freeing from processes...\n";
    hookmgr->destroy();
    namedPipe->close();
    delete hookmgr;
    delete namedPipe;

    // Shows console for debugging
    //fclose(stdin);
    //fclose(stdout);
    //fclose(stderr);
    //
    //if (FreeConsole() == 0)
    //    MessageBoxA(NULL, "Failed to free console!", "ERROR", MB_OK);
}

void PipeThread(LPVOID module)
{
    json message;
    bool shouldFreeLibrary = false;
    while (!shouldChildThreadExit)
    {    
        int res = namedPipe->recv(message);
        if (res > 0)
        {
            SpyMessageType type = message["Type"].get<SpyMessageType>();
           
            if (type == SpyMessageType::PING)
            {
                namedPipe->send(client::PongMessage(message["JobId"].get<std::string>()));
            }
            else if (type == SpyMessageType::INJECT_SEND)
            {
                SOCKET socket = message["SocketId"].get<SOCKET>();
                std::string data = base64_decode(message["Data"].get<std::string>());

                if (data.length() == message["Length"].get<int>())
                {
                    hookmgr->get_ofunction<send>()(socket, data.c_str(), static_cast<int>(data.length()), NULL);
                }
                else
                {
                    namedPipe->send(client::ErrorMessage("INJECT_SEND packet size mismatch"));
                }
            }
            else if (type == SpyMessageType::INJECT_RECV)
            {
                SOCKET socket = message["SocketId"].get<SOCKET>();
                std::string data = base64_decode(message["Data"].get<std::string>());

                if (data.length() == message["Length"].get<int>())
                {
                    //hookmgr->get_ofunction<send>()(socket, data.c_str(), data.length(), NULL);
                }
                else
                {
                    namedPipe->send(client::ErrorMessage("INJECT_RECV packet size mismatch"));
                }
            }
            else if (type == SpyMessageType::ADD_SEND_FITLER)
            {
                const std::string oldValue = base64_decode(message["OldValue"].get<std::string>());
                const std::string newValue = base64_decode(message["NewValue"].get<std::string>());

                const Packet oldPacket(oldValue.begin(), oldValue.end());
                const Packet newPacket(newValue.begin(), newValue.end());


                boost::uuids::uuid id = sendPacketFilter.add(oldPacket, newPacket, true);
                
            }
            else if (type == SpyMessageType::SHUTDOWN_RECV_THREAD)
            {
                shouldChildThreadExit = true; // No longer necessary

                break;
            }
        }
        else if (res == -1)
        {
            shouldChildThreadExit = true;
            shouldFreeLibrary = true;
            break;
        }

        namedPipe->flushOutBuffer();

        Sleep(20);
    }

    if (shouldFreeLibrary)
        FreeLibraryAndExitThread((HMODULE)module, 0);
}

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

int WINAPI Hooked_Send(SOCKET s, const char* buf, int len, int flags)
{
    client::HookedFunctionCallPacketMessage hfcm;
    hfcm.functionName = HookedFunction::SEND;
    hfcm.socket = s;
    hfcm.packetDataB64 = client::IMessage::convertBytesToB64String(buf, len);
    hfcm.packetLen = len;
    namedPipe->send(hfcm);

    return hookmgr->get_ofunction<send>()(s, buf, len, flags);
}

int WINAPI Hooked_Recv(SOCKET s, char* buf, int len, int flags)
{
    int bytesRead = hookmgr->get_ofunction<recv>()(s, buf, len, flags);
    if (bytesRead != SOCKET_ERROR) 
    {
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
    return hookmgr->get_ofunction<closesocket>()(s);
}

int WINAPI Hooked_WSAConnect(SOCKET s, const sockaddr* name, int namelen, LPWSABUF lpCallerData, LPWSABUF lpCalleeData, LPQOS lpSQOS, LPQOS lpGQOS)
{
    int res = hookmgr->get_ofunction<WSAConnect>()(s, name, namelen, lpCallerData, lpCalleeData, lpSQOS, lpGQOS);
    
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
    return hookmgr->get_ofunction<WSASend>()(s, lpBuffers, dwBufferCount, lpNumberOfBytesSent, dwFlags, lpOverlapped, lpCompletionRoutine);
}

int WINAPI Hooked_WSARecv(SOCKET s, LPWSABUF lpBuffers, DWORD dwBufferCount, LPDWORD lpNumberOfBytesRecvd, LPDWORD lpFlags, LPWSAOVERLAPPED lpOverlapped, LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine)
{
    client::HookedFunctionCallPacketMessage hfcm;
    hfcm.functionName = HookedFunction::WSARECV;
    hfcm.socket = s;
    hfcm.packetDataB64 = client::IMessage::convertBytesToB64String(lpBuffers[0].buf, lpBuffers[0].len);
    hfcm.packetLen = lpBuffers[0].len;
    namedPipe->send(hfcm);
    return hookmgr->get_ofunction<WSARecv>()(s, lpBuffers, dwBufferCount, lpNumberOfBytesRecvd, lpFlags, lpOverlapped, lpCompletionRoutine);;
}
