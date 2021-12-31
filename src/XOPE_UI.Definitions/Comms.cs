using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Definitions
{
    namespace Comms
    {

    }

    public enum UiMessageType
    {
        INVALID_MESSAGE,
        PING,
        PONG,
        ERROR_MESSAGE,
        CONNECTED_SUCCESS,
        HOOKED_FUNCTION_CALL,
        JOB_RESPONSE_SUCCESS,
        JOB_RESPONSE_ERROR
    }

#if CPP_COMPATIBLE_ENUM
#warning INSIDE CPP_COMPATIBLE_ENUM
    public enum SpyMessageType
#else
#warning Another_ONE
    public enum SpyMessageType
#endif
    {
        INVALID_MESSAGE,
        PING,
        PONG,
        ERROR_MESSAGE,
        INJECT_SEND,
        INJECT_RECV,
        IS_SOCKET_WRITABLE,
        REQUEST_SOCKET_INFO,
        ADD_PACKET_FITLER,
        EDIT_PACKET_FILTER,
        DELETE_PACKET_FILTER,
        SHUTDOWN_RECV_THREAD
    }

    public enum HookedFuncType
    {
        CONNECT,
        SEND,
        RECV,
        CLOSE,
        WSACONNECT,
        WSASEND,
        WSARECV
    }

    public enum ReplayableFunction
    {
        SEND,
        RECV,
        WSASEND,
        WSARECV
    }
}
