#pragma once


#include <exception>
#include <stdio.h>

#include <Windows.h>

#if _DEBUG
#define x_assert(expr, msg) (!!(expr) || Util::__assert(__FILE__, __LINE__, #expr, msg))
#else
#define x_assert(expr, msg) ((void)0)
#endif

namespace Util
{
	bool __assert(const char* file, int line, const char* exprStr, const char* msg);
}