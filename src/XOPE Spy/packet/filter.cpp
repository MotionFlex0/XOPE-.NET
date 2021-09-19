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
		const Packet& oldVal = f.second.oldVal;
		if (oldVal.size() <= packet.size())
		{
			auto found = std::search(packet.begin(), packet.end(), oldVal.begin(), oldVal.end());
			if (found != packet.end())
			{
				const Packet& newVal = f.second.newVal;
				const int delta = newVal.size() > oldVal.size();

				std::copy(newVal.begin(), newVal.end() - delta, found);

				if (delta > 0)
				{
					packet.insert(found + delta, newVal.begin() + delta, newVal.end());
				}
				else if (delta < 0)
				{
					auto endIt = found + oldVal.size();
					packet.erase(endIt - delta - 1, endIt);
				}
				return true;
			}
		}
		return false;
	}
}
