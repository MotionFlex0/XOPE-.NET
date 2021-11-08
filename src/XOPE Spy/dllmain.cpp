//Uncommect the line below to show Debug Console on process injection
//#define SHOW_DEBUG_CONSOLE

#include <iostream>
#include "application.h"

#pragma comment(lib, "ws2_32.lib") // Not needed if WSAGetLastError is removed

void InitConsole();
void RemoveConsole();

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    {
        InitConsole();
        Application& app = Application::getInstance();
        app.init(hModule);
        app.start();
        break;
    }
    case DLL_PROCESS_DETACH:
    {
        Application& app = Application::getInstance();
        app.shutdown();
        RemoveConsole();
        break;
    }
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
        break;
    }
    return TRUE;
}


void InitConsole()
{
#ifdef SHOW_DEBUG_CONSOLE
    AllocConsole();

    FILE* fpstdin = stdin;
    FILE* fpstdout = stdout;
    FILE* fpstderr = stderr;

    freopen_s(&fpstdin, "conin$", "r", stdin);
    freopen_s(&fpstdout, "conout$", "w", stdout);
    freopen_s(&fpstderr, "conout$", "w", stderr);
    std::cout << "Redirected" << std::endl;
#else
    //Redirects stdout/stderror to nothing
    std::cout.rdbuf(nullptr); 
#endif
}

void RemoveConsole()
{
#ifdef SHOW_DEBUG_CONSOLE
    fclose(stdin);
    fclose(stdout);
    fclose(stderr);
    
    if (FreeConsole() == 0)
        MessageBoxA(NULL, "Failed to free console!", "ERROR", MB_OK);
#endif
}

//void PipeThread(LPVOID module)
//{
//
//    if (shouldFreeLibrary)
//        FreeLibraryAndExitThread((HMODULE)module, 0);
//}
