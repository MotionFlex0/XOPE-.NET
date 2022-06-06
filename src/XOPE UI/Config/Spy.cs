using System;

namespace XOPE_UI.Config
{
    public class Spy
    {
        public static string ModuleName32 => "XOPESpy32.dll";
        public static string ModuleName64 => "XOPESpy64.dll";

        public static string ModulePath32 => $@"{Environment.CurrentDirectory}\{ModuleName32}";
        public static string ModulePath64 => $@"{Environment.CurrentDirectory}\{ModuleName64}";

        public static int SinkPortIPv4 => 10101;
        public static int SinkPortIPv6 => 10102;

        public static string ReceiverPipeNamePrefix => "xopeui_";
    }
}
