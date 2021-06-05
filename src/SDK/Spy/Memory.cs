using System;

namespace SDK.Spy
{
    class Memory
    {
        public static bool WPM(IntPtr hProcess, IntPtr lpBaseAddress, string data)
        {
            return XOPE_UI.Native.NativeMethods.WPM(hProcess, lpBaseAddress, data);
        }

        public static bool WPM<T>(IntPtr hProcess, IntPtr lpBaseAddress, T data) where T : unmanaged
        {
            return XOPE_UI.Native.NativeMethods.WPM<T>(hProcess, lpBaseAddress, data);
        }

        public static string RPM(IntPtr hProcess, IntPtr lpBaseAddress, int strLen)
        {
            return XOPE_UI.Native.NativeMethods.RPM(hProcess, lpBaseAddress, strLen);
        }

        public static T RPM<T>(IntPtr hProcess, IntPtr lpBaseAddress) where T : unmanaged
        {
            return XOPE_UI.Native.NativeMethods.RPM<T>(hProcess, lpBaseAddress);
        }
    }
}
