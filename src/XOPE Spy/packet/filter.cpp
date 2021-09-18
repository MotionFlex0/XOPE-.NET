#include "filter.h"

FilterReplace::FilterReplace()
{

}

boost::uuids::uuid FilterReplace::add(Packet oldVal, Packet newVal, bool inlineReplace)
{
	boost::uuids::uuid id = generator();
	filterMap[id] = Data{ .oldVal = oldVal, .newVal = newVal, .inlineReplace = inlineReplace };
	return id;
}

void FilterReplace::remove(boost::uuids::uuid id)
{
	filterMap.erase(id);
}

bool FilterReplace::find(const Packet packet)
{
	for (const auto& f : filterMap)
	{
		const Packet oldVal = f.second.oldVal;
		if (oldVal.size() <= packet.size())
		{
			auto found = std::search(packet.begin(), packet.end(), oldVal.begin(), oldVal.end());
			return found != packet.end();
		}
		return false;
	}
}

bool FilterReplace::findAndReplace(Packet& packet)
{
	for (const auto& f : filterMap)
	{
		const Packet oldVal = f.second.oldVal;
		if (oldVal.size() <= packet.size())
		{
			auto found = std::search(packet.begin(), packet.end(), oldVal.begin(), oldVal.end());
			if (found != packet.end())
			{
				
			}
		}
		return false;
	}
}
