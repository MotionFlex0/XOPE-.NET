#pragma once
#include <any>
#include <sstream>
#include <unordered_map>
#include <typeindex>
#include <Winsock2.h>
#include <windows.h>
#include "../utils/definition.hpp"

//Winsock 1.x
//typedef int (WINAPI* ConnectPtr_t)(SOCKET, const sockaddr*, int);
//typedef int (WINAPI* SendPtr_t)(SOCKET, const char*, int, int);
//typedef int (WINAPI* RecvPtr_t)(SOCKET, char*, int, int);
//typedef int (WINAPI* CloseSocketPtr_t)(SOCKET);

using ConnectPtr_t = decltype(&connect);
using SendPtr_t = decltype(&send);
using RecvPtr_t = decltype(&recv);
using CloseSocketPtr_t = decltype(&closesocket);

//Winsock 2.x
typedef int(WINAPI* WSAConnectPtr_t)(SOCKET, const sockaddr*, int, LPWSABUF, LPWSABUF, LPQOS, LPQOS);
typedef int (WINAPI* WSASendPtr_t)(SOCKET, LPWSABUF, DWORD, LPDWORD, DWORD, LPWSAOVERLAPPED, LPWSAOVERLAPPED_COMPLETION_ROUTINE);
typedef int (WINAPI* WSARecvPtr_t)(SOCKET, LPWSABUF, DWORD, LPDWORD, LPDWORD, LPWSAOVERLAPPED, LPWSAOVERLAPPED_COMPLETION_ROUTINE);
//CloseSocketPtr_t closes both 1.x and 2.x


//TODO: Fix up this class. It is very messy
class HookManager
{
public: 
	struct HookedFuncArgs
	{
		void* connect = nullptr;
		void* send = nullptr;
		void* recv = nullptr;
		void* close = nullptr;
		void* wsaconnect = nullptr;
		void* wsasend = nullptr;
		void* wsarecv = nullptr;
	};

	//TODO: Possibly move HookFuncArgs here and keep "init" as some sort of "InitHooks" method
	HookManager() { }

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

		if (hf.connect) connectdetour = new Detour(hf.connect, GetProcAddress(hWs2, "connect"), SENDPATCHSIZE);
		if (hf.send) senddetour = new Detour(hf.send, GetProcAddress(hWs2, "send"), SENDPATCHSIZE);
		if (hf.recv) recvdetour = new Detour(hf.recv, GetProcAddress(hWs2, "recv"), SENDPATCHSIZE);
		if (hf.close) closedetour = new Detour(hf.close, GetProcAddress(hWs2, "closesocket"), CLOSEPATCHSIZE);
		if (hf.wsaconnect) wsaconnectdetour = new Detour(hf.wsaconnect, GetProcAddress(hWs2, "WSAConnect"), SENDPATCHSIZE);
		if (hf.wsasend) wsasenddetour = new Detour(hf.wsasend, GetProcAddress(hWs2, "WSASend"), SENDPATCHSIZE);
		if (hf.wsarecv) wsarecvdetour = new Detour(hf.wsarecv, GetProcAddress(hWs2, "WSARecv"), SENDPATCHSIZE);

		if (hf.connect) _oConnect = (ConnectPtr_t)connectdetour->patch();
		if (hf.send) _oSend = (SendPtr_t)senddetour->patch();
		if (hf.recv) _oRecv = (RecvPtr_t)recvdetour->patch();
		if (hf.close) _oClose = (CloseSocketPtr_t)closedetour->patch();
		if (hf.wsaconnect) _oWSAConnect = (WSAConnectPtr_t)wsaconnectdetour->patch();
		if (hf.wsasend) _oWSASend = (WSASendPtr_t)wsasenddetour->patch();
		if (hf.wsarecv) _oWSARecv = (WSARecvPtr_t)wsarecvdetour->patch();
	}

	void destroy() {
		if (!destroyed)
		{
			if (connectdetour) connectdetour->unpatch();
			if (senddetour) senddetour->unpatch();
			if (recvdetour) recvdetour->unpatch();
			if (closedetour) closedetour->unpatch();
			if (wsaconnectdetour) wsaconnectdetour->unpatch();
			if (wsasenddetour) wsasenddetour->unpatch();
			if (wsarecvdetour) wsarecvdetour->unpatch();
			if (connectdetour) delete connectdetour;
			if (senddetour) delete senddetour;
			if (recvdetour) delete recvdetour;
			if (closedetour) delete closedetour;
			if (wsaconnectdetour) delete wsaconnectdetour;
			if (wsasenddetour) delete wsasenddetour;
			if (wsarecvdetour) delete wsarecvdetour;

			for (auto& [_, v] : _hooks)
			{
				v.detour->unpatch();
				delete v.detour;
				v.detour = nullptr;
				v.oFunc = nullptr;
			}
			bool s = _hooks.empty();

			destroyed = true;
		}
	}

	template <typename T>
	bool hookNewFunction(T* func, void* hookedFunc, int patchSize)
	{
		HookFuncData hookFuncData;
		hookFuncData.detour = new Detour(hookedFunc, func, patchSize);
		hookFuncData.oFunc = static_cast<T*>(hookFuncData.detour->patch());

		return (_hooks.insert({ (uintmax_t)func, hookFuncData })).second;
	}

	template<auto Func, typename... Ts>
	auto oFunction(Ts&&... args)
	{
		using Func_t = decltype(&Func);
		auto search = _hooks.find((uintmax_t)&Func);

		if (search == _hooks.end())
			return NULL;
		
		return (std::any_cast<Func_t>(((*search).second).oFunc)(std::forward<Ts>(args)...));
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

	struct HookFuncData
	{
		Detour* detour;
		std::any oFunc;
	};

	bool destroyed = false;

	std::unordered_map<uintmax_t, HookFuncData> _hooks;

	ConnectPtr_t _oConnect;
	SendPtr_t _oSend;
	RecvPtr_t _oRecv;
	CloseSocketPtr_t _oClose;
	WSAConnectPtr_t _oWSAConnect;
	WSASendPtr_t _oWSASend;
	WSARecvPtr_t _oWSARecv;

	Detour* connectdetour = nullptr;
	Detour* senddetour = nullptr;
	Detour* recvdetour = nullptr;
	Detour* closedetour = nullptr;
	Detour* wsaconnectdetour = nullptr;
	Detour* wsasenddetour = nullptr;
	Detour* wsarecvdetour = nullptr;

};