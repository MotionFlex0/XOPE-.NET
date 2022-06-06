#pragma once
#define _WINSOCKAPI_
#define _WINSOCK_DEPRECATED_NO_WARNINGS

#include <mutex>
#include <queue>
#include <set>
#include <time.h>

#include "definitions/socketdata.h"
#include "hook/hookmgr.hpp"
#include "pipe/namedpipeclient.h"
#include "packet/type.h"
#include "packet/filter.h"
#include "patches/functions.h"
#include "server/incomingmessage.h"
#include "server/namedpipeserver.h"
#include "utils/guid.h"
#include "utils/util.h"

class Application
{
public:
	Application(const Application&) = delete;
	Application& operator=(const Application&) = delete;

	static Application& getInstance();

	bool isRunning();

	void init(HMODULE dllModule);
	void start();
	void shutdown();
	// Only use this shutdown if the program is closing/terminating, otherwise use shutdown()
	void programTerminatingShutdown();

	HookManager* getHookManager();
	const PacketFilter& getPacketFilter();

	// TODO: Refactor this mess
	bool isTunnelingEnabled();
	bool isPortTunnelable(int port);
	void startTunnelingSocket(SOCKET socket);
	void stopTunnelingSocket(SOCKET socket);
	bool isSocketTunneled(SOCKET socket);

	bool wasSocketIdSentToSink(SOCKET socket);
	void socketIdSentToSink(SOCKET socket);

	bool isSocketNonBlocking(SOCKET socket);
	void setSocketNonBlocking(SOCKET socket);

	// TODO: Use a single object to store all injectable packets...
	const std::optional<Packet> getNextRecvPacketToInject(SOCKET socket);
	size_t recvPacketsToInjectCount(SOCKET socket);
	void removeInjectableRecvPackets(SOCKET socket);

	void closeSocketGracefully(SOCKET socket);
	bool shouldSocketClose(SOCKET socket);
	void removeSocketFromSet(SOCKET socket);

	void setSocketIpVersion(SOCKET socket, int ipVersion);
	int getSocketIpVersion(SOCKET socket);
	
	void sendToUI(Util::IMessageDerived auto message);
	void processIncomingMessages();
private:
	Application() { }

	HMODULE _dllModule = NULL;
	HookManager* _hookManager = nullptr;
	NamedPipeClient* _namedPipeClient = nullptr;
	NamedPipeServer* _namedPipeServer = nullptr;

	PacketFilter _packetFilter;

	bool _isTunnelingEnabled = false;
	std::unordered_map<SOCKET, SocketData> _socketsData;
	std::mutex _socketsDataMutex;

	//TODO: Instead of hardcoded ports, have the UI send them
	const std::set<int> _tunnelablePorts{ {80, 443} };

	bool _stopApplication = false;
	HANDLE _applicationThread;
	std::thread _serverThread;

	void initHooks();
	bool initClient(std::string spyServerPipeName);
	void initServer(std::string spyServerPipeName);
	void run();
};

void Application::sendToUI(Util::IMessageDerived auto message)
{
	if (_namedPipeClient != nullptr)
		_namedPipeClient->send(message);
}