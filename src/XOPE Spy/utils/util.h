#pragma once

#include "../definitions/definitions.hpp"
#include <string>
#include <Windows.h>
#include <source_location>

namespace Util
{

	template <class T>
	concept IMessageDerived = std::is_base_of_v<client::IMessage, T>;

	constexpr uint32_t line(std::source_location source = std::source_location::current())
	{
		return source.line();
	}
};