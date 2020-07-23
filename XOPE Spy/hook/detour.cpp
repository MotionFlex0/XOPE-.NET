#include "detour.h"

Detour32::Detour32(void* hookFunc, void* targetFunc, int bytesToPatch)
{
	if (bytesToPatch < 5)
		return;

	_targetFunc = (int8_t*)targetFunc;
	_detourFunc = (int8_t*)hookFunc;
	_bytesToPatch = bytesToPatch;
}

void* Detour32::patch()
{
	if (_bytesToPatch < 5)
		return nullptr;

	if (_patched)
		return _trampoline;

	_trampoline = new int8_t[_bytesToPatch + 5];

	MemoryProtect ignore(_trampoline, _bytesToPatch+5, PAGE_EXECUTE_READWRITE, false);
	memcpy(_trampoline, _targetFunc, _bytesToPatch);
	_trampoline[_bytesToPatch] = (int8_t)0xE9;
	*(int32_t*)&_trampoline[_bytesToPatch+1] = (int32_t)((_targetFunc - _trampoline - 5));

	MemoryProtect mp(_targetFunc, _bytesToPatch, PAGE_EXECUTE_READWRITE);
	_targetFunc[0] = (int8_t)0xE9;
	*(int32_t*)&_targetFunc[1] = (int32_t)(_detourFunc - _targetFunc - 5);

	if (_bytesToPatch > 5)
	{
		for (int i = 5; i < _bytesToPatch; i++)
			_targetFunc[i] = (int8_t)0x90;
	}

	_patched = true;
	return _trampoline;
}

void Detour32::unpatch()
{
	if (!_patched)
		return;

	MemoryProtect mp(_targetFunc, _bytesToPatch, PAGE_EXECUTE_READWRITE);
	memcpy(_targetFunc, _trampoline, _bytesToPatch);
	MemoryProtect ignore(_trampoline, _bytesToPatch + 5, PAGE_READWRITE, false);

	delete _trampoline;
	_trampoline = nullptr;
	_patched = false;
}

Detour64::Detour64(void* detourFunc, void* targetFunc, int bytesToPatch)
{
	assert(bytesToPatch >= 14);

	_targetFunc = (int8_t*)targetFunc;
	_detourFunc = (int8_t*)detourFunc;
	_bytesToPatch = bytesToPatch;
}

void* Detour64::patch()
{
	if (_patched)
		return _trampoline;

	_trampoline = new int8_t[_bytesToPatch + 14];

	MemoryProtect ignore(_trampoline, _bytesToPatch + 14, PAGE_EXECUTE_READWRITE, false);
	memcpy(_trampoline, _targetFunc, _bytesToPatch);
	_trampoline[_bytesToPatch] = (int8_t)0xFF; //JMP near absolute 
	_trampoline[_bytesToPatch + 1 ] = (int8_t)0x25;
	*(int32_t*)&_trampoline[_bytesToPatch + 2] = 0; //((_targetFunc - _trampoline - 5));
	*(int64_t*)&_trampoline[_bytesToPatch + 6] = (int64_t)_targetFunc + _bytesToPatch;

	MemoryProtect mp(_targetFunc, _bytesToPatch, PAGE_EXECUTE_READWRITE);
	_targetFunc[0] = (int8_t)0xFF;
	_targetFunc[1] = (int8_t)0x25; //(int32_t)(_detourFunc - _targetFunc - 5);
	*(int32_t*)&_targetFunc[2] = 0;
	*(int64_t*)&_targetFunc[6] = (int64_t)_detourFunc;

	if (_bytesToPatch > 14)
	{
		for (int i = 14; i < _bytesToPatch; i++)
			_targetFunc[i] = (int8_t)0x90;
	}

	_patched = true;
	return _trampoline;
}

void Detour64::unpatch()
{
}
