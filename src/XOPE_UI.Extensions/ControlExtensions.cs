using XOPE_UI.Native;

namespace XOPE_UI.Extensions
{
    //Credits: https://stackoverflow.com/questions/778095/windows-forms-using-backgroundimage-slows-down-drawing-of-the-forms-controls
    public static class ControlExtensions
    {
        public static void ResumeDrawing(this Control control) =>
            control.ResumeDrawing(true);

        public static void ResumeDrawing(this Control control, bool shouldRedraw)
        {
            NativeMethods.SendMessage(control.Handle, Win32API.WindowsMessage.WM_SETREDRAW, 1, 0);
            if (shouldRedraw)
                control.Refresh();
        }

        public static void SuspendDrawing(this Control control) =>
            NativeMethods.SendMessage(control.Handle, Win32API.WindowsMessage.WM_SETREDRAW, 0, 0);
    }
}
