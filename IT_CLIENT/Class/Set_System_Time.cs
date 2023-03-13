using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace IT_CLIENT.Class
{

     class Set_System_Time
    {
        ushort lpyear, lpmonth, lpday, lphour, lpmin, lpsec;
        private struct SystemTime
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        [DllImport("kernel32.dll")]
        private extern static void GetSystemTime(ref SystemTime lpSystemTime);

        [DllImport("kernel32.dll")]
        private extern static uint SetSystemTime(ref SystemTime lpSystemTime);
        [DllImport("kernel32.dll")]
        private extern static uint SetLocalTime(ref SystemTime lpSystemTime);
        /// 

        public void SetTime(int a, int b, int c, int d, int e, int f,string AMPM)
        {
            try
            {
                string sysUIFormat = DateTime.Now.ToString("tt", CultureInfo.InvariantCulture);
            

            DateTime dste = DateTime.Now;
            // Call the native GetSystemTime method 
            // with the defined structure.
            SystemTime systime = new SystemTime();

            systime.wYear = (ushort)c;
            systime.wMonth = (ushort)b;
            systime.wDay = (ushort)a;
                if (AMPM == "PM")
                {
                    if (d >= 12)
                    {
                        systime.wHour = (ushort)d;
                    }
                    else
                    {
                        systime.wHour = (ushort)(d + 12);
                    }
                    //systime.wHour = (ushort)(d + 12);
                }
                else if (AMPM == "AM")
                {
                    if (d == 12)
                    {
                        systime.wHour = 0;
                    }
                    else
                    {
                        systime.wHour = (ushort)d;
                    }
                }
                else
                {
                    systime.wHour = (ushort)d;
                }
            systime.wMinute = (ushort)e;
            systime.wSecond = (ushort)f;
           
            SetLocalTime(ref systime);  
            string newtime = systime.wHour.ToString() + systime.wMinute.ToString();
            }
            catch (Exception exe)
            {

            }
        }
    }
}
