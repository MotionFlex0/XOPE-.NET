﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Definitions
{
    namespace Comms
    {

    }

    public enum ServerMessageType
    {
        INVALID_MESSAGE,
        PING,
        PONG,
        ERROR_MESSAGE,
        CONNECTED_SUCCESS,
        HOOKED_FUNCTION_CALL,
        JOB_RESPONSE
    }

#if NETFRAMEWORK
#warning INSIDE NETFRAMEWORK
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
        ADD_SEND_FITLER,
        EDIT_SEND_FILTER,
        DELETE_SEND_FILTER,
        ADD_RECV_FITLER,
        EDIT_RECV_FILTER,
        DELETE_RECV_FILTER,
        SHUTDOWN_RECV_THREAD,
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

    //Maybe pass a struct to the spy like this
    /*
     * json = {
     *    type: 3
     *    args: [
     *      "test",
     *      1,
     *      2,
     *      3
     *    ]
     * 
     */
    //public enum SpyMessageType
    //{

    //}
}
