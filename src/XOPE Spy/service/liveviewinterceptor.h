#pragma once
#include "../utils/guid.h"

class LiveViewInterceptor
{
public:

	bool isEnabled();
	void setEnabled(bool enable);

	void waitForUserAction(Guid jobId);

private:
	bool _enabled = false;

};