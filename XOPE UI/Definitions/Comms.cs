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

    public enum ServerPacketType
    {
        CONNECTED_SUCCESS,
        HOOKED_FUNCTION_CALL
    }

    public enum SpyPacketType
    {
       INJECT_SEND,
       INJECT_RECV,
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
    //public enum SpyPacketType
    //{

    //}
}
