#pragma once

#include "../definitions/definitions.hpp"
#include <string>
#include <Windows.h>
#include <source_location>

#if _DEBUG
#define x_assert(expr, msg) (!!(expr) || Util::__assert(__FILE__, __LINE__, #expr, msg))
#else
#define x_assert(expr, msg) ((void)0)
#endif

namespace Util
{
	bool __assert(const char* file, int line, const char* exprStr, const char* msg);

	template <class T>
	concept IMessageDerived = std::is_base_of_v<client::IMessage, T>;

	constexpr uint32_t line(std::source_location source = std::source_location::current())
	{
		return source.line();
	}
};