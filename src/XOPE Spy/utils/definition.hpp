#if _WIN64
using Deoour = Detour64;
	#define MESSAGEBOXAPATCHSIZE 14
	#define SENDPATCHSIZE 15
    #define CLOSEPATCHSIZE 15
#else
using Detour = Detour32;
	#define MESSAGEBOXAPATCHSIZE 5
	#define SENDPATCHSIZE 5
	#define CLOSEPATCHSIZE 5
#endif
