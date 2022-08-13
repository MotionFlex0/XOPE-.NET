#include "application.h"

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

    initHooks();

    srand(static_cast<unsigned int>(time(NULL)));
    int randomNum = rand();

    std::stringstream pipeServerName;
    pipeServerName << "xopespy_" << GetCurrentProcessId() << "_" << randomNum;

    std::string name = pipeServerName.str();
    initServer("\\\\.\\pipe\\"+name);
    bool success = initClient(name);
    if (!success)
        _stopApplication = true;
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
    if (!_stopApplication)
    {
        _stopApplication = true;
        WaitForSingleObject(_applicationThread, INFINITE);
    }

    if (_namedPipeServer != nullptr && _serverThread.joinable())
    {
        _namedPipeServer->shutdownServer();
        _serverThread.join();
    }

    if (_hookManager != nullptr)
        _hookManager->destroy();

    if (_namedPipeClient != nullptr)
        _namedPipeClient->close();

    delete _hookManager;
    delete _namedPipeClient;
    delete _namedPipeServer;
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
        _namedPipeServer->shutdownServer();
        _serverThread.join();
    }

    _namedPipeClient->close();
}

void Application::run()
{
    while (!_stopApplication)
    {
        std::this_thread::sleep_for(std::chrono::milliseconds(1));

        processIncomingMessages();
        if (_stopApplication || _namedPipeServer->isPipeBroken())
            break;

        _namedPipeClient->flushOutBuffer();
        if (_namedPipeClient->isPipeBroken())
            break;
    }

    _stopApplication = true;
    this->shutdown();

    FreeLibraryAndExitThread(_dllModule, 0);
}

HookManager* Application::getHookManager()
{
    return _hookManager;
}

const PacketFilter& Application::getPacketFilter()
{
    return _packetFilter;
}

bool Application::isTunnelingEnabled()
{
    return _isTunnelingEnabled;
}

bool Application::isPortTunnelable(int port)
{
    return _tunnelablePorts.contains(port);
}

void Application::startTunnelingSocket(SOCKET socket)
{
    std::lock_guard<std::mutex> lock(_socketsDataMutex);
    _socketsData[socket].isTunneled = true;
    _socketsData[socket].socketIdSentToSink = false;

}

void Application::stopTunnelingSocket(SOCKET socket)
{
    std::lock_guard<std::mutex> lock(_socketsDataMutex);
    _socketsData[socket].isTunneled = false;

}

bool Application::isSocketTunneled(SOCKET socket)
{
    std::lock_guard<std::mutex> lock(_socketsDataMutex);
    return _socketsData.contains(socket) && _socketsData[socket].isTunneled;
}

bool Application::wasSocketIdSentToSink(SOCKET socket)
{
    std::lock_guard<std::mutex> lock(_socketsDataMutex);
    return _socketsData[socket].socketIdSentToSink;
}

void Application::emitSocketIdSentToSink(SOCKET socket)
{
    std::lock_guard<std::mutex> lock(_socketsDataMutex);
    _socketsData[socket].socketIdSentToSink = true;
}

bool Application::isSocketNonBlocking(SOCKET socket)
{
    return _socketsData[socket].isBlocking;
}

void Application::setSocketNonBlocking(SOCKET socket)
{
    _socketsData[socket].isBlocking = true;
}

size_t Application::recvPacketsToInjectCount(SOCKET socket)
{
    std::lock_guard<std::mutex> lock(_socketsDataMutex);
    if (_socketsData.contains(socket))
        return _socketsData[socket].recvPacketsToInject.size();
    return 0;
}

void Application::removeInjectableRecvPackets(SOCKET socket)
{
    std::lock_guard<std::mutex> lock(_socketsDataMutex);
    if (_socketsData.contains(socket))
        _socketsData[socket].recvPacketsToInject = {};
}

void Application::closeSocketGracefully(SOCKET socket)
{
    std::lock_guard<std::mutex> lock(_socketsDataMutex);
    _socketsData[socket].closeSocketGracefully = true;
}

bool Application::shouldSocketClose(SOCKET socket)
{
    std::lock_guard<std::mutex> lock(_socketsDataMutex);
    return _socketsData.contains(socket) && _socketsData[socket].closeSocketGracefully;
}

void Application::removeSocketFromSet(SOCKET socket)
{
    std::lock_guard<std::mutex> lock(_socketsDataMutex);
    _socketsData.erase(socket);
}

void Application::setSocketIpVersion(SOCKET socket, int ipVersion)
{
    std::lock_guard<std::mutex> lock(_socketsDataMutex);
    _socketsData[socket].ipVersion = ipVersion;
}

int Application::getSocketIpVersion(SOCKET socket)
{
    std::lock_guard<std::mutex> lock(_socketsDataMutex);
    return _socketsData[socket].ipVersion;
}

const std::optional<Packet> Application::getNextRecvPacketToInject(SOCKET socket)
{
    std::lock_guard<std::mutex> lock(_socketsDataMutex);
    if (!_socketsData.contains(socket))
        return std::nullopt;

    std::queue<Packet>& packetQueue = _socketsData[socket].recvPacketsToInject;
    if (packetQueue.empty())
        return std::nullopt;

    Packet packet = packetQueue.front();
    packetQueue.pop();
    return packet;
}

void Application::processIncomingMessages()
{
    while (auto incomingMessage = _namedPipeServer->getIncomingMessage())
    {
        SpyMessageType type = (*incomingMessage).type;
        json jsonMessage = (*incomingMessage).rawJsonData;

        if (type == SpyMessageType::PING)
        {
            _namedPipeClient->send(
                client::PongMessageResponse(jsonMessage["JobId"].get<Guid>())
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
                _namedPipeClient->send(client::ErrorMessage("INJECT_SEND packet size mismatch"));
            }
        }
        else if (type == SpyMessageType::INJECT_RECV)
        {
            SOCKET socket = jsonMessage["SocketId"].get<SOCKET>();
            std::string data = base64_decode(jsonMessage["Data"].get<std::string>());

            const Packet packetToInject(data.begin(), data.end());

            if (data.length() == jsonMessage["Length"].get<int>())
            {
                std::lock_guard<std::mutex> lock(_socketsDataMutex);
                _socketsData[socket].recvPacketsToInject.push(packetToInject);
            }
            else
            {
                _namedPipeClient->send(client::ErrorMessage("INJECT_RECV packet size mismatch"));
            }
        }
        else if (type == SpyMessageType::CLOSE_SOCKET_GRACEFULLY)
        {
            SOCKET socket = jsonMessage["SocketId"].get<SOCKET>();
            closeSocketGracefully(socket);
        }
        else if (type == SpyMessageType::REQUEST_SOCKET_INFO)
        {
            std::string jobId = jsonMessage["JobId"].get<std::string>();
            SOCKET socket = jsonMessage["SocketId"].get<SOCKET>();

            sockaddr_storage sin;
            int sinSize = sizeof(sin);
            if (getpeername(socket, (sockaddr*)&sin, &sinSize) == 0 
                && (sin.ss_family == AF_INET || sin.ss_family == AF_INET6))
            {
                if (sin.ss_family == AF_INET)
                {
                    sockaddr_in* sa = reinterpret_cast<sockaddr_in*>(&sin);
                    int port = ntohs(sa->sin_port);

                    char addr[INET_ADDRSTRLEN];
                    int addrSize = sizeof(addr);
                    int sinSize = sizeof(sockaddr_in);

                    WSAAddressToStringA((LPSOCKADDR)sa, sinSize, NULL, addr, (LPDWORD)&addrSize);
                    std::replace(addr, addr + sizeof(addr), ':', '\x00');

                    _namedPipeClient->send(
                        client::SocketInfoResponse(jobId, addr, port, sa->sin_family, -1)
                    );
                }
                else if (sin.ss_family == AF_INET6)
                {
                    sockaddr_in6* sa = reinterpret_cast<sockaddr_in6*>(&sin);
                    int port = ntohs(sa->sin6_port);

                    char addr[INET6_ADDRSTRLEN];
                    int addrSize = sizeof(addr);
                    int sinSize = sizeof(sockaddr_in6);

                    WSAAddressToStringA((LPSOCKADDR)sa, sinSize, NULL, addr, (LPDWORD)&addrSize);

                    std::string formattedAddr{ addr };
                    auto colonPos = formattedAddr.find_last_of(':');
                    if (colonPos != std::string::npos)
                        formattedAddr.erase(colonPos);

                    _namedPipeClient->send(
                        client::SocketInfoResponse(jobId, formattedAddr, port, sa->sin6_family, -1)
                    );
                }
            }
            else
            {
                _namedPipeClient->send(
                    client::ErrorMessageResponse(jobId, "could not find that socket ID")
                ); 
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

            _namedPipeClient->send(client::IsSocketWritableResponse(
                jsonMessage["JobId"].get<std::string>(),
                selectRet == 1,
                selectRet == 0,
                selectRet == SOCKET_ERROR,
                WSAGetLastError()
            ));
        }
        else if (type == SpyMessageType::TOGGLE_HTTP_TUNNELING)
        {
            _isTunnelingEnabled = jsonMessage["IsTunnelingEnabled"].get<bool>();
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

            if (type == SpyMessageType::ADD_PACKET_FITLER)
            {
                Guid id = _packetFilter.add(packetType,
                    socket, oldPacket, newPacket, false, recursiveReplace, isActivated);

                _namedPipeClient->send(client::AddPacketFilterResponse(
                    jsonMessage["JobId"].get<Guid>(),
                    id
                ));
            }
            else if (type == SpyMessageType::MODIFY_PACKET_FILTER)
            {
                const std::string filterId = jsonMessage["FilterId"].get<std::string>();

                bool success = _packetFilter.modify(filterId,
                    packetType, socket, oldPacket, newPacket, false, recursiveReplace);

                if (success)
                    _namedPipeClient->send(client::GenericPacketFilterResponse(
                        jsonMessage["JobId"].get<std::string>()
                    ));
                else
                    _namedPipeClient->send(client::ErrorMessageResponse(
                        jsonMessage["JobId"].get<std::string>(),
                        "Error when modifying this packet filter."
                    ));
            }
        }
        else if (type == SpyMessageType::TOGGLE_ACTIVATE_FILTER)
        {
            const Guid filterId = jsonMessage["FilterId"].get<Guid>();
            const bool isActivated = jsonMessage["Activated"].get<bool>();

            bool success = _packetFilter.toggleActivated(filterId,
                isActivated);

            if (success)
                _namedPipeClient->send(client::GenericPacketFilterResponse(
                    jsonMessage["JobId"].get<std::string>()
                ));
            else
                _namedPipeClient->send(client::ErrorMessageResponse(
                    jsonMessage["JobId"].get<std::string>(),
                    "That packet filter does not seem to exist in Spy."
                ));

        }
        else if (type == SpyMessageType::DELETE_PACKET_FILTER)
        {
            const Guid filterId = jsonMessage["FilterId"].get<Guid>();

            _packetFilter.remove(filterId);

            _namedPipeClient->send(client::GenericPacketFilterResponse(
                jsonMessage["JobId"].get<std::string>()
            ));
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
    _hookManager = new HookManager();

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

    _namedPipeClient = new NamedPipeClient(pipePath.c_str());
    if (!_namedPipeClient->isPipeBroken())
    {
        std::cout << "successfully connected to pipe: " << pipePath << '\n';
        _namedPipeClient->send(client::ConnectedSuccessMessage(spyServerPipeName));
        _namedPipeClient->flushOutBuffer();
    }

    return !_namedPipeClient->isPipeBroken();
}

void Application::initServer(std::string spyServerPipeName)
{
    _namedPipeServer = new NamedPipeServer(spyServerPipeName);
    if (_namedPipeServer->isPipeBroken())
        MessageBoxA(NULL, "could not start named pipe server in Spy.", "ERROR", MB_OK);  // TODO: Temp solution

    _serverThread = std::thread(&NamedPipeServer::run, _namedPipeServer);
}
