#pragma once
#include <any>
#include <functional>
#include <iostream>
#include <intrin.h>
#include <source_location>
#include <sstream>
#include <thread>
#include <typeindex>
#include <unordered_map>
#include "../utils/assert.h"
#include "../utils/definition.hpp"
#include "../utils/util.h"

#define HOOK_FUNCTION_NO_SIZE(hookManager, target, hookedFunc) \
	hookManager->hookNewFunction<Util::line()>(target, hookedFunc, -1, #target);

#define HOOK_FUNCTION_SIZE(hookManager, target, hookedFunc, patchSize) \
	hookManager->hookNewFunction<Util::line()>(target, hookedFunc, patchSize, #target);

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

template <uint32_t line, class F>
struct HookedFuncWrapper;

template <uint32_t line, class R, class... Args>
struct HookedFuncWrapper<line, R(__cdecl *)(Args...)> : public IHookedFuncWrapper
{
	using Type = R(__cdecl *)(Args...);

	static HookedFuncWrapper<line, Type>* getInstance()
	{
		static HookedFuncWrapper<line, Type> instance;
		return &instance;
	}

	static R invoke(Args... args)
	{
		ScopedReferenceCounter refCounter(getInstance());
		return _function(args...);
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

	class ScopedReferenceCounter
	{
	public:
		ScopedReferenceCounter(HookedFuncWrapper<line, Type>* funcWrapper)
		{
			_funcWrapper = funcWrapper;
			_funcWrapper->increaseRefCount();
		}
		~ScopedReferenceCounter()
		{
			_funcWrapper->decreaseRefCount();
		}
	private:
		HookedFuncWrapper<line, Type>* _funcWrapper;
	};
};

#ifndef _WIN64
// TODO: Add a more permanant fix for this instead of 2 specialisation with 
//		with different calling conventions
template <uint32_t line, class R, class... Args>
struct HookedFuncWrapper<line, R(__stdcall*)(Args...)> : public IHookedFuncWrapper
{
	using Type = R(__stdcall *)(Args...);

	static HookedFuncWrapper<line, Type>* getInstance()
	{
		static HookedFuncWrapper<line, Type> instance;
		return &instance;
	}

	static R invoke(Args... args)
	{
		HookedFuncWrapper<line, Type>* this_ = getInstance();
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
template <uint32_t line, class R, class... Args>
R(__cdecl *HookedFuncWrapper<line, R(__cdecl *)(Args...)>::_function)(Args...) = nullptr;

#ifndef _WIN64
template <uint32_t line, class R, class... Args>
R(__stdcall *HookedFuncWrapper<line, R(__stdcall *)(Args...)>::_function)(Args...) = nullptr;
#endif

class HookManager
{
public: 
	HookManager() { }

	~HookManager()
	{
		destroy();
		//m_destroyed = true;
	}

	void destroy() {
		if (!m_destroyed)
		{
			for (auto& [_, v] : m_hooks)
				v.detour->restoreOriginalFunction();

			for (auto& [_, v] : m_hooks)
			{
				while (v.hookedFunction->getReferenceCount() > 0)
					std::this_thread::sleep_for(std::chrono::milliseconds(20));
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

	// line is used to create unique class based on template class which have 
	//  identical T function arguments
	template <uint32_t line, class T>
	bool hookNewFunction(T* target, T* hookedFunc, int patchSize, const char* targetName = "")
	{
		if (m_hooks.contains((uintptr_t)target))
			return true;

		IHookedFuncWrapper* hookedFuncWrapper = HookedFuncWrapper<line, T*>::getInstance();
		HookedFuncWrapper<line, T*>::_function = hookedFunc;
		HookedFuncData hookedFuncData;
		hookedFuncData.hookedFunction = hookedFuncWrapper;
		if (patchSize == -1)
			hookedFuncData.detour = new Detour(HookedFuncWrapper<line, T*>::invoke, target);
		else
			hookedFuncData.detour = new Detour(HookedFuncWrapper<line, T*>::invoke, target, patchSize);
		
		hookedFuncData.oFunc = static_cast<T*>(hookedFuncData.detour->patch());
		hookedFuncData.hookedWrapperId = line;
		hookedFuncData.funcName = targetName;

		std::stringstream ss;
		ss << "Failed to hook function with type: " << typeid(target).name();
		x_assert(std::any_cast<T*>(hookedFuncData.oFunc) != 0x00, ss.str().c_str());
		
		return m_hooks.insert({ (uintptr_t)target, std::move(hookedFuncData) }).second;
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
		ss << "\n_F_t TYPE: " << typeid(_F_t).name() << "\n\nsearch->second.oFunc TYPE: " << 
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
		ss << "_F TYPE: " << typeid(_F_t).name() << " | search->second.oFunc TYPE: " << search->second.oFunc.type().name();
		x_assert(typeid(_F_t) == search->second.oFunc.type(), ss.str().c_str());

		auto oFunc = std::any_cast<_F_t>(search->second.oFunc);
		return OriginalFuncWrapper<_F_t>(oFunc)(std::forward<Ts>(args)...);
	}

private:
	struct HookedFuncData
	{
		IHookedFuncWrapper* hookedFunction;
		uint32_t hookedWrapperId;
		Detour* detour;
		std::any oFunc;
		const char* funcName;
	};

	bool m_destroyed = false;

	std::unordered_map<uintptr_t, HookedFuncData> m_hooks;
};