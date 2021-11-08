#pragma once

#include <string>
#include <Windows.h>

#if _DEBUG
#define x_assert(expr, msg) (!!(expr) || Util::__assert(__FILE__, __LINE__, #expr, msg))
#else
#define x_assert(expr, msg) ((void)0)
#endif

namespace Util
{
	bool __assert(const char* file, int line, const char* exprStr, const char* msg);

	//https://stackoverflow.com/questions/3175219/restrict-c-template-parameter-to-subclass
	template<class T, class B> struct Derived_from {
		static void constraints(T* p) { B* pb = p; }
		Derived_from() { void(*p)(T*) = constraints; }
	};
};