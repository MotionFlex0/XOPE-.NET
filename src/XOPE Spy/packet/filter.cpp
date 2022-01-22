#include "filter.h"

PacketFilter::PacketFilter()
{

}

boost::uuids::uuid PacketFilter::add(FilterableFunction ff, SOCKET s, const Packet oldVal, const Packet newVal,
	bool replaceEntirePacket, bool recursiveReplace, bool activated)
{
	const boost::uuids::uuid id = generator();
	filterMap[id] = Data
	{
		.socketId = s,
		.oldVal = oldVal, 
		.newVal = newVal,
		.replaceEntirePacket = replaceEntirePacket,
		.recursiveReplace = recursiveReplace,
		.filterableFunction = ff,
		.activated = activated
	};
	return id;
}

bool PacketFilter::modify(boost::uuids::uuid id, FilterableFunction ff, SOCKET s, const Packet oldVal, const Packet newVal, bool replaceEntirePacket, bool recursiveReplace)
{
	const auto it = filterMap.find(id);
	if (it == filterMap.end())
		return false;

	it->second = Data
	{
		.socketId = s,
		.oldVal = oldVal,
		.newVal = newVal,
		.replaceEntirePacket = replaceEntirePacket,
		.recursiveReplace = recursiveReplace,
		.filterableFunction = ff
	};
	return true;
}

bool PacketFilter::toggleActivated(boost::uuids::uuid id, bool isActivated)
{
	const auto it = filterMap.find(id);
	if (it == filterMap.end())
		return false;

	it->second.activated = isActivated;
	return true;
}

bool PacketFilter::remove(boost::uuids::uuid id)
{
	return filterMap.erase(id) == 1;
}

bool PacketFilter::find(FilterableFunction ff, SOCKET s, const Packet packet) const
{
	for (auto& f : filterMap)
	{
		if (f.second.filterableFunction != ff || !f.second.activated)
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

	for (auto& [_, filterData] : filterMap)
	{
		if (filterData.filterableFunction != ff || !filterData.activated)
			continue;

		const Packet& oldVal = filterData.oldVal;
		if ((filterData.socketId == -1 || filterData.socketId == s) && oldVal.size() <= packet.size())
		{
			auto found = std::search(packet.begin(), packet.end(), oldVal.begin(), oldVal.end());
			while (found != packet.end())
			{
				const Packet& newVal = filterData.newVal;
				const int sizeDelta = static_cast<int>(newVal.size()) - static_cast<int>(oldVal.size());
				
				// if newVal is longer than the oldVal
				if (sizeDelta > 0)
				{
					const auto newIt = packet.insert(found + oldVal.size(), sizeDelta, 0);
					found = newIt - oldVal.size();
				}
				else if (sizeDelta < 0)
				{
					const auto beginIt = found + newVal.size();
					packet.erase(beginIt, beginIt + abs(sizeDelta));
				}

				std::copy(newVal.begin(), newVal.end(), found);

				modified = true;
				if (!filterData.recursiveReplace)
					break;
				
				found = std::search(found + newVal.size(), packet.end(), oldVal.begin(), oldVal.end());
				/*auto nextIt = std::copy(newVal.begin(), newVal.end() - copyDelta, found);

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
				if (!filterData.recursiveReplace)
					break;

				found = std::search(nextIt, packet.end(), oldVal.begin(), oldVal.end());*/
			}

			if (modified)
				break;
		}
	}
	return modified;
}
