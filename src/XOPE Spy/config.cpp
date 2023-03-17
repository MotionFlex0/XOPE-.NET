#include "config.h"

bool Config::isTunnellingEnabled() const
{
	return _isTunnelingEnabled;
}

void Config::toggleTunnellingEnabled(bool enable)
{
	_isTunnelingEnabled = enable;
}

bool Config::isPortTunnelable(int port) const
{
	return _tunnelablePorts.contains(port);
}

bool Config::isInterceptorEnabled() const
{
	return _isInterceptorEnabled;
}

void Config::toggleInterceptorEnabled(bool enable)
{
	_isInterceptorEnabled = enable;
}
