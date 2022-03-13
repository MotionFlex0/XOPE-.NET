#pragma once
#include "../definitions/definitions.hpp"

#include <unordered_map>
#include <Windows.h>

#include "type.h"

class PacketFilter
{
	struct Data
	{
		SOCKET socketId;
		Packet oldVal;
		Packet newVal;
		bool replaceEntirePacket;
		bool recursiveReplace;
		FilterableFunction filterableFunction;
		bool activated = true;
	};

public:
	PacketFilter() { };

	Guid add(FilterableFunction ff, SOCKET s, const Packet oldVal, 
		const Packet newVal, bool replaceEntirePacket, bool recursiveReplace, bool activated);
	bool modify(Guid id, FilterableFunction ff, SOCKET s,
		const Packet oldVal, const Packet newVal, bool replaceEntirePacket, bool recursiveReplace);
	bool toggleActivated(Guid id, bool isActivated);
	bool remove(Guid id);


	bool find(FilterableFunction ff, SOCKET s, const Packet packet) const;
	bool findAndReplace(FilterableFunction ff, SOCKET s, Packet& packet) const;

private:
	std::unordered_map<Guid, Data> filterMap;
};
