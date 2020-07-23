#pragma once
#include <Windows.h>

class MemoryProtect
{
public:
	MemoryProtect(LPVOID memoryRegion, SIZE_T size, DWORD newProtection, bool autoRestoreProt = true);
	~MemoryProtect();
	void restoreOldProtection();
private:
	LPVOID _memoryRegion;
	SIZE_T _size;
	DWORD _oldMemoryProtection;
	bool _autoRestoreProtection;
};