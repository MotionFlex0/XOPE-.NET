#pragma once
#define _WINSOCKAPI_
#define _WINSOCK_DEPRECATED_NO_WARNINGS

#include <queue>
#include <time.h>
#include "hook/hookmgr.hpp"
#include "pipe/namedpipeclient.h"
#include "packet/type.h"
#include "packet/filter.h"
#include "patches/functions.h"
#include "server/incomingmessage.h"
#include "server/namedpipeserver.h"
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

	HookManager* getHookManager();
	const PacketFilter& getPacketFilter(FilterableFunction filterableFunction);
	
	template<class T>
	void sendToUI(T message);
	void processIncomingMessages();
private:
	Application() { }

	HMODULE _dllModule = NULL;
	HookManager* _hookManager = nullptr;
	NamedPipeClient* _namedPipeClient = nullptr;
	NamedPipeServer* _namedPipeServer = nullptr;

	std::unordered_map<FilterableFunction, PacketFilter> _packetFilters;

	bool _stopApplication = false;
	HANDLE _applicationThread;
	std::thread _serverThread;

	void initHooks();
	bool initClient(std::string spyServerPipeName);
	void initServer(std::string spyServerPipeName);
	void run();
};

template<class T>
void Application::sendToUI(T message)
{
	Util::Derived_from < T, client::IMessage>();
	_namedPipeClient->send(message);
}