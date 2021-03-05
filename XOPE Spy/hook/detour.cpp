#include "detour.h"

Detour32::Detour32(void* hookFunc, void* targetFunc, int bytesToPatch)
{
	if (bytesToPatch < 5)
		return;

	m_targetFunc = (int8_t*)targetFunc;
	m_detourFunc = (int8_t*)hookFunc;
	m_bytesToPatch = bytesToPatch;
}

void* Detour32::patch()
{
	if (m_bytesToPatch < 5)
		return nullptr;

	if (m_patched)
		return m_trampoline;

	m_trampoline = new int8_t[m_bytesToPatch + 5];

	MemoryProtect ignore(m_trampoline, m_bytesToPatch+5, PAGE_EXECUTE_READWRITE, false);
	memcpy(m_trampoline, m_targetFunc, m_bytesToPatch);
	m_trampoline[m_bytesToPatch] = (int8_t)0xE9;
	*(int32_t*)&m_trampoline[m_bytesToPatch+1] = (int32_t)((m_targetFunc - m_trampoline - 5));

	MemoryProtect mp(m_targetFunc, m_bytesToPatch, PAGE_EXECUTE_READWRITE);
	m_targetFunc[0] = (int8_t)0xE9;
	*(int32_t*)&m_targetFunc[1] = (int32_t)(m_detourFunc - m_targetFunc - 5);

	if (m_bytesToPatch > 5)
	{
		for (int i = 5; i < m_bytesToPatch; i++)
			m_targetFunc[i] = (int8_t)0x90;
	}

	m_patched = true;
	return m_trampoline;
}

void Detour32::unpatch()
{
	if (!m_patched)
		return;

	MemoryProtect mp(m_targetFunc, m_bytesToPatch, PAGE_EXECUTE_READWRITE);
	memcpy(m_targetFunc, m_trampoline, m_bytesToPatch);
	MemoryProtect ignore(m_trampoline, m_bytesToPatch + 5, PAGE_READWRITE, false);

	delete m_trampoline;
	m_trampoline = nullptr;
	m_patched = false;
}

Detour64::Detour64(void* detourFunc, void* targetFunc, int bytesToPatch)
{
	assert(bytesToPatch >= 14);

	m_targetFunc = (int8_t*)targetFunc;
	m_detourFunc = (int8_t*)detourFunc;
	m_bytesToPatch = bytesToPatch;
}

void* Detour64::patch()
{
	if (m_patched)
		return m_trampoline;

	m_trampoline = new int8_t[m_bytesToPatch + 14];

	MemoryProtect ignore(m_trampoline, m_bytesToPatch + 14, PAGE_EXECUTE_READWRITE, false);
	memcpy(m_trampoline, m_targetFunc, m_bytesToPatch);
	m_trampoline[m_bytesToPatch] = (int8_t)0xFF; //JMP near absolute 
	m_trampoline[m_bytesToPatch + 1 ] = (int8_t)0x25;
	*(int32_t*)&m_trampoline[m_bytesToPatch + 2] = 0; //((_targetFunc - _trampoline - 5));
	*(int64_t*)&m_trampoline[m_bytesToPatch + 6] = (int64_t)m_targetFunc + m_bytesToPatch;

	MemoryProtect mp(m_targetFunc, m_bytesToPatch, PAGE_EXECUTE_READWRITE);
	m_targetFunc[0] = (int8_t)0xFF; //JMP
	m_targetFunc[1] = (int8_t)0x25; //[RIP]
	*(int32_t*)&m_targetFunc[2] = 0;
	*(int64_t*)&m_targetFunc[6] = (int64_t)m_detourFunc;

	if (m_bytesToPatch > 14)
	{
		for (int i = 14; i < m_bytesToPatch; i++)
			m_targetFunc[i] = (int8_t)0x90;
	}

	m_patched = true;
	return m_trampoline;
}

void Detour64::unpatch()
{
	if (!m_patched)
		return;

	MemoryProtect mp(m_targetFunc, m_bytesToPatch, PAGE_EXECUTE_READWRITE);
	memcpy(m_targetFunc, m_trampoline, m_bytesToPatch);
	MemoryProtect ignore(m_trampoline, m_bytesToPatch + 14, PAGE_READWRITE, false);

	delete m_trampoline;
	m_trampoline = nullptr;
	m_patched = false;
}
