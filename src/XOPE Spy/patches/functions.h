#pragma once
#include <winsock2.h>
#include <windows.h>

namespace Functions {
	int WSAAPI Hooked_Connect(SOCKET s, const sockaddr* name, int namelen);
	int WSAAPI Hooked_Send(SOCKET s, const char* buf, int len, int flags);
	int WSAAPI Hooked_Recv(SOCKET s, char* buf, int len, int flags);
	int WSAAPI Hooked_CloseSocket(SOCKET s);
	int WSAAPI Hooked_WSAConnect(SOCKET s, const sockaddr* name, int namelen, LPWSABUF lpCallerData, LPWSABUF lpCalleeData, LPQOS lpSQOS, LPQOS lpGQOS);
	int WSAAPI Hooked_WSASend(SOCKET s, LPWSABUF lpBuffers, DWORD dwBufferCount, LPDWORD lpNumberOfBytesSent, DWORD dwFlags, LPWSAOVERLAPPED lpOverlapped, LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine);
	int WSAAPI Hooked_WSARecv(SOCKET s, LPWSABUF lpBuffers, DWORD dwBufferCount, LPDWORD lpNumberOfBytesRecvd, LPDWORD lpFlags, LPWSAOVERLAPPED lpOverlapped, LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine);
	int WSAAPI Hooked_Select(int nfds, fd_set FAR* readfds, fd_set FAR* writefds, fd_set FAR* exceptfds, const struct timeval FAR* timeout);
	int WSAAPI Hooked_Ioctlsocket(SOCKET s, long cmd, u_long* argp);
	SOCKET WSAAPI Hooked_Socket(int af, int type, int protocol);
	SOCKET WSAAPI Hooked_WSASocketA(int af, int type, int protocol, LPWSAPROTOCOL_INFOA lpProtocolInfo, GROUP g, DWORD dwFlags);
	SOCKET WSAAPI Hooked_WSASocketW(int af, int type, int protocol, LPWSAPROTOCOL_INFOW lpProtocolInfo, GROUP g, DWORD dwFlags);
}	