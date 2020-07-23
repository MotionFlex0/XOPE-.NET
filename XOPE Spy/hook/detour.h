#pragma once

#include <cassert>
#include <stdint.h>
#include "../utils/memory.h"


class IDetour;
class Detour32;
class Detour64;

class Detour32
{
public:
	Detour32(void* hookedFunc, void* sourceFunc, int bytesToPatch = 5);

	void* patch();
	void unpatch();

private:
	int8_t* _targetFunc = nullptr; //original function start
	int8_t* _detourFunc = nullptr;
	int8_t* _trampoline = nullptr; //bridge between the old function's first few bytes and the rest of the function
	int _bytesToPatch = 0;
	
	bool _patched = false;
};

class Detour64
{
public:
	Detour64(void* hookedFunc, void* sourceFunc, int bytesToPatch);

	void* patch();
	void unpatch();


private:
	int8_t* _targetFunc = nullptr; //original function start
	int8_t* _detourFunc = nullptr;
	int8_t* _trampoline = nullptr; //bridge between the old function's first few bytes and the rest of the function
	int _bytesToPatch = 0;

	bool _patched = false;
};