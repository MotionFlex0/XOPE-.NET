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
    initClient(name);
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
    // This block is only entered if shutdown() is called from outside of this class (e.g. DLL_PROCESS_DETACH)
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

    _hookManager->destroy();

    _namedPipeClient->close();

    delete _hookManager;
    delete _namedPipeClient;
    delete _namedPipeServer;
}


void Application::run()
{
    while (!_stopApplication)
    {
        std::this_thread::sleep_for(std::chrono::milliseconds(30));

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

void Application::processIncomingMessages()
{
    while (auto incomingMessage = _namedPipeServer->getIncomingMessage())
    {
        SpyMessageType type = (*incomingMessage).type;
        json jsonMessage = (*incomingMessage).rawJsonData;

        if (type == SpyMessageType::PING)
        {
            _namedPipeClient->send(
                client::PongMessageResponse(jsonMessage["JobId"].get<std::string>())
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

            if (data.length() == jsonMessage["Length"].get<int>())
            {
                //hookmgr->get_ofunction<send>()(socket, data.c_str(), data.length(), NULL);
            }
            else
            {
                _namedPipeClient->send(client::ErrorMessage("INJECT_RECV packet size mismatch"));
            }
        }
        else if (type == SpyMessageType::REQUEST_SOCKET_INFO)
        {
            std::string jobId = jsonMessage["JobId"].get<std::string>();
            SOCKET socket = jsonMessage["SocketId"].get<SOCKET>();

            sockaddr_in sin;
            int sinSize = sizeof(sin);
            if (getpeername(socket, (sockaddr*)&sin, &sinSize) == 0 
                && (sin.sin_family == AF_INET || sin.sin_family == AF_INET6))
            {
                int port = ntohs(sin.sin_port);

                char addr[32];
                int addrSize = sizeof(addr);
                WSAAddressToStringA((LPSOCKADDR)&sin, sinSize, NULL, addr, (LPDWORD)&addrSize);

                std::replace(addr, addr + sizeof(addr), ':', '\x00');

                _namedPipeClient->send(
                    client::SocketInfoResponse(jobId, addr, port, sin.sin_family, -1)
                );
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

            if (type == SpyMessageType::ADD_PACKET_FITLER)
            {
                boost::uuids::uuid id = _packetFilter.add(packetType,
                    socket, oldPacket, newPacket, false, recursiveReplace);

                _namedPipeClient->send(client::AddPacketFilterResponse(
                    jsonMessage["JobId"].get<std::string>(),
                    boost::uuids::to_string(id)
                ));
            }
            else if (type == SpyMessageType::MODIFY_PACKET_FILTER)
            {
                const std::string filterId = jsonMessage["FilterId"].get<std::string>();

                bool success = _packetFilter.modify(boost::lexical_cast<boost::uuids::uuid>(filterId),
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
            const std::string filterId = jsonMessage["FilterId"].get<std::string>();
            const bool isActivated = jsonMessage["Activated"].get<bool>();

            bool success = _packetFilter.toggleActivated(boost::lexical_cast<boost::uuids::uuid>(filterId),
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
            const std::string filterId = jsonMessage["FilterId"].get<std::string>();

            _packetFilter.remove(boost::lexical_cast<boost::uuids::uuid>(filterId));

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

    _hookManager->hookNewFunction(connect, Functions::Hooked_Connect, DEFAULTPATCHSIZE);
    _hookManager->hookNewFunction(send, Functions::Hooked_Send, DEFAULTPATCHSIZE);
    _hookManager->hookNewFunction(recv, Functions::Hooked_Recv, DEFAULTPATCHSIZE);
    _hookManager->hookNewFunction(closesocket, Functions::Hooked_CloseSocket, CLOSEPATCHSIZE);
    _hookManager->hookNewFunction(WSAConnect, Functions::Hooked_WSAConnect, DEFAULTPATCHSIZE);
    _hookManager->hookNewFunction(WSASend, Functions::Hooked_WSASend, DEFAULTPATCHSIZE);
    _hookManager->hookNewFunction(WSARecv, Functions::Hooked_WSARecv, DEFAULTPATCHSIZE);
}


bool Application::initClient(std::string spyServerPipeName)
{
    const char* pipePath = "\\\\.\\pipe\\xopeui";

    _namedPipeClient = new NamedPipeClient(pipePath);
    if (!_namedPipeClient->isPipeBroken())
    {
        std::cout << "successfully connected to pipe: " << pipePath << '\n';
        _namedPipeClient->send(client::ConnectedSuccessMessage(spyServerPipeName));
        _namedPipeClient->flushOutBuffer();
    }
    else
        MessageBoxA(NULL, "Failed to connect to named pipe!", "ERROR", MB_OK);

    return !_namedPipeClient->isPipeBroken();
}

void Application::initServer(std::string spyServerPipeName)
{
    _namedPipeServer = new NamedPipeServer(spyServerPipeName);
    if (_namedPipeServer->isPipeBroken())
        MessageBoxA(NULL, "could not start named pipe server in Spy.", "ERROR", MB_OK);  // TODO: Temp solution

    _serverThread = std::thread(&NamedPipeServer::run, _namedPipeServer);
}
