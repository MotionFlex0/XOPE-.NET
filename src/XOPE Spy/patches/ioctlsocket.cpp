#include "../application.h"
#include "functions.h"

int WSAAPI Functions::Hooked_Ioctlsocket(SOCKET s, long cmd, u_long* argp)
{
	Application& app = Application::getInstance();
	int ret = app.getHookManager()->get_ofunction<ioctlsocket>()(s, cmd, argp);
	
	if (ret == 0 && cmd == FIONBIO && *argp == 1)
		app.getOpenSocketsRepo()->setSocketToNonBlocking(s);

	return ret;
}