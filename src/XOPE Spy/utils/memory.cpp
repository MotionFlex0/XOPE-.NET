#include "memory.h"

MemoryProtect::MemoryProtect(LPVOID memoryRegion, SIZE_T size, DWORD newProtection, bool autoRestoreProt)
{
	_memoryRegion = memoryRegion;
	_size = size;
	_autoRestoreProtection = autoRestoreProt;
	VirtualProtect(memoryRegion, size, newProtection, &_oldMemoryProtection);
}

MemoryProtect::~MemoryProtect()
{
	if (_autoRestoreProtection)
		restoreOldProtection();
}

void MemoryProtect::restoreOldProtection()
{
	VirtualProtect(_memoryRegion, _size, _oldMemoryProtection, &_oldMemoryProtection);
}