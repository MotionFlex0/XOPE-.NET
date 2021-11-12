#include "application.h"

Application& Application::getInstance()
{
    static Application instance;
    return instance;
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
    _applicationThread = std::thread(&Application::run, this);
}

void Application::shutdown()
{
    _stopApplication = true;
    _applicationThread.join();

    _namedPipeServer->shutdownServer();
    _serverThread.join();

    _hookManager->destroy();

    _namedPipeClient->close();
    _namedPipeServer->shutdownServer();

    delete _hookManager;
    delete _namedPipeClient;
    delete _namedPipeServer;
}


void Application::run()
{
	while (!_stopApplication)
	{
		processIncomingMessages();

        _namedPipeClient->flushOutBuffer();
        std::this_thread::sleep_for(std::chrono::milliseconds(200));
	}
}

HookManager* Application::getHookManager()
{
    return _hookManager;
}

void Application::processIncomingMessages()
{
    while (auto incomingMessage = _namedPipeServer->getIncomingMessage())
    {
        SpyMessageType type = (*incomingMessage).type;
        json jsonMessage = (*incomingMessage).rawJsonData;

        if (type == SpyMessageType::PING)
        {
            _namedPipeClient->send(client::PongMessageResponse(jsonMessage["JobId"].get<std::string>()));
        }
        else if (type == SpyMessageType::INJECT_SEND)
        {
            SOCKET socket = jsonMessage["SocketId"].get<SOCKET>();
            std::string data = base64_decode(jsonMessage["Data"].get<std::string>());

            if (data.length() == jsonMessage["Length"].get<int>())
            {
                _hookManager->get_ofunction<send>()(socket, data.c_str(), static_cast<int>(data.length()), NULL);
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
            SOCKET socket = jsonMessage["SocketId"].get<SOCKET>();

            sockaddr_in sin;
            int sinSize = sizeof(sin);
            if (getsockname(socket, (sockaddr*)&sin, &sinSize) == 0 && sin.sin_family == AF_INET)
            {
                int port = ((sin.sin_port & 0xFF) << 8) | ((sin.sin_port >> 8));

                char addr[32];
                int addrSize = sizeof(addr);
                WSAAddressToStringA((LPSOCKADDR)&sin, sinSize, NULL, addr, (LPDWORD)&addrSize);

                _namedPipeClient->send(client::SocketInfoResponse(jsonMessage["JobId"].get<std::string>(), std::string(addr), port, sin.sin_family, -1));
            }
            else
            {
                _namedPipeClient->send(client::ErrorMessage("could not find that socket ID"));  // TEMP // need to remove jobId from Server map
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
                selectRet
            ));
        }
        else if (type == SpyMessageType::ADD_SEND_FITLER)
        {
            const std::string oldValue = base64_decode(jsonMessage["OldValue"].get<std::string>());
            const std::string newValue = base64_decode(jsonMessage["NewValue"].get<std::string>());

            const Packet oldPacket(oldValue.begin(), oldValue.end());
            const Packet newPacket(newValue.begin(), newValue.end());


            boost::uuids::uuid id = _sendPacketFilter.add(oldPacket, newPacket, true);

        }
        else if (type == SpyMessageType::SHUTDOWN_RECV_THREAD)
        {
            _namedPipeServer->shutdownServer();
        }
    }

}

void Application::initHooks()
{
    _hookManager = new HookManager();

    _hookManager->hookNewFunction(connect, Functions::Hooked_Connect, DEFAULTPATCHSIZE);
    _hookManager->hookNewFunction(send, Functions::Hooked_Send, DEFAULTPATCHSIZE);
    _hookManager->hookNewFunction(recv, Functions::Hooked_Recv, DEFAULTPATCHSIZE);
    //hookmgr->hookNewFunction(closesocket, Functions::Hooked_CloseSocket, CLOSEPATCHSIZE);
    _hookManager->hookNewFunction(WSAConnect, Functions::Hooked_WSAConnect, DEFAULTPATCHSIZE);
    _hookManager->hookNewFunction(WSASend, Functions::Hooked_WSASend, DEFAULTPATCHSIZE);
    _hookManager->hookNewFunction(WSARecv, Functions::Hooked_WSARecv, DEFAULTPATCHSIZE);
}


void Application::initClient(std::string spyServerPipeName)
{
    const char* pipePath = "\\\\.\\pipe\\xopeui";

    _namedPipeClient = new NamedPipeClient(pipePath);
    if (_namedPipeClient->isValid())
    {
        std::cout << "successfully connected to pipe: " << pipePath << '\n';
        _namedPipeClient->send(client::ConnectedSuccessMessage(spyServerPipeName));
        _namedPipeClient->flushOutBuffer();
    }
    else
        MessageBoxA(NULL, "Failed to connect to named pipe!", "ERROR", MB_OK);
}

void Application::initServer(std::string spyServerPipeName)
{
    _namedPipeServer = new NamedPipeServer(spyServerPipeName);
    if (_namedPipeServer->isPipeBroken())
        MessageBoxA(NULL, "could not start named pipe server in Spy.", "ERROR", MB_OK);  // TODO: Temp solution

    _serverThread = std::thread(&NamedPipeServer::run, _namedPipeServer);
}