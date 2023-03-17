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
	static Guid newGuid();
	static Guid& defaultGuid();

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

namespace std
{
	template<>
	struct hash<Guid>
	{
		size_t operator()(const Guid& guid) const
		{
			GUID g = guid;
			size_t data1 = g.Data1;
			size_t data2 = g.Data2;
			size_t data3 = g.Data3;
			size_t data4Split1 = *reinterpret_cast<size_t*>(g.Data4);
			size_t data4Split2 = *reinterpret_cast<size_t*>(&g.Data4[4]);
			return ((data1 * 17 + data2) + data3 * 17) + (data4Split1 + data4Split2);
		}
	};
}

namespace nlohmann
{
	template<>
	struct adl_serializer<Guid>
	{
		static Guid from_json(const json& j) 
		{
			return { j.get<std::string>() };
		}

		static void from_json(const json& j, Guid& guid) 
		{
			guid = j.get<std::string>();
		}

		static void to_json(json& j, Guid guid) 
		{
			j = guid.toString();
		}
	};
}
