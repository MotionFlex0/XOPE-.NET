#pragma once

#include "imessagewithresponse.h"


IMessageWithResponse::IMessageWithResponse(UiMessageType m) : IMessage(m), jobId(Guid::newGuid()) 
{
}