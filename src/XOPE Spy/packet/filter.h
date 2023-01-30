#pragma once
#include "../definitions/definitions.h"

#include <unordered_map>
#include <Windows.h>

#include "type.h"

class PacketFilter
{
	struct Data;

public:
	enum class ReplaceState;

	PacketFilter() { };

	Guid add(FilterableFunction ff, SOCKET s, const Packet oldVal, 
		const Packet newVal, bool replaceEntirePacket, bool recursiveReplace, bool activated, bool dropPacket);
	bool modify(Guid id, FilterableFunction ff, SOCKET s,
		const Packet oldVal, const Packet newVal, bool replaceEntirePacket, bool recursiveReplace, bool dropPacket);
	bool toggleActivated(Guid id, bool isActivated);
	bool remove(Guid id);


	bool find(FilterableFunction ff, SOCKET s, const Packet packet) const;
	ReplaceState findAndReplace(FilterableFunction ff, SOCKET s, Packet& packet) const;

	enum class ReplaceState
	{
		NO_CHANGE,
		MODIFIED_PACKET,
		DROP_PACKET
	};
private:
	std::unordered_map<Guid, Data> filterMap;

	struct Data
	{
		SOCKET socketId;
		Packet oldVal;
		Packet newVal;
		bool replaceEntirePacket;
		bool recursiveReplace;
		FilterableFunction filterableFunction;
		bool activated = true;
		bool dropPacket = false;
	};
};
