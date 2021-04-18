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


class HookManager
{
public: 
	HookManager() { }

	~HookManager()
	{
		destroy();
	}

	void destroy() {
		if (!destroyed)
		{
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

private:

	struct HookFuncData
	{
		Detour* detour;
		std::any oFunc;
	};

	bool destroyed = false;

	std::unordered_map<uintmax_t, HookFuncData> _hooks;
};