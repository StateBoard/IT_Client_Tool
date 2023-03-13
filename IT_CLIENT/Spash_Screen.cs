using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Net;
using System.IO;
using IT_CLIENT.Class;
using IT_CLIENT.Properties;
using Microsoft.Win32;
using System.Globalization;

namespace IT_CLIENT
{
    public partial class Splash_Screen : Form
    {
        Exam_WebService.Exam_IT lw = new Exam_WebService.Exam_IT();
        Taskbar Tsk = new Taskbar();
        StartBtn stbtn = new StartBtn();

        string path, data, datepass;
        bool setSysTimeFlag, flagos = false;

        const int MF_BYPOSITION = 0x400;

        //Declaring Global objects
        //System level functions to be used for hook and unhook keyboard input
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wp, IntPtr lp);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string name);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern short GetAsyncKeyState(Keys key);

        private IntPtr ptrHook;
        private LowLevelKeyboardProc objKeyboardProcess;

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public Keys key;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr extra;
        }

        [DllImport("User32")]

        private static extern int RemoveMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("User32")]

        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("User32")]

        private static extern int GetMenuItemCount(IntPtr hWnd);

        public Splash_Screen()
        {

            InitializeComponent();

            ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule;
            objKeyboardProcess = new LowLevelKeyboardProc(captureKey);
            ptrHook = SetWindowsHookEx(13, objKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0);
            versiontxt.Text = "Version 20.0.1";
            progressBar1.Focus();
        }

        private void Splash_Screen_Load(object sender, EventArgs e)
        {
            try
            {


                path = @"c:\ITEXAM" + Resources.ResourceManager.GetString("Registry_Name") + "";

                if (Directory.Exists(path))
                {
                    // MessageBox.Show(" March16 Folder Already Create");
                }
                else
                {
                    Directory.CreateDirectory(path);
                }


                setSysTimeFlag = true;


                //  // remove before final exam


                //   AllowCtrlAltDelete();
                //   RegistryKey objRegistryKey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                //  objRegistryKey.SetValue("DisableLockWorkstation", 0);
                //   objRegistryKey.Close();

                try
                {
                    string path = @"c:\ITEXAM" + Resources.ResourceManager.GetString("Registry_Name") + "\\screen.txt";


                    try
                    {
                        FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                        if (!File.Exists(path))
                        {

                            StreamWriter writer = new StreamWriter(fs);
                            writer.WriteLine(Common.ScreenRes());
                            writer.Close();

                        }
                        else
                        {

                            StreamWriter writer = new StreamWriter(fs);
                            writer.WriteLine(Common.ScreenRes());
                            writer.Close();
                        }
                    }
                    catch
                    {

                    }
                }
                catch (Exception exe)
                {

                }
                try
                {
                  //  Common.screenset("1024", "768");
                  //  HideControl();
                    //Common.KillCtrlAltDelete();

                    //  ShowControl();

                }
                catch (Exception exe)
                {
                    MessageBox.Show("H2");
                }


                //ShowControl();

                timer.Interval = 250;
                timer.Tick += new EventHandler(IncreaseProgressBar);
                timer.Start();
            }
            catch (UnauthorizedAccessException ua)
            {
                MessageBox.Show("Run this Tool from Admin login ");
                this.Hide();
                Application.Exit();

            }
        }


        private void IncreaseProgressBar(object sender, EventArgs e)
        {
            try
            {

                int flag = 0;
                progressBar1.Increment(4);

                if (progressBar1.Value == progressBar1.Maximum)
                {
                    flag = 1;

                }
                if (flag == 1)
                {
                    timer.Stop();
                    string Index_NO = ch();
                    if (Index_NO == "" || (!Index_NO.Contains("J")))
                    {
                        MessageBox.Show("You have not done inspection ");
                        ShowControl();
                        Application.Exit();

                    }
                    else
                    {
                        GetServerTime(Index_NO);//Get Index Number and Pass GetServerTime
                    }
                    //this.Hide();

                }
            }
            catch (Exception exe)
            {
                MessageBox.Show(exe.ToString());
            }
        }
        public void GetServerTime(string Index_No)

        {
            try
            {
                int Day = 0;

                DataTable dt = lw.GetServerTime();
                if (dt.Rows.Count > 0)
                {
                    Set_System_Time set_system_time = new Set_System_Time();
                    set_system_time.SetTime(Convert.ToUInt16(dt.Rows[0]["DD"].ToString()), Convert.ToUInt16(dt.Rows[0]["MM"].ToString()), Convert.ToUInt16(dt.Rows[0]["YYYY"].ToString()), Convert.ToUInt16(dt.Rows[0]["HR"].ToString()), Convert.ToUInt16(dt.Rows[0]["MIN"].ToString()), Convert.ToUInt16(dt.Rows[0]["SEC"].ToString()), dt.Rows[0]["AMPM"].ToString());
                    //Change Day on Exam Schedule

                    if (dt.Rows[0]["DD"].ToString() != "11"&& dt.Rows[0]["DD"].ToString() != "12"&& dt.Rows[0]["DD"].ToString() != "13"&& dt.Rows[0]["DD"].ToString() != "14")
                    {
                        MessageBox.Show("This is not the Correct date to start Exam.");
                        ShowControl();
                        Application.Exit();
                    }
                    if (dt.Rows[0]["DD"].ToString() == "11") { Day = 1; }
                    if (dt.Rows[0]["DD"].ToString() == "12") { Day = 2; }
                    if (dt.Rows[0]["DD"].ToString() == "13") { Day = 3; }
                    if (dt.Rows[0]["DD"].ToString() == "14") { Day = 4; }
                    if (Day == 1 || Day == 2 || Day == 3 || Day == 4)
                    {
                        MessageBox.Show("Exam Day "+Day);
                            Check_Login_Time(dt, Index_No);
                    }
                }
                else
                {
                    MessageBox.Show("This is not the Correct date to start Exam.");
                    ShowControl();
                    Application.Exit();
                }


            }
            catch (Exception exe)
            {
                MessageBox.Show(exe.ToString());
                MessageBox.Show("Please Check Internet Connectivity");
                ShowControl();
                Application.Exit();
            }
        }


        public void Check_Login_Time(DataTable dt, string Index_No)
        {
            int HOUR = 0, MINUTE = 0, SECONDS = 0;

            string sysUIFormat = dt.Rows[0]["AMPM"].ToString();
            MINUTE = Convert.ToInt32(dt.Rows[0]["MIN"].ToString());
            HOUR = Convert.ToInt32(dt.Rows[0]["HR"].ToString());
            if (sysUIFormat == "PM")
            {
                if (HOUR >= 12)
                {

                }
                else
                {
                    HOUR = HOUR + 12;
                }
            }
            if (HOUR >= 10 && HOUR <= 13) //Morning Batch
            {
                if (HOUR == 10)
                {
                    if (MINUTE >= 30 && MINUTE <= 59)
                    {
                        //LOGIN
                        Login_Success(dt, Index_No);
                    }
                    else
                    {
                        MessageBox.Show("This Is Not The Right Time");
                        ShowControl();
                        Application.Exit();
                    }
                }
                else if (HOUR == 13)
                {
                    if (MINUTE >= 0 && MINUTE <= 30)
                    {
                        //LOGIN
                        Login_Success(dt, Index_No);
                    }
                    else
                    {
                        MessageBox.Show("Time Over For Login");
                        ShowControl();
                        Application.Exit();
                    }

                }
                else
                {
                    //LOGIN
                    Login_Success(dt, Index_No);
                }


            }
            else if (HOUR >= 14 && HOUR <= 17) //Afternoon Batch
            {
                if (HOUR == 14)
                {
                    if (MINUTE >= 30 && MINUTE <= 59)
                    {
                        Login_Success(dt, Index_No);
                    }
                    else
                    {
                        MessageBox.Show("This Is Not The Right Time");
                        ShowControl();
                        Application.Exit();
                    }
                }
                else if (HOUR == 17)
                {
                    if (MINUTE >= 0 && MINUTE <= 30)
                    {
                        //LOGIN
                        Login_Success(dt, Index_No);
                    }
                    else
                    {
                        MessageBox.Show("Time Over For Login");
                        ShowControl();
                        Application.Exit();
                    }

                }
                else
                {
                    //LOGIN
                    Login_Success(dt, Index_No);
                }

            }
            else
            {
                MessageBox.Show("This Is Not The Right Time");
                ShowControl();
                Application.Exit();
            }
        }
        public void Login_Success(DataTable dt, string Index_No)
        {
            this.Hide();
            Login login = new Login(dt, Index_No);
            login.Show();
        }

        private void HideControl()
        {
            Tsk.Hide();
            stbtn.Hide();


        }

        public string ch()
        {

            try
            {

                OperatingSystem osversion = Environment.OSVersion;
                string os = osversion.Version.Major.ToString();
                if (Convert.ToInt32(os) < 5)
                {
                    flagos = true;
                    MessageBox.Show("The system is not valid For Exam!! Operating system should be Xp or Higher than it");
                    ShowControl();
                    Application.Exit();

                }
                //-------- check for "JULY2016" key  for windows 7 ---------
                if (IntPtr.Size == 8)
                {
                    RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node");
                    int cnt = key.SubKeyCount;
                    int flag = 0;
                    string[] subkeys = key.GetSubKeyNames();
                    for (int i = 0; i < cnt; i++)
                    {
                        string str = subkeys[i];
                        if (str == Resources.ResourceManager.GetString("Registry_Name"))
                        {
                            flag = 1;
                            break;
                        }
                    }
                    if (flag == 0)
                    {
                        timer.Stop(); MessageBox.Show("The System is Not Approved for Exam .");
                        ShowControl();
                        Application.Exit();

                    }
                    else if (flag == 1)
                    {

                        RegistryKey dttkey = Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\" + Resources.ResourceManager.GetString("Registry_Name") + "");
                        int count1 = dttkey.SubKeyCount;
                        int flag1 = 0;
                        string[] subkeys1 = dttkey.GetSubKeyNames();
                        for (int i = 0; i < count1; i++)
                        {
                            string str1 = subkeys1[i];
                            if (str1 == "cindex")
                            {
                                flag1 = 1;
                                break;
                            }
                        }
                        if (flag1 == 0)
                        {

                        }
                        else
                        {
                            RegistryKey subkey3 = Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\" + Resources.ResourceManager.GetString("Registry_Name") + "\\cindex");


                            string[] valname = subkey3.GetValueNames();
                            int valcnt = subkey3.ValueCount;

                            for (int i = 0; i < valcnt;)
                            {
                                data = subkey3.GetValue(valname[i]).ToString();
                                if (data == "")
                                {

                                    MessageBox.Show("You have not done inspection ");
                                    ShowControl();
                                    Application.Exit();
                                }

                                else
                                {

                                }
                                break;
                            }

                        }

                    }
                }
                //-------------Check JULY2016 folder for Windows XP
                else if (IntPtr.Size == 4)
                {
                    RegistryKey key = Registry.LocalMachine.OpenSubKey("Software");
                    int cnt = key.SubKeyCount;
                    int flag = 0;
                    string[] subkeys = key.GetSubKeyNames();
                    for (int i = 0; i < cnt; i++)
                    {
                        string str = subkeys[i];
                        if (str == Resources.ResourceManager.GetString("Registry_Name"))
                        {
                            flag = 1;
                            break;
                        }
                    }
                    if (flag == 0)
                    {

                        timer.Stop(); MessageBox.Show("The System is Not Approved for Exam ");
                        ShowControl();
                        Application.Exit();

                    }
                    else if (flag == 1)
                    {


                        RegistryKey dttkey = Registry.LocalMachine.OpenSubKey("Software\\" + Resources.ResourceManager.GetString("Registry_Name") + "");
                        int count1 = dttkey.SubKeyCount;
                        int flag1 = 0;
                        string[] subkeys1 = dttkey.GetSubKeyNames();
                        for (int i = 0; i < count1; i++)
                        {
                            string str1 = subkeys1[i];
                            if (str1 == "cindex")
                            {
                                flag1 = 1;
                                break;
                            }
                        }
                        if (flag1 == 0)
                        {

                        }
                        else
                        {
                            RegistryKey subkey3 = Registry.LocalMachine.OpenSubKey("Software\\" + Resources.ResourceManager.GetString("Registry_Name") + "\\cindex");

                            string[] valname = subkey3.GetValueNames();
                            int valcnt = subkey3.ValueCount;
                            if (valcnt == 0)
                            {
                                MessageBox.Show("You have not done inspection ");
                                ShowControl();
                                Application.Exit();
                            }
                            for (int i = 0; i < valcnt;)
                            {
                                data = subkey3.GetValue(valname[i]).ToString();
                                if (data == "")
                                {

                                    MessageBox.Show("You have not done inspection ");
                                    ShowControl();
                                    Application.Exit();
                                }

                                else
                                {

                                    //MessageBox.Show("Run Client");

                                }

                                break;
                            }
                        }
                    }
                    else
                    {

                    }
                }
                else
                {
                    MessageBox.Show("Upgrade Your System");
                    ShowControl();
                    Application.Exit();

                }

                StringBuilder sb = new StringBuilder();


                lw.Url = Common.GetServerURLBy_Cindex(data.Replace("J", "").Substring(0, 2), data.Replace("J", "").Substring(3, 2)) + "Exam_IT.asmx";
                sb.Append(lw.Check_Status());

                if (sb.ToString() == "UPDATE")
                {
                    MessageBox.Show("Please Visit gk Site for the UPDATES ...");
                    Tsk.Show();
                    Common.AllowCtrlAltDelete(); stbtn.Show();
                    Application.Exit();

                }
                else if (sb.ToString() == "WAIT")
                {
                    // MessageBox.Show("NOT A CORRECT DATE TO RUN Data Test");
                    MessageBox.Show("NOT A CORRECT DATE TO RUN CLIENT ", "MESSAGE");
                    ShowControl();
                    Application.Exit();

                }

                else if (sb.ToString() == "OK")
                {


                }
            }
            catch (WebException webex)
            {
                MessageBox.Show("Unable to Reach The Internet ... Please Try Again !!!");
                this.Hide();
                ShowControl();
                Application.Exit();
            }
            catch (UnauthorizedAccessException ua)
            {
                MessageBox.Show("Run this Tool from Admin login ");
                ShowControl();
                Application.Exit();

            }
            return data;
        }

        private void ShowControl()
        {
            Tsk.Show();
            Common.AllowCtrlAltDelete();
            stbtn.Show(); string line1 = "";
            using (StreamReader reader = new StreamReader(@"c:\ITEXAM" + Resources.ResourceManager.GetString("Registry_Name") + "\\screen.txt"))
            {
                line1 = reader.ReadLine() ?? "";
            }
            string[] spscreen = line1.Split('x');
            Common.screenset_default(spscreen[0], spscreen[1]);

        }

        //Disable Enable
        #region
        private IntPtr captureKey(int nCode, IntPtr wp, IntPtr lp)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));


                if (objKeyInfo.key == Keys.RWin || objKeyInfo.key == Keys.LWin) // Disabling Windows keys
                {
                    return (IntPtr)1;
                }
                if (objKeyInfo.key == Keys.Alt || objKeyInfo.key == Keys.Tab) // Disabling alt+tab
                {
                    return (IntPtr)1;
                }
               
                if (objKeyInfo.key == Keys.Alt || objKeyInfo.key == Keys.F4) // Disabling alt+f4
                {
                    return (IntPtr)1;
                }
                if (objKeyInfo.key == Keys.Alt || objKeyInfo.key == Keys.Escape) // Disabling alt+esc
                {
                    return (IntPtr)1;
                }
                if (objKeyInfo.key == Keys.RWin || objKeyInfo.key == Keys.LWin) // Disabling Windows keys
                {
                    return (IntPtr)1;
                }


            }
            return CallNextHookEx(ptrHook, nCode, wp, lp);
        }
        #endregion
    }
}
