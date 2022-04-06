#include "util.h"

bool Util::__assert(const char* file, int line, const char* exprStr, const char* msg)
{
	char moduleName[MAX_PATH];
	GetModuleFileNameA(NULL, moduleName, sizeof(moduleName));

	char output[2048];
	sprintf_s(output, "Assertion failed!\n\nProgram: %s\nFile: %s\nLine: %d\n\nExpression: %s\n\nMessage: %s\n\n[End Message]", moduleName, file, line, exprStr, msg);
	
	int res = MessageBoxA(NULL, output, "Assertion failed", MB_ICONERROR | MB_ABORTRETRYIGNORE);
	switch (res)
	{
	case (IDABORT):
	case (IDRETRY):
	case (IDIGNORE):
		((void)0); //Not Implemented
	}

	std::terminate();
	return true;
}
