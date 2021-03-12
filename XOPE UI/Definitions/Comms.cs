using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Definitions
{
    enum ClientPacketType
    {
        HOOKED_FUNCTION_CALL
    }

    enum HookedFuncType
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
    enum SpyPacketType
    {

    }
}
