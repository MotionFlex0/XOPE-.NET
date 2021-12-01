#include "filter.h"

PacketFilter::PacketFilter()
{

}

boost::uuids::uuid PacketFilter::add(SOCKET s, const Packet oldVal, const Packet newVal, 
	bool replaceEntirePacket)
{
	const boost::uuids::uuid id = generator();
	filterMap[id] = Data
	{ 
		.oldVal = oldVal, 
		.newVal = newVal,
		.replaceEntirePacket = replaceEntirePacket 
	};
	return id;
}

void PacketFilter::remove(boost::uuids::uuid id)
{
	filterMap.erase(id);
}

bool PacketFilter::find(SOCKET s, const Packet packet) const
{
	for (auto& f : filterMap)
	{
		const Packet oldVal = f.second.oldVal;
		if (oldVal.size() <= packet.size())
		{
			auto found = std::search(packet.begin(), packet.end(), oldVal.begin(), oldVal.end());
			return found != packet.end();
		}
	}
	return false;
}

bool PacketFilter::findAndReplace(SOCKET s, Packet& packet) const
{
	for (auto& f : filterMap)
	{
		const Packet& oldVal = f.second.oldVal;
		if (s == f.second.socketId && oldVal.size() <= packet.size())
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
					const auto endIt = found + oldVal.size();
					packet.erase(endIt - delta - 1, endIt);
				}
				return true;
			}
		}
	}
	return false;
}
