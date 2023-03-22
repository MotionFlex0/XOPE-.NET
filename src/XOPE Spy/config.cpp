#include "config.h"

bool Config::isTunnellingEnabled() const
{
	return _isTunnelingEnabled;
}

void Config::toggleTunnellingEnabled(bool enable)
{
	_isTunnelingEnabled = enable;
}

bool Config::isPortTunnelable(int sourcePort) const
{
	return _tunnelablePorts.contains(sourcePort);
}

bool Config::isInterceptorEnabled() const
{
	return _isInterceptorEnabled;
}

void Config::toggleInterceptorEnabled(bool enable)
{
	_isInterceptorEnabled = enable;
}
