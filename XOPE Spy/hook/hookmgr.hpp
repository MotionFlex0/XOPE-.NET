#pragma once
#include <Winsock2.h>
#include <windows.h>
#include "../utils/definition.hpp"

//Winsock 1.x
typedef int (WINAPI* ConnectPtr_t)(SOCKET, const sockaddr*, int);
typedef int (WINAPI* SendPtr_t)(SOCKET, const char*, int, int);
typedef int (WINAPI* RecvPtr_t)(SOCKET, char*, int, int);
typedef int (WINAPI* CloseSocketPtr_t)(SOCKET);

//Winsock 2.x
typedef int(WINAPI* WSAConnectPtr_t)(SOCKET, const sockaddr*, int, LPWSABUF, LPWSABUF, LPQOS, LPQOS);
typedef int (WINAPI* WSASendPtr_t)(SOCKET, LPWSABUF, DWORD, LPDWORD, DWORD, LPWSAOVERLAPPED, LPWSAOVERLAPPED_COMPLETION_ROUTINE);
typedef int (WINAPI* WSARecvPtr_t)(SOCKET, LPWSABUF, DWORD, LPDWORD, LPDWORD, LPWSAOVERLAPPED, LPWSAOVERLAPPED_COMPLETION_ROUTINE);
//CloseSocketPtr_t closes both 1.x and 2.x

class HookManager
{
public: 
	struct HookedFuncArgs
	{
		void* connect;
		void* send;
		void* recv;
		void* close;
		void* wsaconnect;
		void* wsasend;
		void* wsarecv;
	};

	HookManager() 
	{ }

	~HookManager()
	{
		destroy();
	}

	void init(HookedFuncArgs hf) {
		HMODULE hWs2 = GetModuleHandleA("ws2_32.dll");
		if (hWs2 == NULL)
		{
			destroyed = true;
			return;
		}

		//connectdetour = new DETOUR(hf.connect, GetProcAddress(hWs2, "connect"), SENDPATCHSIZE);
		//senddetour = new DETOUR(hf.send, GetProcAddress(hWs2, "send"), SENDPATCHSIZE);
		//recvdetour = new DETOUR(hf.recv, GetProcAddress(hWs2, "recv"), SENDPATCHSIZE);
		closedetour = new DETOUR(hf.close, GetProcAddress(hWs2, "closesocket"), CLOSEPATCHSIZE);
		wsaconnectdetour = new DETOUR(hf.wsaconnect, GetProcAddress(hWs2, "WSAConnect"), SENDPATCHSIZE);
		wsasenddetour = new DETOUR(hf.wsasend, GetProcAddress(hWs2, "WSASend"), SENDPATCHSIZE);
		wsarecvdetour = new DETOUR(hf.wsarecv, GetProcAddress(hWs2, "WSARecv"), SENDPATCHSIZE);

		//_oConnect = (ConnectPtr_t)connectdetour->patch();
		//_oSend = (SendPtr_t)senddetour->patch();
		//_oRecv = (RecvPtr_t)recvdetour->patch();
		_oClose = (CloseSocketPtr_t)closedetour->patch();
		_oWSAConnect = (WSAConnectPtr_t)wsaconnectdetour->patch();
		_oWSASend = (WSASendPtr_t)wsasenddetour->patch();
		_oWSARecv = (WSARecvPtr_t)wsarecvdetour->patch();
	}

	void destroy() {
		if (!destroyed)
		{
			connectdetour->unpatch();
			senddetour->unpatch();
			recvdetour->unpatch();
			closedetour->unpatch();
			wsaconnectdetour->unpatch();
			wsasenddetour->unpatch();
			wsarecvdetour->unpatch();
			delete connectdetour;
			delete senddetour;
			delete recvdetour;
			delete closedetour;
			delete wsaconnectdetour;
			delete wsasenddetour;
			delete wsarecvdetour;
			destroyed = true;
		}
	}

	template<typename... Ts>
	const int oConnect(Ts&&... args) {
		return _oConnect(std::forward<Ts>(args)...);
	}

	template<typename... Ts>
	const int oSend(Ts&&... args) {
		return _oSend(std::forward<Ts>(args)...);
	}

	template<typename... Ts>
	const int oRecv(Ts&&... args) {
		return _oRecv(std::forward<Ts>(args)...);
	}

	template<typename... Ts>
	const int oClose(Ts&&... args) {
		return _oClose(std::forward<Ts>(args)...);
	}

	template<typename... Ts>
	const int oWSAConnect(Ts&&... args) {
		return _oWSAConnect(std::forward<Ts>(args)...);
	}

	template<typename... Ts>
	const int oWSASend(Ts&&... args) {
		return _oWSASend(std::forward<Ts>(args)...);
	}

	template<typename... Ts>
	const int oWSARecv(Ts&&... args) {
		return _oWSARecv(std::forward<Ts>(args)...);
	}

private:
	bool destroyed = false;

	ConnectPtr_t _oConnect;
	SendPtr_t _oSend;
	RecvPtr_t _oRecv;
	CloseSocketPtr_t _oClose;
	WSAConnectPtr_t _oWSAConnect;
	WSASendPtr_t _oWSASend;
	WSARecvPtr_t _oWSARecv;

	DETOUR* connectdetour;
	DETOUR* senddetour;
	DETOUR* recvdetour;
	DETOUR* closedetour;
	DETOUR* wsaconnectdetour;
	DETOUR* wsasenddetour;
	DETOUR* wsarecvdetour;
};