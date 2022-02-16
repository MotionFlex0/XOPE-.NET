#pragma once
// Simple wrapper for Windows' GUID

#include <algorithm>
#include <string>
#include <sstream>
#include <Windows.h>
#include "../nlohmann/json.hpp"

class Guid
{
public:
	Guid static newGuid();

	Guid(GUID guid);
	Guid(std::string guid);
	
	GUID get() const;
	std::string toString() const;

	bool operator==(const GUID& guid) const;
	bool operator==(const Guid& guid) const;
	operator GUID() const;
private:
	GUID m_guid;
};

namespace nlohmann
{
	template<>
	struct adl_serializer<Guid>
	{
		static Guid from_json(const json& j) {
			return { j.get<std::string>() };
		}

		static void to_json(json& j, Guid t) {
			j = t.toString();
		}
	};
}
