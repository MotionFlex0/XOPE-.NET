#pragma once
#include <any>
#include <functional>
#include <sstream>
#include <unordered_map>
#include <typeindex>
#include <Winsock2.h>
#include <windows.h>
#include "../utils/definition.hpp"
#include "../utils/util.h"

class HookManager
{
public: 
	HookManager() { }

	~HookManager()
	{
		destroy();
	}

	void destroy() {
		if (!m_destroyed)
		{
			for (auto& [_, v] : m_hooks)
			{
				v.detour->unpatch();
				delete v.detour;
				v.detour = nullptr;
				v.oFunc = nullptr;
			}
			bool s = m_hooks.empty();

			m_destroyed = true;
		}
	}

	template <class T>
	bool hookNewFunction(T* func, T* hookedFunc, int patchSize)
	{
		if (m_hooks.contains((uintptr_t)func))
			return true;

		HookFuncData hookFuncData;
		hookFuncData.detour = new Detour(hookedFunc, func, patchSize);
		hookFuncData.oFunc = static_cast<T*>(hookFuncData.detour->patch());

		std::stringstream ss;
		ss << "Failed to hook function with type: " << typeid(func).name();
		x_assert(std::any_cast<T*>(hookFuncData.oFunc) != 0x00, ss.str().c_str());
		
		return (m_hooks.insert({ (uintptr_t)func, hookFuncData })).second;
	}

	//Returns original function _F with the correct type
	template<auto _F>
	auto get_ofunction()
	{
		auto func = _F;
		using _F_t = decltype(func);

		auto search = m_hooks.find((uintptr_t)_F);

		if (search == m_hooks.end())
		{
			char msg[1024];
			sprintf_s(msg, sizeof(msg), "Could not find hook for this function - Calling _F\nFunction Address: %p\nFunction Type: %s", _F, typeid(_F).name());
			MessageBoxA(NULL, msg, "Hook Missing - get_ofunction", MB_OK);
			return _F;
		}

		std::stringstream ss;
		ss << "call_ofunction _F did not match type of stored original function\n";
		ss << "\n_F_t type: " << typeid(_F_t).name() << "\n\nsearch->second.oFunc type: " << search->second.oFunc.type().name();
		x_assert(typeid(_F_t) == search->second.oFunc.type(), ss.str().c_str());

		return std::any_cast<_F_t>(search->second.oFunc);
	}

	//Forwards passed arguments to the original function _F
	template<auto _F, typename... Ts>
	auto call_ofunction(Ts&&... args)   //, class... Ts>
	{
		auto func = _F;
		using _F_t = decltype(func);
		auto search = m_hooks.find((uintptr_t)_F);
		
		if (search == m_hooks.end())
		{
			char msg[1024];
			sprintf_s(msg, sizeof(msg), "Could not find hook for this function - Calling _F\nFunction Address: %p\nFunction Type: %s", _F, typeid(_F).name());
			MessageBoxA(NULL, msg, "Hook Missing - call_ofunction", MB_OK);
			return _F(std::forward<Ts>(args)...);
		}
		
		std::stringstream ss;
		ss << "call_ofunction _F did not match type of stored original function\n";
		ss << "_F_t type: " << typeid(_F_t).name() << " | search->second.oFunc type: " << search->second.oFunc.type().name();
		x_assert(typeid(_F_t) == search->second.oFunc.type(), ss.str().c_str());

		return (std::any_cast<_F_t>(((*search).second).oFunc)(std::forward<Ts>(args)...));
	}

private:
	struct HookFuncData
	{
		Detour* detour;
		std::any oFunc;
	};

	bool m_destroyed = false;

	std::unordered_map<uintptr_t, HookFuncData> m_hooks;
};