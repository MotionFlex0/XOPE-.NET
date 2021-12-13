#include "../hook/detour.h"

#if _WIN64
using Detour = Detour64;
	#define MESSAGEBOXAPATCHSIZE 14
	#define DEFAULTPATCHSIZE 15
    #define CLOSEPATCHSIZE 14
#else
using Detour = Detour32;
	#define MESSAGEBOXAPATCHSIZE 5
	#define DEFAULTPATCHSIZE 5
	#define CLOSEPATCHSIZE 5
#endif
