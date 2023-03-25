#include "application.h"

Application::Application()
{
    _openSocketsRepo = std::make_shared<OpenSocketsRepo>();

    _packetFilter = std::make_shared<PacketFilter>();
    _liveViewInterceptor = std::make_shared<LiveViewInterceptor>();

    _config = std::make_shared<Config>();

    _pool = std::make_shared<BS::thread_pool>();
}

Application& Application::getInstance()
{
    static Application instance;
    return instance;
}

bool Application::isRunning()
{
    return !_stopApplication;
}

void Application::init(HMODULE dllModule)
{
    _dllModule = dllModule;

    srand(static_cast<unsigned int>(time(NULL)));
    int randomNum = rand();

    std::stringstream pipeServerName;
    pipeServerName << "xopespy_" << GetCurrentProcessId() << "_" << randomNum;

    std::string name = pipeServerName.str();
    initServer("\\\\.\\pipe\\"+name);
    bool success = initClient(name);
    if (!success)
        _stopApplication = true;
    else
        initHooks();
}

void Application::start()
{
    auto applicationRunWrapper = [](LPVOID lpVoid) -> DWORD
    {
        Application::getInstance().run();
        return 0;
    };

    _applicationThread = CreateThread(NULL, 0, applicationRunWrapper, NULL, 0, NULL);
}

void Application::shutdown()
{
    // This block is only entered if shutdown() is called from outside of this class (e.g. DllMain with DLL_PROCESS_DETACH)

    _jobQueue.finishAllJobs();

    _pool->pause();
    _pool->wait_for_tasks();
    _pool.reset();

    if (!_stopApplication)
    {
        _stopApplication = true;
        WaitForSingleObject(_applicationThread, INFINITE);
    }

    if (_namedPipeReceiver != nullptr && _serverThread.joinable())
    {
        _namedPipeReceiver->shutdownServer();
        _serverThread.join();
    }

    if (_hookManager != nullptr)
        _hookManager->destroy();

    if (_namedPipeDispatcher != nullptr)
        _namedPipeDispatcher->close();
}

void Application::programTerminatingShutdown()
{
    if (!_stopApplication)
    {
        _stopApplication = true;
        WaitForSingleObject(_applicationThread, INFINITE);
    }

    if (_serverThread.joinable())
    {
        _namedPipeReceiver->shutdownServer();
        _serverThread.join();
    }

    _namedPipeDispatcher->close();
}

void Application::run()
{
    // should be safe to re-enable the thread pool
    _namedPipeDispatcher->disableThreadPool(false);

    while (!_stopApplication)
    {
        std::this_thread::sleep_for(std::chrono::milliseconds(1));

        try 
        {
            processIncomingMessages();
        }
        catch (json::exception ex)
        {
            std::stringstream ss;
            ss << "JSON Exception thrown when processing incoming message. Exception message: " << ex.what();
            sendToUI(dispatcher::ErrorMessage(ss.str()));
        }
        catch (std::exception ex)
        {
            std::stringstream ss;
            ss << "Exception thrown when processing incoming message. Exception message: " << ex.what();
            sendToUI(dispatcher::ErrorMessage(ss.str()));
        }
        
        
        if (_stopApplication || _namedPipeReceiver->isPipeBroken())
            break;

        pingUi();

        _namedPipeDispatcher->flushOutBuffer();
        if (_namedPipeDispatcher->isPipeBroken())
            break;
    }

    _stopApplication = true;
    this->shutdown();

    FreeLibraryAndExitThread(_dllModule, 0);
}

std::shared_ptr<const HookManager> Application::getHookManager()
{
    return _hookManager;
}

std::shared_ptr<OpenSocketsRepo> Application::getOpenSocketsRepo()
{
    return _openSocketsRepo;
}

std::shared_ptr<const Config> Application::getConfig()
{
    return _config;
}

std::shared_ptr<const PacketFilter> Application::getPacketFilter()
{
    return _packetFilter;
}

std::shared_ptr<const LiveViewInterceptor> Application::getLiveViewInterceptor()
{
    return _liveViewInterceptor;
}

void Application::processIncomingMessages()
{
    while (auto incomingMessage = _namedPipeReceiver->getIncomingMessage())
    {
        SpyMessageType type = incomingMessage->type;
        json jsonMessage = incomingMessage->rawJsonData;
        
        if (type == SpyMessageType::PING)
        {
            sendToUI(
                dispatcher::PongMessageResponse(jsonMessage["JobId"].get<Guid>())
            );
        }
        else if (type == SpyMessageType::INJECT_SEND)
        {
            SOCKET socket = jsonMessage["SocketId"].get<SOCKET>();
            std::string data = base64_decode(jsonMessage["Data"].get<std::string>());

            if (data.length() == jsonMessage["Length"].get<int>())
            {
                _hookManager->get_ofunction<send>()
                    (socket, data.c_str(), static_cast<int>(data.length()), NULL);
            }
            else
            {
                sendToUI(dispatcher::ErrorMessage("INJECT_SEND packet size mismatch"));
            }
        }
        else if (type == SpyMessageType::INJECT_RECV)
        {
            SOCKET socket = jsonMessage["SocketId"].get<SOCKET>();
            std::string data = base64_decode(jsonMessage["Data"].get<std::string>());

            const Packet packetToInject(data.begin(), data.end());

            if (data.length() == jsonMessage["Length"].get<int>())
            {
                _openSocketsRepo->addRecvPacketToInject(socket, packetToInject);
            }
            else
            {
               sendToUI(dispatcher::ErrorMessage("INJECT_RECV packet size mismatch"));
            }
        }
        else if (type == SpyMessageType::CLOSE_SOCKET) // This now calls closesocket directly
        {
            SOCKET socket = jsonMessage["SocketId"].get<SOCKET>();
            closesocket(socket);
        }
        else if (type == SpyMessageType::REQUEST_SOCKET_INFO)
        {
            std::string jobId = jsonMessage["JobId"].get<std::string>();
            SOCKET socket = jsonMessage["SocketId"].get<SOCKET>();

            sockaddr_storage destSaStor;
            int destSaStorSize = sizeof(destSaStor);
            if (getpeername(socket, (sockaddr*)&destSaStor, &destSaStorSize) == 0
                && (destSaStor.ss_family == AF_INET || destSaStor.ss_family == AF_INET6))
            {
                if (destSaStor.ss_family == AF_INET)
                {
                    sockaddr_in* destSin = reinterpret_cast<sockaddr_in*>(&destSaStor);

                    sockaddr_in sourceSin;
                    int sourceSinSize = sizeof(sourceSin);
                    getsockname(socket, (sockaddr*)&sourceSin, &sourceSinSize);

                    sendToUI(
                        dispatcher::SocketInfoResponse(
                            jobId, 
                            StringConverter::IpAddressV4ToString(&sourceSin), ntohs(sourceSin.sin_port),
                            StringConverter::IpAddressV4ToString(destSin), ntohs(destSin->sin_port),
                            destSin->sin_family, -1
                        )
                    );
                }
                else if (destSaStor.ss_family == AF_INET6)
                {
                    sockaddr_in6* destSin = reinterpret_cast<sockaddr_in6*>(&destSaStor);

                    sockaddr_in6 sourceSin;
                    int sourceSinSize = sizeof(sourceSin);
                    getsockname(socket, (sockaddr*)&sourceSin, &sourceSinSize);

                    sendToUI(
                        dispatcher::SocketInfoResponse(
                            jobId, 
                            StringConverter::IpAddressV6ToString(&sourceSin), ntohs(sourceSin.sin6_port),
                            StringConverter::IpAddressV6ToString(destSin), ntohs(destSin->sin6_port),
                            destSin->sin6_family, -1
                        )
                    );
                }
                else
                {
                    sendToUI(dispatcher::ErrorMessageResponse(jobId, "could not find open socket " + std::to_string(socket)));
                }
            }
            else
            {
                sendToUI(dispatcher::ErrorMessageResponse(jobId, "unknown socket family ("+ std::to_string(destSaStor.ss_family) +
                    ") for socket " + std::to_string(socket)));
            }
        }
        else if (type == SpyMessageType::IS_SOCKET_WRITABLE)
        {
            SOCKET socket = jsonMessage["SocketId"].get<SOCKET>();

            fd_set fds;
            FD_ZERO(&fds);
            FD_SET(socket, &fds);

            TIMEVAL timeout;
            timeout.tv_sec = 2;
            timeout.tv_usec = 0;

            int selectRet = select(NULL, nullptr, &fds, nullptr, &timeout);

            sendToUI(dispatcher::IsSocketWritableResponse(
                jsonMessage["JobId"].get<std::string>(),
                selectRet == 1,
                selectRet == 0,
                selectRet == SOCKET_ERROR,
                WSAGetLastError()
            ));
        }
        else if (type == SpyMessageType::TOGGLE_HTTP_TUNNELING)
        {
            _config->toggleTunnellingEnabled(jsonMessage["IsTunnelingEnabled"].get<bool>());
        }
        else if (type == SpyMessageType::TOGGLE_INTERCEPTOR)
        {
            _config->toggleInterceptorEnabled(jsonMessage["IsInterceptorEnabled"].get<bool>());
        }
        else if (type == SpyMessageType::ADD_PACKET_FITLER || 
            type == SpyMessageType::MODIFY_PACKET_FILTER)
        {
            const SOCKET socket = jsonMessage["SocketId"].get<SOCKET>();
            const std::string oldValue = base64_decode(jsonMessage["OldValue"].get<std::string>());
            const std::string newValue = base64_decode(jsonMessage["NewValue"].get<std::string>());

            const Packet oldPacket(oldValue.begin(), oldValue.end());
            const Packet newPacket(newValue.begin(), newValue.end());

            const FilterableFunction packetType = jsonMessage["PacketType"].get<FilterableFunction>();

            const bool recursiveReplace = jsonMessage["RecursiveReplace"].get<bool>();

            const bool isActivated = jsonMessage["Activated"].get<bool>();

            const bool dropPacket = jsonMessage["DropPacket"].get<bool>();

            if (type == SpyMessageType::ADD_PACKET_FITLER)
            {
                Guid id = _packetFilter->add(packetType,
                    socket, oldPacket, newPacket, false, recursiveReplace, isActivated, dropPacket);

                sendToUI(dispatcher::AddPacketFilterResponse(
                    jsonMessage["JobId"].get<Guid>(),
                    id
                ));
            }
            else if (type == SpyMessageType::MODIFY_PACKET_FILTER)
            {
                const std::string filterId = jsonMessage["FilterId"].get<std::string>();

                bool success = _packetFilter->modify(filterId,
                    packetType, socket, oldPacket, newPacket, false, recursiveReplace, dropPacket);

                if (success)
                    sendToUI(dispatcher::GenericPacketFilterResponse(
                        jsonMessage["JobId"].get<std::string>()
                    ));
                else
                    sendToUI(dispatcher::ErrorMessageResponse(
                        jsonMessage["JobId"].get<std::string>(),
                        "Error when modifying this packet filter."
                    ));
            }
        }
        else if (type == SpyMessageType::TOGGLE_ACTIVATE_FILTER)
        {
            const Guid filterId = jsonMessage["FilterId"].get<Guid>();
            const bool isActivated = jsonMessage["Activated"].get<bool>();

            bool success = _packetFilter->toggleActivated(filterId,
                isActivated);

            if (success)
                sendToUI(dispatcher::GenericPacketFilterResponse(
                    jsonMessage["JobId"].get<std::string>()
                ));
            else
                sendToUI(dispatcher::ErrorMessageResponse(
                    jsonMessage["JobId"].get<std::string>(),
                    "That packet filter does not seem to exist in Spy."
                ));

        }
        else if (type == SpyMessageType::DELETE_PACKET_FILTER)
        {
            const Guid filterId = jsonMessage["FilterId"].get<Guid>();

            _packetFilter->remove(filterId);

            sendToUI(dispatcher::GenericPacketFilterResponse(
                jsonMessage["JobId"].get<Guid>()
            ));
        }
        else if (type == SpyMessageType::JOB_RESPONSE_SUCCESS ||
            type == SpyMessageType::JOB_RESPONSE_ERROR)
        {
            const Guid jobId = jsonMessage["JobId"].get<Guid>();
            _jobQueue.completeJob(jobId, *incomingMessage);
        }
        else if (type == SpyMessageType::SHUTDOWN_RECV_THREAD)
        {
            _stopApplication = true;
            break;
        }

        if (_stopApplication)
            break;
    }
}

void Application::initHooks()
{
    _hookManager = std::make_shared<HookManager>();

    // Not the biggest fan of macros but unable to find a better way to do this
    HOOK_FUNCTION_NO_SIZE(_hookManager, connect, Functions::Hooked_Connect);
    HOOK_FUNCTION_NO_SIZE(_hookManager, send, Functions::Hooked_Send);
    HOOK_FUNCTION_NO_SIZE(_hookManager, recv, Functions::Hooked_Recv);
    HOOK_FUNCTION_NO_SIZE(_hookManager, closesocket, Functions::Hooked_CloseSocket);
    HOOK_FUNCTION_NO_SIZE(_hookManager, WSAConnect, Functions::Hooked_WSAConnect);
    HOOK_FUNCTION_NO_SIZE(_hookManager, WSASend, Functions::Hooked_WSASend);
    HOOK_FUNCTION_NO_SIZE(_hookManager, WSARecv, Functions::Hooked_WSARecv);
    HOOK_FUNCTION_NO_SIZE(_hookManager, select, Functions::Hooked_Select);
    HOOK_FUNCTION_NO_SIZE(_hookManager, ioctlsocket, Functions::Hooked_Ioctlsocket);
    HOOK_FUNCTION_NO_SIZE(_hookManager, socket, Functions::Hooked_Socket);
    HOOK_FUNCTION_NO_SIZE(_hookManager, WSASocketA, Functions::Hooked_WSASocketA);
    HOOK_FUNCTION_NO_SIZE(_hookManager, WSASocketW, Functions::Hooked_WSASocketW);
//;
    // WSAAsyncSelect & WSAEventSelect have not been hooked but may be needed for non-blocking connects
}

bool Application::initClient(std::string spyServerPipeName)
{
    std::string pipePath = "\\\\.\\pipe\\xopeui_" + std::to_string(GetCurrentProcessId());

    _namedPipeDispatcher = std::make_shared<NamedPipeDispatcher>(pipePath.c_str(), _pool, _jobQueue);
    if (!_namedPipeDispatcher->isPipeBroken())
    {
        std::cout << "successfully connected to pipe: " << pipePath << '\n';
        _namedPipeDispatcher->disableThreadPool(true);
        sendToUI(dispatcher::ConnectedSuccessMessage(spyServerPipeName));
        _namedPipeDispatcher->flushOutBuffer();
    }
    return !_namedPipeDispatcher->isPipeBroken();
}

void Application::initServer(std::string spyServerPipeName)
{
    _namedPipeReceiver = std::make_shared<NamedPipeReceiver>(spyServerPipeName);
    if (_namedPipeReceiver->isPipeBroken())
        MessageBoxA(NULL, "could not start named pipe server in Spy.", "ERROR", MB_OK);  // TODO: Temp solution

    _serverThread = std::thread(&NamedPipeReceiver::run, _namedPipeReceiver);
}

void Application::pingUi()
{
    static ULONGLONG lastPing = GetTickCount64();

    if (((GetTickCount64() - lastPing) / 1000.0f) > 5.0f)
    {
        sendToUI(dispatcher::PingMessage());
        lastPing = GetTickCount64();
    }
}