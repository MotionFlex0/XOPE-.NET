#define WIN32_LEAN_AND_MEAN 

#include <iostream>
#include <Windows.h>
#include <unordered_map>

const std::unordered_map<const char* , const char*> modules = {
	{"LoadLibraryA", "kernel32.dll"},
	{"FreeLibrary", "kernel32.dll"}
};

int main(int argc, char* argv[])
{	
	void* funcAddr = 0;
	if (argc >= 2)
	{
		auto findModule = [=](std::pair<const char*, const char*> m) { return strcmp(m.first, argv[1]) == 0; };
		auto search = std::find_if(modules.begin(), modules.end(), findModule);
		if (search != modules.end())
		{
			HMODULE module = GetModuleHandleA(search->second);
			if (module != NULL)
				funcAddr = GetProcAddress(module, search->first);
		}
	}

	std::cout << (int)funcAddr;
	
	return 0;
}