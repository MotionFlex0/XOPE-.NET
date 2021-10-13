using System;
using System.Diagnostics;
using System.IO;
using XOPE_UI.Native;

namespace XOPE_UI.Injection
{
    class CreateRemoteThread
    {
        public static bool Inject(IntPtr hProcess)
        {
            return (!Environment.Is64BitProcess || NativeMethods.IsWow64Process(hProcess))
                ? Inject32(hProcess, XOPE_UI.Config.Spy.ModulePath32)
                : Inject64(hProcess, XOPE_UI.Config.Spy.ModulePath64);
        }

        /*
         * Use Inject64 for remote 64-bit processes. This method is slower as it needs to do some extra 
         *  work to make it compatible with 32-bit processes. 
         */
        public static bool Inject32(IntPtr hProcess, string modulePath)
        {
            if (!File.Exists(modulePath) || !File.Exists("helper32.exe"))
                return false;

            IntPtr loadLibraryAddr = IntPtr.Zero;
           
            using (Process helper32 = new Process())
            {
                helper32.StartInfo.UseShellExecute = false;
                helper32.StartInfo.FileName = "helper32.exe";
                helper32.StartInfo.RedirectStandardOutput = true;
                helper32.StartInfo.Arguments = "LoadLibraryA";

                helper32.Start();

                StreamReader reader = helper32.StandardOutput;
                loadLibraryAddr = (IntPtr)Int32.Parse(reader.ReadToEnd());
                helper32.WaitForExit();
            }

            if (loadLibraryAddr == IntPtr.Zero)
                return false;

            return InjectImpl(hProcess, modulePath, loadLibraryAddr); ;
        }

        public static bool Inject64(IntPtr hProcess, string modulePath)
        {
            if (!File.Exists(modulePath))
                return false;

            IntPtr loadLibraryAddr = IntPtr.Zero;
            var processModules = Process.GetCurrentProcess().Modules;
            foreach (ProcessModule pm in processModules)
            {
                if (pm.ModuleName.ToLower() == "kernel32.dll")
                {
                    loadLibraryAddr = NativeMethods.GetProcAddress(pm.BaseAddress, "LoadLibraryA");
                    break;
                }
            }

            if (loadLibraryAddr == IntPtr.Zero)
                return false;

            return InjectImpl(hProcess, modulePath, loadLibraryAddr);
        }

        /*
         * Make sure all arguments have been checked prior to passing it to this method
         */
        private static bool InjectImpl(IntPtr hProcess, string modulePath, IntPtr loadLibraryAddr)
        {
            IntPtr allocatedMem = NativeMethods.VirtualAllocEx(hProcess, IntPtr.Zero, modulePath.Length + 1, Win32API.AllocationType.MEM_RESERVE | Win32API.AllocationType.MEM_COMMIT, Win32API.MemoryProtection.PAGE_READWRITE);
            if (allocatedMem == IntPtr.Zero)
                return false;

            bool wpmResult = NativeMethods.WPM(hProcess, allocatedMem, modulePath + Char.MinValue); // Passes a null-terminated string.
            if (!wpmResult)
                return false;

            //TODO: maybe store the threadId or return it back, so it can be used at a later point?
            int threadId = NativeMethods.CreateRemoteThread(hProcess, IntPtr.Zero, 0, loadLibraryAddr, allocatedMem, Win32API.ThreadCreationFlag.ZERO);
            if (threadId == 0)
                return false;

            return true;
        }

        public static bool Free(IntPtr hProcess)
        {
            return (!Environment.Is64BitProcess || NativeMethods.IsWow64Process(hProcess))
                ? Free32(hProcess, XOPE_UI.Config.Spy.ModuleName32)
                : Free64(hProcess, XOPE_UI.Config.Spy.ModuleName64);
        }

        public static bool Free32(IntPtr hProcess, string moduleName)
        {
            if (!File.Exists("helper32.exe") || hProcess == IntPtr.Zero)
                return false;
           
            IntPtr moduleToUnload = NativeMethods.GetModuleHandle(hProcess, moduleName);

            if (moduleToUnload == IntPtr.Zero)
                return false;

            Console.WriteLine($"[unload] found module: 0x{moduleToUnload.ToString("X")} | name: {NativeMethods.GetModuleBaseName(hProcess, moduleToUnload).ToLower()}");

            IntPtr freeLibraryAddr = IntPtr.Zero;

            using (Process helper32 = new Process())
            {
                helper32.StartInfo.UseShellExecute = false;
                helper32.StartInfo.FileName = "helper32.exe";
                helper32.StartInfo.RedirectStandardOutput = true;
                helper32.StartInfo.Arguments = "FreeLibrary";

                helper32.Start();

                StreamReader reader = helper32.StandardOutput;
                freeLibraryAddr = (IntPtr)Int32.Parse(reader.ReadToEnd());
                helper32.WaitForExit();
            }

            if (freeLibraryAddr == IntPtr.Zero)
                return false;

            Console.WriteLine($"[unload] found freelibrary(). addr: 0x{freeLibraryAddr.ToString("X")}");

            return FreeImpl(hProcess, moduleToUnload, freeLibraryAddr);
        }

        public static bool Free64(IntPtr hProcess, string moduleName)
        {
            IntPtr moduleToUnload = NativeMethods.GetModuleHandle(hProcess, moduleName);

            //IntPtr[] modules = NativeMethods.EnumProcessModulesEx(hProcess, Win32API.EPMFilterFlag.LIST_MODULES_64BIT);
            //if (modules == null)
            //    return false;

            //foreach (IntPtr m in modules)
            //{
            //    if (NativeMethods.GetModuleBaseName(hProcess, m).Equals(moduleName.ToLower(), StringComparison.OrdinalIgnoreCase))
            //    {
            //        moduleToUnload = m;
            //        break;
            //    }
            //}

            if (moduleToUnload == IntPtr.Zero)
                return false;

            Console.WriteLine($"[unload-64] found module: 0x{moduleToUnload.ToString("X")} | name: {NativeMethods.GetModuleBaseName(hProcess, moduleToUnload).ToLower()}");

            IntPtr freeLibraryAddr = IntPtr.Zero;

            var processModules = Process.GetCurrentProcess().Modules;
            foreach (ProcessModule pm in processModules)
            {
                if (pm.ModuleName.ToLower() == "kernel32.dll")
                {
                    freeLibraryAddr = NativeMethods.GetProcAddress(pm.BaseAddress, "FreeLibrary");
                    break;
                }
            }

            if (freeLibraryAddr == IntPtr.Zero)
                return false;

            Console.WriteLine($"[unload-64] found freelibrary(). addr: 0x{freeLibraryAddr.ToString("X")}");

            return FreeImpl(hProcess, moduleToUnload, freeLibraryAddr);
        }

        private static bool FreeImpl(IntPtr hProcess, IntPtr hModule, IntPtr freeLibraryAddr)
        {
            int threadId = NativeMethods.CreateRemoteThread(hProcess, IntPtr.Zero, 0, freeLibraryAddr, hModule, Win32API.ThreadCreationFlag.ZERO);
            if (threadId == 0)
                return false;

            Console.WriteLine($"[unload] successfully created thread. id: {threadId}");

            return true;
        }
    }
}
