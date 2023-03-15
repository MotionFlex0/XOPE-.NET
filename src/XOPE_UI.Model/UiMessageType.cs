using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOPE_UI.Model
{
    public enum UiMessageType
    {
        INVALID_MESSAGE,
        PING,
        PONG,
        INFO_MESSAGE,
        EXTERNAL_MESSAGE,
        ERROR_MESSAGE,
        CONNECTED_SUCCESS,
        HOOKED_FUNCTION_CALL,
        INTERCEPTOR_REQUEST,
        JOB_RESPONSE_SUCCESS,
        JOB_RESPONSE_ERROR
    }
}
