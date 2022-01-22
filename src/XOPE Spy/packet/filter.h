#pragma once
#include <boost/functional/hash.hpp>
#include <boost/uuid/uuid.hpp>          
#include <boost/uuid/uuid_generators.hpp>
#include <boost/uuid/uuid_io.hpp>       

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
	PacketFilter();

	boost::uuids::uuid add(FilterableFunction ff, SOCKET s, const Packet oldVal, 
		const Packet newVal, bool replaceEntirePacket, bool recursiveReplace, bool activated);
	bool modify(boost::uuids::uuid id, FilterableFunction ff, SOCKET s,
		const Packet oldVal, const Packet newVal, bool replaceEntirePacket, bool recursiveReplace);
	bool toggleActivated(boost::uuids::uuid id, bool isActivated);
	bool remove(boost::uuids::uuid id);


	bool find(FilterableFunction ff, SOCKET s, const Packet packet) const;
	bool findAndReplace(FilterableFunction ff, SOCKET s, Packet& packet) const;

private:
	std::unordered_map<boost::uuids::uuid, Data, boost::hash<boost::uuids::uuid>> filterMap;
	boost::uuids::random_generator generator;
};
