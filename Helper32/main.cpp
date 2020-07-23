#define WIN32_LEAN_AND_MEAN 

#include <iostream>
#include <Windows.h>
#include <unordered_map>

const std::unordered_map<std::string, const char*> modules = {
	{"LoadLibraryA", "kernel32.dll"},
	{"FreeLibrary", "kernel32.dll"}
};

int main(int argc, char* argv[])
{	
	bool found = false;
	if (argc >= 2)
	{
		auto search = modules.find(argv[1]);
		if (search != modules.end())
			std::cout << (int)GetProcAddress(GetModuleHandleA(search->second), search->first.c_str());
		else
			std::cout << 0;
	}
	else
		std::cout << 0; // so ya don't crash.
	
	return 0;
}