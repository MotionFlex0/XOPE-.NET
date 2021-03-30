#pragma once

#include <cassert>
#include <stdint.h>
#include "../utils/memory.h"


class IDetour;
class Detour32;
class Detour64;

class IDetour 
{
public:
	virtual void* patch() = 0;
	virtual void unpatch() = 0;
};

class Detour32 : IDetour
{
public:
	Detour32(void* hookedFunc, void* sourceFunc, int bytesToPatch = 5);

	void* patch();
	void unpatch();

private:
	uint8_t* m_targetFunc = nullptr; //original function start
	uint8_t* m_detourFunc = nullptr;
	uint8_t* m_trampoline = nullptr; //bridge between the old function's first few bytes and the rest of the function
	int m_bytesToPatch = 0;
	
	bool m_patched = false;
};

class Detour64 : IDetour
{
public:
	Detour64(void* hookedFunc, void* sourceFunc, int bytesToPatch);

	void* patch();
	void unpatch();


private:
	uint8_t* m_targetFunc = nullptr; //original function start
	uint8_t* m_detourFunc = nullptr;
	uint8_t* m_trampoline = nullptr; //bridge between the old function's first few bytes and the rest of the function
	int m_bytesToPatch = 0;

	bool m_patched = false;
};