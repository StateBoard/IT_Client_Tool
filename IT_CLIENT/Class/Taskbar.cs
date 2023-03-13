using System;

using System.Runtime.InteropServices;


namespace IT_CLIENT
{
    public class Taskbar
    {
        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);
        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;

        protected static int Handle
        {
            get
            {
                return FindWindow("Shell_TrayWnd", "");
            }
        }

        public Taskbar()
        {
            // hide ctor
        }

        public void Show()
        {
            try
            {
                ShowWindow(Handle, SW_SHOW);
            }
            catch (Exception exe)
            {

            }
        }

        public void Hide()
        {
            try
            {
                ShowWindow(Handle, SW_HIDE);
            }
            catch (Exception exe)
            {

            }
        }
    }
}
