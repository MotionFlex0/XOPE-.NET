#include "application.h"

#define EXTERN_DLL_EXPORT extern "C" __declspec( dllexport )

EXTERN_DLL_EXPORT void SendMessageToLog(const char* message, int messageLen)
{
	std::string str(message, messageLen);
	Application::getInstance().sendToUI(client::ExternalMessage(str));
}