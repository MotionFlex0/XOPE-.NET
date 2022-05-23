#pragma once

#include <capstone/capstone.h>
#include <cassert>
#include <stdint.h>
#include <vector>
#include "../utils/util.h"
#include "../utils/memory.h"

#define MINIMUM_PATCH_SIZE_X86 5
#define MINIMUM_PATCH_SIZE_X64 14

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

	virtual int getPatchSize() = 0;
};

class Detour32 : IDetour
{
public:
	// Automatically calculates bytesToPath
	Detour32(void* detourFunc, void* targetFunc);
	// Manual choose the number of bytesToPath
	Detour32(void* detourFunc, void* targetFunc, int bytesToPatch);

	void* patch();
	void restoreOriginalFunction();
	void deleteTrampoline();
	void unpatch();

	int getPatchSize();

private:
	uint8_t* m_targetFunc = nullptr; //original function start
	uint8_t* m_detourFunc = nullptr;
	uint8_t* m_trampoline = nullptr; //bridge between the old function's first few bytes and the rest of the function
	int m_bytesToPatch = 0;
	
	bool m_patched = false;

	// Automatically calculates bytes to patch if no bytesToPatch is set
	int calculateBytesToPatch();
};

class Detour64 : IDetour
{
public:
	// Automatically calculates bytesToPath
	Detour64(void* detourFunc, void* targetFunc);
	// Manual choose the number of bytesToPath
	Detour64(void* detourFunc, void* targetFunc, int bytesToPatch);

	void* patch();
	void restoreOriginalFunction();
	void deleteTrampoline();
	void unpatch();

	int getPatchSize();
private:
	uint8_t* m_targetFunc = nullptr; //original function start
	uint8_t* m_detourFunc = nullptr;
	uint8_t* m_trampoline = nullptr; //bridge between the old function's first few bytes and the rest of the function
	uint8_t* m_originalBytes = nullptr;
	int m_bytesToPatch = 0;
	int m_trampolineSize = 0;

	bool m_patched = false;

	// Automatically calculates bytes to patch if no bytesToPatch is set
	int calculateBytesToPatch(csh csHandle);

	// Calculates required size of trampoline
	int calculateTrampolineSize(cs_insn* inst, size_t instCount);
};