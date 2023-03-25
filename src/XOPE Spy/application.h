#pragma once
#define _WINSOCKAPI_
#define _WINSOCK_DEPRECATED_NO_WARNINGS

#include <BS_thread_pool.hpp>
#include <mutex>
#include <queue>
#include <set>
#include <time.h>

#include "config.h"
#include "data/opensocketrepo.h"
#include "definitions/socketdata.h"
#include "hook/hookmgr.hpp"
#include "dispatcher/namedpipedispatcher.h"
#include "job/jobresponse.h"
#include "packet/type.h"
#include "patches/functions.h"
#include "receiver/incomingmessage.h"
#include "receiver/namedpipereceiver.h"
#include "service/liveviewinterceptor.h"
#include "service/packetfilter.h"
#include "utils/guid.h"
#include "utils/util.h"
#include "utils/stringconverter.h"

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

	std::shared_ptr<const HookManager> getHookManager();

	std::shared_ptr<OpenSocketsRepo> getOpenSocketsRepo();

	std::shared_ptr<const PacketFilter> getPacketFilter();
	std::shared_ptr<const LiveViewInterceptor> getLiveViewInterceptor();
	
	std::shared_ptr<const Config> getConfig();

	std::shared_ptr<JobResponse> sendToUI(Util::IMessageWithResponseDerived auto&& message);
	void sendToUI(Util::IMessageDerived auto&& message);
private:
	Application();

	HMODULE _dllModule = NULL;

	// make_shared used in init functions
	std::shared_ptr<HookManager> _hookManager;
	std::shared_ptr<NamedPipeDispatcher> _namedPipeDispatcher;
	std::shared_ptr<NamedPipeReceiver> _namedPipeReceiver;

	std::shared_ptr<OpenSocketsRepo> _openSocketsRepo;

	std::shared_ptr<PacketFilter> _packetFilter;
	std::shared_ptr<LiveViewInterceptor> _liveViewInterceptor;

	std::shared_ptr<Config> _config;
	
	JobQueue _jobQueue;

	std::shared_ptr<BS::thread_pool> _pool;
	std::thread _serverThread;
	HANDLE _applicationThread;
	bool _stopApplication = false;

	void initHooks();
	bool initClient(std::string spyServerPipeName);
	void initServer(std::string spyServerPipeName);
	void run();
	void processIncomingMessages();
	void pingUi();
};

std::shared_ptr<JobResponse> Application::sendToUI(Util::IMessageWithResponseDerived auto&& message)
{
	std::shared_ptr<JobResponse> jobResponse = _jobQueue.push(message.jobId);

	if (_namedPipeDispatcher != nullptr)
		_namedPipeDispatcher->send(std::make_unique<std::remove_reference_t<decltype(message)>>(message));
	return jobResponse;
}

void Application::sendToUI(Util::IMessageDerived auto&& message)
{
	if (_namedPipeDispatcher != nullptr)
		_namedPipeDispatcher->send(std::make_unique<std::remove_reference_t<decltype(message)>>(message));
}
