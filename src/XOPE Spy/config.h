#pragma once
#include <set>

class Config
{
public:
	bool isTunnellingEnabled() const;
	void toggleTunnellingEnabled(bool enable);

	bool isPortTunnelable(int port) const;

private:

	bool _isTunnelingEnabled = false;

	//TODO: Instead of hardcoded ports, have the UI send them
	const std::set<int> _tunnelablePorts{ {80, 443} };
};