#pragma once
#pragma once
#include <boost/functional/hash.hpp>
#include <boost/uuid/uuid.hpp>          
#include <boost/uuid/uuid_generators.hpp>
#include <boost/uuid/uuid_io.hpp>       

#include <unordered_map>
#include <Windows.h>

#include "type.h"

class FilterReplace
{
	struct Data
	{
		Packet oldVal;
		Packet newVal;
		bool inlineReplace;
	};

public:
	FilterReplace();

	boost::uuids::uuid add(const Packet oldVal, const Packet newVal, bool inlineReplace);
	void remove(boost::uuids::uuid id);


	bool find(const Packet packet);
	bool findAndReplace(Packet& packet);

private:
	std::unordered_map<boost::uuids::uuid, Data, boost::hash<boost::uuids::uuid>> filterMap;
	boost::uuids::random_generator generator;
};
