using System;
using System.Runtime.InteropServices;
using System.Text;

namespace XOPE_UI.Native
{
    using static Win32API;

    public static class NativeMethods
    {
        public static bool CreateConsole()
        {
            return AllocConsole();
        }

        public static int CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes,int dwStackSize,
            IntPtr lpStartAddress, IntPtr lpParameter, ThreadCreationFlag dwCreationFlags)
        {
            int lpThreadIdl;
            Win32API.CreateRemoteThread(hProcess, lpThreadAttributes, dwStackSize, lpStartAddress, lpParameter, (int)dwCreationFlags, out lpThreadIdl);
            return lpThreadIdl;
        }

        public static IntPtr[] EnumProcessModulesEx(IntPtr hProcess, EPMFilterFlag dwFilterFlag)
        {
            int lpcbNeeded;
            IntPtr[] lphModule = null;

            if (Win32API.EnumProcessModulesEx(hProcess, lphModule, 0, out lpcbNeeded, (int)dwFilterFlag))
            {
                lphModule = new IntPtr[lpcbNeeded / Marshal.SizeOf<IntPtr>()];

                Win32API.EnumProcessModulesEx(hProcess, lphModule, lphModule.Length * Marshal.SizeOf<IntPtr>(), out lpcbNeeded, (int)dwFilterFlag);
            }
            
            return lphModule;
        }

        public static string GetFullProcessName(IntPtr hProcess, int dwFlags, int bufferSize=1024)
        {
            StringBuilder processPath = new StringBuilder(bufferSize);
            return QueryFullProcessImageName(hProcess, dwFlags, processPath, ref bufferSize) ? processPath.ToString() : null;
        }

        public static IntPtr GetModuleHandle(IntPtr hProcess, string moduleName)
        {
            IntPtr moduleHandle = IntPtr.Zero;

            IntPtr[] modules = NativeMethods.EnumProcessModulesEx(hProcess, Win32API.EPMFilterFlag.LIST_MODULES_ALL);
            if (modules == null)
                return IntPtr.Zero;

            foreach (IntPtr m in modules)
            {
                if (NativeMethods.GetModuleBaseName(hProcess, m).Equals(moduleName, StringComparison.OrdinalIgnoreCase))
                {
                    moduleHandle = m;
                    break;
                }
            }

            return moduleHandle;
        }

        public static string GetModuleBaseName(IntPtr hProcess, IntPtr hModule, int bufferSize=257)
        {
            StringBuilder moduleName = new StringBuilder(bufferSize);
            Win32API.GetModuleBaseNameA(hProcess, hModule, moduleName, bufferSize);
            return moduleName.ToString() ;
        }

        public static IntPtr GetProcAddress(IntPtr hModule, string lpProcName)
        {
            return Win32API.GetProcAddress(hModule, lpProcName);
        }

        public static bool IsWow64Process(IntPtr hProcess)
        {
            bool wow64Process;
            Win32API.IsWow64Process(hProcess, out wow64Process);
            return wow64Process;
        }

        unsafe public static string RPM(IntPtr hProcess, IntPtr lpBaseAddress, int strLen)
        {
            int lpNumberOfBytesRead;
            byte[] buffer = new byte[strLen];
            ReadProcessMemory(hProcess, lpBaseAddress, buffer, strLen, out lpNumberOfBytesRead);
            return Encoding.ASCII.GetString(buffer);
        }

        unsafe public static T RPM<T>(IntPtr hProcess, IntPtr lpBaseAddress) where T : unmanaged
        {
            int lpNumberOfBytesRead;
            byte[] buffer = new byte[Marshal.SizeOf<T>()];
            ReadProcessMemory(hProcess, lpBaseAddress, buffer, Marshal.SizeOf<T>(), out lpNumberOfBytesRead);
            fixed (byte* pBuffer = buffer)
            {
                return *(T*)pBuffer;
            }
        }

        public static IntPtr SendMessage([In] IntPtr hWnd, WindowsMessage Msg, nuint wParam, string lParam)
        {
            IntPtr pData = Marshal.StringToHGlobalUni(lParam);
            IntPtr res = Win32API.SendMessage(hWnd, (uint)Msg, wParam, pData);
            Marshal.FreeHGlobal(pData);
            return res;
        }

        public static nuint SendMessage([In] IntPtr hWnd, WindowsMessage Msg, nuint wParam, nuint lParam)
        {
            nuint res = Win32API.SendMessage(hWnd, (uint)Msg, wParam, lParam);
            return res;
        }

        public static IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int size, AllocationType flAllocationType, MemoryProtection flProtect)
        {
            return Win32API.VirtualAllocEx(hProcess, lpAddress, size, (int)flAllocationType, (int)flProtect);
        }

        unsafe public static bool WPM(IntPtr hProcess, IntPtr lpBaseAddress, string data)
        {
            int bytesWritten;
            IntPtr pData = Marshal.StringToHGlobalAnsi(data);
            WriteProcessMemory(hProcess, lpBaseAddress, pData, data.Length, out bytesWritten);
            Marshal.FreeHGlobal(pData);
            return bytesWritten == data.Length;
        }

        unsafe public static bool WPM<T>(IntPtr hProcess, IntPtr lpBaseAddress, T data) where T : unmanaged
        {
            int bytesWritten;
            WriteProcessMemory(hProcess, lpBaseAddress, (IntPtr)(byte*)&data, Marshal.SizeOf<T>(), out bytesWritten);
            Console.WriteLine($"WPM length: {Marshal.SizeOf<T>()} | TYPE: {data.GetType()}"); 
            return bytesWritten == Marshal.SizeOf<T>();
        }

        /*unsafe public static int WPM<T>(IntPtr hProcess, IntPtr lpBaseAddress, Int16Converter data, int lpNumberOfBytesWritten) where T : class
        {
            int bytesWritten;
            //;
            

            fixed (void* ka = data)
            {

            }

            IntPtr addr = Marshal.UnsafeAddrOfPinnedArrayElement(data, 0);
            WriteProcessMemory(hProcess, lpBaseAddress, addr, data.Length, out bytesWritten);
            return bytesWritten;
        }*/
    }
}
