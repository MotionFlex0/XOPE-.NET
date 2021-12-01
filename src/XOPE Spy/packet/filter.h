#pragma once
#pragma once
#include <boost/functional/hash.hpp>
#include <boost/uuid/uuid.hpp>          
#include <boost/uuid/uuid_generators.hpp>
#include <boost/uuid/uuid_io.hpp>       

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
	};

public:
	PacketFilter();

	boost::uuids::uuid add(SOCKET s, const Packet oldVal, const Packet newVal, bool replaceEntirePacket);
	void remove(boost::uuids::uuid id);


	bool find(SOCKET s, const Packet packet) const;
	bool findAndReplace(SOCKET s, Packet& packet) const;

private:
	std::unordered_map<boost::uuids::uuid, Data, boost::hash<boost::uuids::uuid>> filterMap;
	boost::uuids::random_generator generator;
};
