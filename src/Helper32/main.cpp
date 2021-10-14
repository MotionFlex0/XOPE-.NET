#define WIN32_LEAN_AND_MEAN 

#include <iostream>
#include <numeric>
#include <sstream>
#include <unordered_map>
#include <vector>
#include <Windows.h>

const std::unordered_map<const char* , const char*> functions = {
	{"LoadLibraryA", "kernel32.dll"},
	{"FreeLibrary", "kernel32.dll"}
};

int main(int argc, char* argv[])
{	
	std::ostringstream oss;
	if (argc >= 2)
	{
		std::vector<void*> addresses;

		for (int i = 1; i < argc; i++)
		{
			auto findModule = [&](std::pair<const char*, const char*> m) { return strcmp(m.first, argv[i]) == 0; };

			auto search = std::find_if(functions.begin(), functions.end(), findModule);
			if (search != functions.end())
			{
				HMODULE module = GetModuleHandleA(search->second);
				if (module != NULL)
				{
					addresses.push_back(GetProcAddress(module, search->first));
					continue;
				}
			}

			addresses.push_back(0);
		}

		for (void* a : addresses)
		{
			if (!oss.tellp() == 0)
				oss << ",";
			oss << (intptr_t)a;
		}
	}

	if (oss.tellp() == 0)
		std::cout << 0;
	else
		std::cout << oss.str();

	return 0;
}