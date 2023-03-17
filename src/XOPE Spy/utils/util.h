#pragma once

#include <string>
#include <source_location>

#include "../dispatcher/type/impl/imessage.h"
#include "../dispatcher/type/impl/imessagewithresponse.h"

namespace Util
{

	template <class T>
	concept IMessageDerived = std::is_base_of_v<IMessage, T> && !std::is_base_of_v<IMessageWithResponse, T>;
	template <class T>
	concept IMessageWithResponseDerived = std::is_base_of_v<IMessageWithResponse, T>;

	constexpr uint32_t line(std::source_location source = std::source_location::current())
	{
		return source.line();
	}
};