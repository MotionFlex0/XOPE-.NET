#include "config.h"

bool Config::isTunnellingEnabled() const
{
	return _isTunnelingEnabled;
}

void Config::toggleTunnellingEnabled(bool enable)
{
	_isTunnelingEnabled = enable;
}

bool Config::isPortTunnelable(int destPort) const
{
	return _tunnelablePorts.contains(destPort);
}

bool Config::isInterceptorEnabled() const
{
	return _isInterceptorEnabled;
}

void Config::toggleInterceptorEnabled(bool enable)
{
	_isInterceptorEnabled = enable;
}
