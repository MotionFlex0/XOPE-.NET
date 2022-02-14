#pragma once
#include <any>
#include <functional>
#include <iostream>
#include <sstream>
#include <unordered_map>
#include <typeindex>
#include <intrin.h>
#include "../utils/definition.hpp"
#include "../utils/util.h"

struct IHookedFuncWrapper;

template <class F>
struct OriginalFuncWrapper;

template <class R, class... Args>
struct OriginalFuncWrapper<R(__cdecl *)(Args...)>
{
	using Type = R(__cdecl*)(Args...);
	
	OriginalFuncWrapper(Type lf)
	{
		_function = lf;
		
	}

	R operator()(Args... args)
	{
		return _function(args...);
	}
private:
	Type _function;
};

#ifndef _WIN64
template <class R, class... Args>
struct OriginalFuncWrapper<R(__stdcall *)(Args...)>
{
	using Type = R(__stdcall*)(Args...);

	OriginalFuncWrapper(Type lf)
	{
		_function = lf;
	}

	R __stdcall operator()(Args... args)
	{
		return _function(args...);
	}
private:
	Type _function;
};
#endif

struct IHookedFuncWrapper
{
	virtual int getReferenceCount() = 0;
};

template <class F>
struct HookedFuncWrapper;

template <class R, class... Args>
struct HookedFuncWrapper<R(__cdecl *)(Args...)> : public IHookedFuncWrapper
{
	using Type = R(__cdecl *)(Args...);

	static HookedFuncWrapper<Type>* getInstance()
	{
		static HookedFuncWrapper<Type> instance;
		return &instance;
	}

	static R invoke(Args... args)
	{
		HookedFuncWrapper<Type>* this_ = getInstance();
		this_->increaseRefCount();
		R ret = _function(args...);
		this_->decreaseRefCount();
		return ret;
	}

	int getReferenceCount() override
	{
		return _callRefCount;
	}

	void decreaseRefCount()
	{
		_callRefCount--;
	}

	void increaseRefCount()
	{
		_callRefCount++;
	}

	static Type _function;
private:
	HookedFuncWrapper() { }

	std::atomic_int _callRefCount = 0;
};

#ifndef _WIN64
// TODO: Add a more permanant fix for this instead of 2 specialisation with 
//		with different calling conventions
template <class R, class... Args>
struct HookedFuncWrapper<R(__stdcall*)(Args...)> : public IHookedFuncWrapper
{
	using Type = R(__stdcall *)(Args...);

	static HookedFuncWrapper<Type>* getInstance()
	{
		static HookedFuncWrapper<Type> instance;
		return &instance;
	}

	static R invoke(Args... args)
	{
		HookedFuncWrapper<Type>* this_ = getInstance();
		this_->increaseRefCount();
		R ret = _function(args...);
		this_->decreaseRefCount();
		return ret;
	}

	int getReferenceCount() override
	{
		return _callRefCount;
	}

	void decreaseRefCount()
	{
		_callRefCount--;
	}

	void increaseRefCount()
	{
		_callRefCount++;
	}

	static Type _function;
private:
	HookedFuncWrapper() { }

	std::atomic_int _callRefCount = 0;
};
#endif

// Definition for static class variable HookedFuncWrapper<..>::_function
template <class R, class... Args>
R(__cdecl *HookedFuncWrapper<R(__cdecl *)(Args...)>::_function)(Args...) = nullptr;

#ifndef _WIN64
template <class R, class... Args>
R(__stdcall *HookedFuncWrapper<R(__stdcall *)(Args...)>::_function)(Args...) = nullptr;
#endif

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
				v.detour->restoreOriginalFunction();

			for (auto& [_, v] : m_hooks)
			{
				while (v.hookedFunction->getReferenceCount() > 0)
					Sleep(20);
			}

			for (auto& [_, v] : m_hooks)
			{
				v.detour->deleteTrampoline();
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

		IHookedFuncWrapper* hookedFuncWrapper = HookedFuncWrapper<T*>::getInstance();
		HookedFuncWrapper<T*>::_function = hookedFunc;
		HookedFuncData hookedFuncData;
		hookedFuncData.hookedFunction = hookedFuncWrapper;
		hookedFuncData.detour = new Detour(HookedFuncWrapper<T*>::invoke, func, patchSize);
		hookedFuncData.oFunc = static_cast<T*>(hookedFuncData.detour->patch());

		std::stringstream ss;
		ss << "Failed to hook function with type: " << typeid(func).name();
		x_assert(std::any_cast<T*>(hookedFuncData.oFunc) != 0x00, ss.str().c_str());
		
		return m_hooks.insert({ (uintptr_t)func, std::move(hookedFuncData) }).second;
	}

	//Returns original function _F with the correct type
	template<auto _F>
	auto get_ofunction() const
	{
		auto func = _F;
		using _F_t = decltype(func);

		auto search = m_hooks.find((uintptr_t)func);

		if (search == m_hooks.end())
		{
			char msg[1024];
			sprintf_s(msg, sizeof(msg), "Could not find hook for this function - Calling _F\nFunction Address: %p\nFunction Type: %s", _F, typeid(_F).name());
			MessageBoxA(NULL, msg, "Hook Missing - get_ofunction", MB_OK);
			return OriginalFuncWrapper<_F_t>(func);
		}

		std::stringstream ss;
		ss << "call_ofunction _F did not match type of stored original function\n";
		ss << "\n_F_t type: " << typeid(_F_t).name() << "\n\nsearch->second.oFunc type: " << 
			search->second.oFunc.type().name();
		x_assert(typeid(_F_t) == search->second.oFunc.type(), ss.str().c_str());
		
		auto oFunc = std::any_cast<_F_t>(search->second.oFunc);
		return OriginalFuncWrapper<_F_t>(oFunc);
	}

	//Forwards passed arguments to the original function _F
	template<auto _F, typename... Ts>
	auto call_ofunction(Ts&&... args) const
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

		auto oFunc = std::any_cast<_F_t>(search->second.oFunc);
		return OriginalFuncWrapper<_F_t>(oFunc)(std::forward<Ts>(args)...);
	}

private:
	struct HookedFuncData
	{
		IHookedFuncWrapper* hookedFunction;
		Detour* detour;
		std::any oFunc;
	};

	bool m_destroyed = false;

	std::unordered_map<uintptr_t, HookedFuncData> m_hooks;
};