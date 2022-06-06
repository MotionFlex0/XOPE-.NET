#include <filesystem>

#include "application.h"

#define EXTERN_DLL_EXPORT extern "C" __declspec( dllexport )

EXTERN_DLL_EXPORT void SendMessageToLog(const char* message, int messageLen)
{
	PVOID baseAddressOfCaller;
	char modulFilePath[MAX_PATH];
	std::string moduleFileName;
	std::string str(message, messageLen);
	
	RtlPcToFileHeader(_ReturnAddress(), &baseAddressOfCaller);
	GetModuleFileNameA(static_cast<HMODULE>(baseAddressOfCaller), modulFilePath, sizeof(modulFilePath));
	moduleFileName = std::filesystem::path(modulFilePath).filename().string();

	Application::getInstance().sendToUI(client::ExternalMessage(str, moduleFileName));
}