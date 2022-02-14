#pragma once

#include <capstone/capstone.h>
#include <cassert>
#include <stdint.h>
#include <vector>
#include "../utils/util.h"
#include "../utils/memory.h"

class IDetour;
class Detour32;
class Detour64;

class IDetour 
{
public:
	virtual void* patch() = 0;
	virtual void restoreOriginalFunction() = 0;
	virtual void deleteTrampoline() = 0;

	// unpatch() calls restoreOriginalFunction and deleteTrampoline.
	virtual void unpatch() = 0;
};

class Detour32 : IDetour
{
public:
	Detour32(void* hookedFunc, void* sourceFunc, int bytesToPatch = 5);

	void* patch();
	void restoreOriginalFunction();
	void deleteTrampoline();
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
	void restoreOriginalFunction();
	void deleteTrampoline();
	void unpatch();


private:
	uint8_t* m_targetFunc = nullptr; //original function start
	uint8_t* m_detourFunc = nullptr;
	uint8_t* m_trampoline = nullptr; //bridge between the old function's first few bytes and the rest of the function
	uint8_t* m_originalBytes = nullptr;
	int m_bytesToPatch = 0;
	int m_trampolineSize = 0;

	bool m_patched = false;

	// Calculates required size of trampoline
	int calculateTrampolineSize(cs_insn* inst, size_t instCount);
};