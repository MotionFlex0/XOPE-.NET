#include "liveviewinterceptor.h"

bool LiveViewInterceptor::isEnabled()
{
	return _enabled;
}

void LiveViewInterceptor::setEnabled(bool enable)
{
	_enabled = enable;
}

void LiveViewInterceptor::waitForUserAction(Guid jobId)
{

}
