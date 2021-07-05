using System;

namespace XOPE_UI.Config
{
    public class Spy
    {
        public static string ModuleName32 { get; } = "XOPESpy32.dll";
        public static string ModuleName64 { get; } = "XOPESpy64.dll";

        public static string ModulePath32 { get; } = $@"{Environment.CurrentDirectory}\{ModuleName32}";
        public static string ModulePath64 { get; } = $@"{Environment.CurrentDirectory}\{ModuleName64}";

    }
}
