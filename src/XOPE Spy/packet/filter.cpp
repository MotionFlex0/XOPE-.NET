#include "filter.h"

PacketFilter::PacketFilter()
{

}

boost::uuids::uuid PacketFilter::add(FilterableFunction ff, SOCKET s, const Packet oldVal, const Packet newVal,
	bool replaceEntirePacket, bool recursiveReplace)
{
	const boost::uuids::uuid id = generator();
	filterMap[id] = Data
	{
		.socketId = s,
		.oldVal = oldVal, 
		.newVal = newVal,
		.replaceEntirePacket = replaceEntirePacket,
		.recursiveReplace = recursiveReplace
	};
	return id;
}

void PacketFilter::remove(boost::uuids::uuid id)
{
	filterMap.erase(id);
}

bool PacketFilter::find(FilterableFunction ff, SOCKET s, const Packet packet) const
{
	for (auto& f : filterMap)
	{
		if (f.second.filterableFunction == ff)
			continue;

		const Packet oldVal = f.second.oldVal;
		if (oldVal.size() <= packet.size())
		{
			auto found = std::search(packet.begin(), packet.end(), oldVal.begin(), oldVal.end());
			if (found != packet.end())
				return f.second.socketId == s;
		}
	}
	return false;
}

bool PacketFilter::findAndReplace(FilterableFunction ff, SOCKET s, Packet& packet) const
{
	bool modified = false;

	for (auto& f : filterMap)
	{
		if (f.second.filterableFunction == ff)
			continue;

		const Packet& oldVal = f.second.oldVal;
		if ((f.second.socketId == -1 || f.second.socketId == s) && oldVal.size() <= packet.size())
		{
			auto found = std::search(packet.begin(), packet.end(), oldVal.begin(), oldVal.end());
			while (found != packet.end())
			{
				const Packet& newVal = f.second.newVal;
				const int copyDelta = newVal.size() - oldVal.size();

				auto nextIt = std::copy(newVal.begin(), newVal.end() - copyDelta, found);

				if (copyDelta > 0)
				{
					nextIt = packet.insert(nextIt, newVal.begin() + copyDelta, newVal.end());
					nextIt += newVal.size() - copyDelta;
				}
				else if (copyDelta < 0)
				{
					const auto endIt = found + oldVal.size();
					nextIt = packet.erase(nextIt, endIt);
				}
				
				modified = true;
				if (!f.second.recursiveReplace)
					break;

				found = std::search(nextIt, packet.end(), oldVal.begin(), oldVal.end());
			}

			if (modified)
				break;
		}
	}
	return modified;
}
