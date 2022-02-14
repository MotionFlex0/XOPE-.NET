#include "detour.h"

Detour32::Detour32(void* hookFunc, void* targetFunc, int bytesToPatch)
{
	if (bytesToPatch < 5)
		return;

	m_targetFunc = (uint8_t*)targetFunc;
	m_detourFunc = (uint8_t*)hookFunc;
	m_bytesToPatch = bytesToPatch;
}

void* Detour32::patch()
{
	if (m_bytesToPatch < 5)
		return nullptr;

	if (m_patched)
		return m_trampoline;

	m_trampoline = new uint8_t[m_bytesToPatch + 5];

	MemoryProtect ignore(m_trampoline, m_bytesToPatch+5, PAGE_EXECUTE_READWRITE, false);
	memcpy(m_trampoline, m_targetFunc, m_bytesToPatch);
	m_trampoline[m_bytesToPatch] = (int8_t)0xE9;
	*(uint32_t*)&m_trampoline[m_bytesToPatch+1] = (int32_t)((m_targetFunc - m_trampoline - 5));

	MemoryProtect mp(m_targetFunc, m_bytesToPatch, PAGE_EXECUTE_READWRITE);
	m_targetFunc[0] = (uint8_t)0xE9;
	*(uint32_t*)&m_targetFunc[1] = (uint32_t)(m_detourFunc - m_targetFunc - 5);

	if (m_bytesToPatch > 5)
	{
		for (int i = 5; i < m_bytesToPatch; i++)
			m_targetFunc[i] = (uint8_t)0x90;
	}

	m_patched = true;
	return m_trampoline;
}

void Detour32::restoreOriginalFunction()
{
	if (!m_patched)
		return;

	MemoryProtect mp(m_targetFunc, m_bytesToPatch, PAGE_EXECUTE_READWRITE);
	memcpy(m_targetFunc, m_trampoline, m_bytesToPatch);

	if (m_trampoline == nullptr)
		m_patched = false;
}

void Detour32::deleteTrampoline()
{
	if (!m_patched || m_trampoline == nullptr)
		return;

	MemoryProtect ignore(m_trampoline, m_bytesToPatch + 5, PAGE_READWRITE, false);
	delete[] m_trampoline;
	m_trampoline = nullptr;
}

void Detour32::unpatch()
{
	if (!m_patched)
		return;

	restoreOriginalFunction();
	deleteTrampoline();
	m_patched = false;
}

Detour64::Detour64(void* detourFunc, void* targetFunc, int bytesToPatch)
{
	assert(bytesToPatch >= 14);

	m_targetFunc = (uint8_t*)targetFunc;
	m_detourFunc = (uint8_t*)detourFunc;
	m_bytesToPatch = bytesToPatch;
	m_originalBytes = new uint8_t[m_bytesToPatch];
}

void* Detour64::patch()
{
	if (m_patched)
		return m_trampoline;

	memcpy(m_originalBytes, m_targetFunc, m_bytesToPatch);

	csh csHandle;
	cs_insn* inst;
	cs_err csRes = cs_open(CS_ARCH_X86, CS_MODE_64, &csHandle);
	x_assert(csRes == CS_ERR_OK, "cs_open failed in Detour64::patch");

	cs_option(csHandle, CS_OPT_DETAIL, CS_OPT_ON);

	size_t count = cs_disasm(csHandle, m_targetFunc, m_bytesToPatch, (uint64_t)m_targetFunc, 0, &inst);
	x_assert(count > 0, "cs_disasm failed to disassemble code in Detour64::patch");

	m_trampolineSize = calculateTrampolineSize(inst, count);

	m_trampoline = new uint8_t[m_trampolineSize];
	MemoryProtect ignore(m_trampoline, m_trampolineSize, PAGE_EXECUTE_READWRITE, false);

	/*
	* Copies bytes to patch to trampoline and fixes instruction which rely on 
	*	relative disp positioning. (e.g. MOV RAX [EIP+40FC])
	*/
	int offset = 0;
	for (int i = 0; i < count; i++)
	{
		cs_insn* currentInstr = &(inst[i]);
		bool keepOriginalInstr = true;
		if (strcmp(currentInstr->mnemonic, "mov") == 0)
		{
			for (int j = 0; j < currentInstr->detail->x86.op_count; j++)
			{
				cs_x86 opDetails = currentInstr->detail->x86;
				cs_x86_op operand = opDetails.operands[j];
				x86_op_type operandType = operand.type;
				if (operandType == X86_OP_MEM && operand.mem.base == x86_reg::X86_REG_RIP)
				{
					// (currentInstr->address + currentInstr->size) = RIP after exec of this instr.
					// Add disp to RIP, to get the absolute address of operand
					uint64_t absoluteAddr = (currentInstr->address + currentInstr->size) + operand.mem.disp;

					// MOV RAX [absoluteAddr]
					m_trampoline[offset] = (uint8_t)0x48; // REX.W
					m_trampoline[offset+1] = (uint8_t)0xA1; // MOV
					*(uint64_t*)&m_trampoline[offset + 2] = absoluteAddr;

					offset += 10;
					keepOriginalInstr = false;
				}
			}
		}
		
		if (keepOriginalInstr)
		{
			memcpy(m_trampoline + offset, &(currentInstr->bytes[0]), currentInstr->size);
			offset += currentInstr->size;
		}
	}

	m_trampoline[offset] = (uint8_t)0xFF; //JMP near absolute 
	m_trampoline[offset + 1] = (uint8_t)0x25;
	*(uint32_t*)&m_trampoline[offset + 2] = 0;
	*(uint64_t*)&m_trampoline[offset + 6] = (uint64_t)m_targetFunc + m_bytesToPatch;

	cs_free(inst, count);
	cs_close(&csHandle);

	/*
	* Patches target function with a JMP and replace the remaining bytes with NOP
	*/
	MemoryProtect mp(m_targetFunc, m_bytesToPatch, PAGE_EXECUTE_READWRITE);
	m_targetFunc[0] = (uint8_t)0xFF; //JMP
	m_targetFunc[1] = (uint8_t)0x25; //[RIP]
	*(uint32_t*)&m_targetFunc[2] = 0; // +0
	*(uint64_t*)&m_targetFunc[6] = (uint64_t)m_detourFunc;

	if (m_bytesToPatch > 14)
	{
		for (int i = 14; i < m_bytesToPatch; i++)
			m_targetFunc[i] = (uint8_t)0x90;
	}

	m_patched = true;
	return m_trampoline;
}

void Detour64::restoreOriginalFunction()
{
	if (!m_patched || m_originalBytes == nullptr)
		return;

	MemoryProtect mp(m_targetFunc, m_bytesToPatch, PAGE_EXECUTE_READWRITE);
	memcpy(m_targetFunc, m_originalBytes, m_bytesToPatch);
	delete[] m_originalBytes;
	m_originalBytes = nullptr;

	if (m_trampoline == nullptr)
		m_patched = false;
}

void Detour64::deleteTrampoline()
{
	if (!m_patched || m_trampoline == nullptr)
		return;

	MemoryProtect ignore(m_trampoline, m_trampolineSize, PAGE_READWRITE, false);
	delete[] m_trampoline;
	m_trampoline = nullptr;

	if (m_originalBytes == nullptr)
		m_patched = false;
}

void Detour64::unpatch()
{
	if (!m_patched)
		return;

	restoreOriginalFunction();
	deleteTrampoline();
	m_patched = false;
}

int Detour64::calculateTrampolineSize(cs_insn* inst, size_t instCount)
{
	int tSize = 0;

	for (int i = 0; i < instCount; i++)
	{
		cs_insn* currentInstr = &(inst[i]);
		bool keepOriginalInstr = true;
		if (strcmp(currentInstr->mnemonic, "mov") == 0)
		{
			for (int j = 0; j < currentInstr->detail->x86.op_count; j++)
			{
				cs_x86 opDetails = currentInstr->detail->x86;
				cs_x86_op operand = opDetails.operands[j];
				x86_op_type operandType = operand.type;
				if (operandType == X86_OP_MEM && operand.mem.base == x86_reg::X86_REG_RIP)
				{
					// Add 10 bytes. MOV RAX [RIP+DISP] or MOV [RIP+DISP] RAX
					keepOriginalInstr = false;
					tSize += 10;
				}
			}
		}

		if (keepOriginalInstr)
			tSize += currentInstr->size;
	}

	// Add 14 bytes for JMP [m_targetFunction+m_bytesToPatch] -> Executes target function
	tSize += 14;

	return tSize;
}
