#pragma once

#include <string>
#include <Windows.h>
#include <source_location>

#include "../dispatcher/type/impl/imessage.h"

namespace Util
{

	template <class T>
	concept IMessageDerived = std::is_base_of_v<IMessage, T>;

	constexpr uint32_t line(std::source_location source = std::source_location::current())
	{
		return source.line();
	}
};