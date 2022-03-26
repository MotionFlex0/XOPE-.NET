namespace XOPE_UI.Model
{
    public enum SpyMessageType
    {
        INVALID_MESSAGE,
        PING,
        PONG,
        ERROR_MESSAGE,
        INJECT_SEND,
        INJECT_RECV,
        IS_SOCKET_WRITABLE,
        REQUEST_SOCKET_INFO,
        TOGGLE_HTTP_TUNNELING,
        ADD_PACKET_FITLER,
        MODIFY_PACKET_FILTER,
        TOGGLE_ACTIVATE_FILTER,
        DELETE_PACKET_FILTER,
        SHUTDOWN_RECV_THREAD
    }
}
