#pragma once
#include <string>
#include "../definitions/definitions.h"

namespace StringConverter
{
	std::string IpAddressV4ToString(const sockaddr_in* sin);
	std::string IpAddressV6ToString(const sockaddr_in6* sin);
};