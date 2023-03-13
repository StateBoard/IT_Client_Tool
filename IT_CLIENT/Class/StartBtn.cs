using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace IT_CLIENT
{
    class StartBtn
    {
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(
                     IntPtr parentHwnd,
                     IntPtr childAfterHwnd,
                     IntPtr className,
                     string windowText);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hwnd, int command);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;

        //  IntPtr hwndOrb = FindWindowEx(IntPtr.Zero, IntPtr.Zero, (IntPtr)0xC017, null);
        protected static IntPtr hwndOrb
        {
            get
            {
                return FindWindowEx(IntPtr.Zero, IntPtr.Zero, (IntPtr)0xC017, null);
            }
        }

        public void Show()
        { try
            {
                ShowWindow(hwndOrb, SW_SHOW);
            }
            catch (Exception exe)
            {

            }
            }
        public  void Hide()
        {
            try
            {
                ShowWindow(hwndOrb, SW_HIDE);
            }
            catch (Exception exe)
            {

            }
        }
    }
}
