#pragma once
#include <winsock2.h>
#include <windows.h>

namespace Functions {
	int WINAPI Hooked_Connect(SOCKET s, const sockaddr* name, int namelen);
	int WINAPI Hooked_Send(SOCKET s, const char* buf, int len, int flags);
	int WINAPI Hooked_Recv(SOCKET s, char* buf, int len, int flags);
	int WINAPI Hooked_CloseSocket(SOCKET s);
	int WINAPI Hooked_WSAConnect(SOCKET s, const sockaddr* name, int namelen, LPWSABUF lpCallerData, LPWSABUF lpCalleeData, LPQOS lpSQOS, LPQOS lpGQOS);
	int WINAPI Hooked_WSASend(SOCKET s, LPWSABUF lpBuffers, DWORD dwBufferCount, LPDWORD lpNumberOfBytesSent, DWORD dwFlags, LPWSAOVERLAPPED lpOverlapped, LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine);
	int WINAPI Hooked_WSARecv(SOCKET s, LPWSABUF lpBuffers, DWORD dwBufferCount, LPDWORD lpNumberOfBytesRecvd, LPDWORD lpFlags, LPWSAOVERLAPPED lpOverlapped, LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine);
}	