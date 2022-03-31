#include "functions.h"
#include "../application.h"


int WSAAPI Functions::Hooked_Select(int nfds, fd_set FAR * readfds, fd_set FAR * writefds, fd_set FAR * exceptfds, const struct timeval FAR * timeout)
{
	Application& app = Application::getInstance();
	
	std::vector<SOCKET> socketsToSpoof;
	
	if (writefds != nullptr)
	{
		for (int i = 0; i < writefds->fd_count; i++)
			if (app.isSocketTunneled(writefds->fd_array[i]))
				socketsToSpoof.push_back(writefds->fd_array[i]);
	}

	int ret = app.getHookManager()->get_ofunction<select>()(nfds, readfds, writefds, exceptfds, timeout);
	
	if (writefds != nullptr && ret != SOCKET_ERROR)
	{
		for (SOCKET s : socketsToSpoof)
		{
			FD_SET(s, writefds);
			ret++;
		}
	}
	return ret;
}