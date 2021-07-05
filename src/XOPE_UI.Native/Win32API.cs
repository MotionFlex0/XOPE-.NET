using System;
using System.Runtime.InteropServices;
using System.Text;

namespace XOPE_UI.Native
{
    /*
     * Avoid using this class directly. Instead use NativeMethods instead.
     */
    public class Win32API
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AllocConsole();

        /*
         * 
         * */
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateRemoteThread([In] IntPtr hProcess, [In] IntPtr lpThreadAttributes, [In] int dwStackSize,
            [In] IntPtr lpStartAddress, [In] IntPtr lpParameter, [In] int dwCreationFlags, [Out] out int lpThreadId); // lpThreadAttributes not implemented. pass IntPtr.Zero

        [DllImport("Psapi.dll", SetLastError = true)]
        public static extern bool EnumProcessModulesEx([In] IntPtr hProcess, [Out] IntPtr[] lphModule, [In] int cb, [Out] out int lpcbNeeded, [In] int dwFilterFlag);

        [DllImport("Psapi.dll", SetLastError = true)]
        public static extern bool GetModuleBaseNameA([In] IntPtr hProcess, [In] IntPtr hModule, [Out] StringBuilder lpBaseName, [In] int nSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcAddress([In] IntPtr hModule, [In] string lpProcName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool Wow64Process);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess([In] int dwDesiredAccess, [In] bool bInheritHandle, [In] int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool PeekNamedPipe([In] IntPtr hNamedPipe, [Out] out byte[] lpBuffer, [In] int nBufferSize, [Out] out int lpBytesRead, [Out] out int lpTotalBytesAvail, [Out] out int lpBytesLeftThisMessage);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory([In] IntPtr hProcess, [In] IntPtr lpBaseAddress, [Out] byte[] lpBuffer, [In] int nSize, [Out] out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory([In] IntPtr hProcess, [In] IntPtr lpBaseAddress, [In] IntPtr lpBuffer, [In] int nSize, [Out] out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] int dwFlags, [Out] StringBuilder lpExeName, [In, Out] ref int lpdwSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx([In] IntPtr hProcess, [In] IntPtr lpAddress, [In] int size, [In] int flAllocationType, [In] int flProtect);

        [Flags]
        public enum AllocationType
        {
            MEM_COMMIT = 0x00001000,
            MEM_RESERVE = 0x00002000,
            MEM_RESET = 0x00080000,
            MEM_RESET_UNDO = 0x1000000,
            MEM_LARGE_PAGES = 0x20000000,
            MEM_PHYSICAL = 0x00400000,
            MEM_TOP_DOWN = 0x00100000
        }

        [Flags]
        public enum EPMFilterFlag
        {
            LIST_MODULES_DEFAULT = 0x0,
            LIST_MODULES_32BIT = 0x1,
            LIST_MODULES_64BIT = 0x2,
            LIST_MODULES_ALL = 0x3
        }

        [Flags]
        public enum MemoryProtection
        {
            PAGE_NOACCESS = 0b1,
            PAGE_READONLY = 0x2,
            PAGE_READWRITE = 0x4,
            PAGE_WRITECOPY = 0x8,
            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_EXECUTE_READWRITE = 0x40,
            PAGE_EXECUTE_WRITECOPY = 0x80,
            PAGE_GUARD = 0x100,
            PAGE_NOCACHE = 0x200,
            PAGE_WRITECOMBINE = 0x400,
            PAGE_TARGETS_INVALID = 0x40000000,
            PAGE_TARGETS_NO_UPDATE = 0x40000000
        }

        public enum ThreadCreationFlag
        {
            ZERO = 0, //Not needed but it's here as a reminder
            CREATE_SUSPENDED = 0x00000004,
            STACK_SIZE_PARAM_IS_A_RESERVATION = 0x00010000
        }
    }
}
