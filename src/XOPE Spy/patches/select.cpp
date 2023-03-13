#include "functions.h"
#include "../application.h"

// What this function does:
//	- It will spoof a socket's readibility if there are packets that need to be injected
int WSAAPI Functions::Hooked_Select(int nfds, fd_set FAR * readfds, fd_set FAR * writefds, fd_set FAR * exceptfds, const struct timeval FAR * timeout)
{
	Application& app = Application::getInstance();
	
	std::vector<SOCKET> socketsToSpoof;

	fd_set newReadfds;
	FD_ZERO(&newReadfds);

	int ret = 0;
	if (readfds != nullptr)
	{
		for (u_int i = 0; i < readfds->fd_count; i++)
		{
			if (app.getOpenSocketsRepo()->recvPacketsToInjectCount(readfds->fd_array[i]) > 0)
				socketsToSpoof.push_back(readfds->fd_array[i]);
			else
				FD_SET(readfds->fd_array[i], &newReadfds);
		}
	}
	
	ret = app.getHookManager()->get_ofunction<select>()(nfds, &newReadfds, writefds, exceptfds, timeout);

	if (readfds != nullptr)
	{
		for (SOCKET s : socketsToSpoof)
		{
			FD_SET(s, readfds);
			ret++;
		}
	}

	return ret;
}