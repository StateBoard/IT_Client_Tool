using IT_CLIENT.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;

namespace IT_CLIENT
{
    public class Common
    {
        public Common()
        {

        }
        //Exam Day
        #region
        public static int Exam_Day1()
        {
            return 10;
        }
        public static int Exam_Day2()
        {
            return 11;
        }
        public static int Exam_Day3()
        {
            return 12;
        }
        public static int Exam_Day4()
        {
            return 13;
        }
        #endregion
        //Morning batch
        #region
        public static int Morning_Batch_Start_Hour()
        {
            return 10;
        }
        public static int Morning_Batch_End_Hour()
        {
            return 10;
        }
        public static int Morning_Batch_Start_Minute()
        {
            return 10;
        }
        public static int Morning_Batch_End_Minute()
        {
            return 10;
        }
        #endregion

        //Afternoon batch
        #region
        public static int Afternoon_Batch_Start_Hour()
        {
            return 10;
        }
        public static int Afternoon_Batch_End_Hour()
        {
            return 10;
        }
        public static int Afternoon_Batch_Start_Minute()
        {
            return 10;
        }
        public static int Afternoon_Batch_End_Minute()
        {
            return 10;
        }
        #endregion
        public static string GetServerURLBy_Cindex(string DID, string TID)
        {
            string URL = "";
            if (DID == "16")
            {
                return URL = "http://115.124.96.32/";
            }
            else if (DID == "31" || DID == "32")
            {
                return URL = "http://115.124.96.33/";
            }
            else if (DID == "18" || DID == "33")
            {
                return URL = "http://115.124.96.34/";
            }
            else if (DID == "12" || TID == "24")
            {
                return URL = "http://115.124.96.35/";
            }
            else if (DID == "11")
            {
                if (TID == "16")
                {
                    return URL = "http://115.124.96.35/";
                }
                return URL = "http://115.124.96.36/";
            }
            else if (DID == "17" || DID == "25" || DID == "26")
            {
                return URL = "http://115.124.96.37/";
            }
            else if (DID == "21" || DID == "22" || DID == "23" || DID == "13" || DID == "14" || DID == "15" || DID == "19")
            {
                return URL = "http://115.124.96.38/";
            }
            else if (DID == "58" || DID == "59" || DID == "62" || DID == "03" || DID == "05" || DID == "06" || DID == "07" || DID == "09" || DID == "29" || DID == "01" || DID == "02" || DID == "04" || DID == "08" || DID == "10" || DID == "56" || DID == "57" || DID == "60" || DID == "61" || DID == "66")
            {
                return URL = "http://115.124.96.39/";
            }
            else
            {
                return URL = "http://115.124.96.37/";
            }


            return URL;



        }
        public static string encryption(string s)
        {
            string ss="";
            ss = s.ToUpper();
            string s1 = "", s2 = "", s3;
            int l, a;
            Char[] thech = ss.ToCharArray();
            string ii = thech.ToString();

            foreach (char ch in thech)
            {

                l = (int)Convert.ToChar(ch);
                a = l ^ 255;
                ss = Convert.ToChar(a).ToString();
                s1 = s1 + ss;
            }
            return s1;
        }
        public static string AnsFileSize(string SeatNo)
        {
            string path = @"c:\ITEXAM" + Resources.ResourceManager.GetString("Registry_Name") + "\\" + SeatNo + ".ans";
            FileInfo fi = new FileInfo(path);
             return fi.Length.ToString();
        }
        public static string GetAnsFilePath(string SeatNo)
        {
            return @"c:\ITEXAM" + Resources.ResourceManager.GetString("Registry_Name") + "\\" + SeatNo + ".ans";
        }
        public static string GetMAC()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection mo = mc.GetInstances();
            string addr = string.Empty;
            string m = string.Empty;
            foreach (ManagementObject o in mo)
            {
                if (addr == string.Empty)
                {
                    if ((bool)o["IPEnabled"] == true)
                    {
                        addr = o["MacAddress"].ToString();
                        m = addr;
                    }
                }
            }
            return m;
        }
        public static void WriteAnsFile(string SeatNo,string Contain)
        { 
            string path = @"c:\ITEXAM" + Resources.ResourceManager.GetString("Registry_Name") + "\\" + SeatNo+".ans";
           
           
            try
            {
                if (!File.Exists(path))
                {
                    FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                    StreamWriter writer = new StreamWriter(fs);
                    writer.WriteLine("_");
                    writer.WriteLine(encryption(Contain));                   
                    writer.Close();
                  
                }
                else {
                
                    File.AppendAllText(path, encryption(Contain) + Environment.NewLine);
                }
            }
            catch
            {
              
            }
        }
        public static string ScreenRes()
        {
            string strscrres = string.Empty;

            int minx, miny, maxx, maxy;
            minx = miny = int.MaxValue;
            maxx = maxy = int.MinValue;

            foreach (Screen screen in Screen.AllScreens)
            {
                var bounds = screen.Bounds;
                minx = Math.Min(minx, bounds.X);
                miny = Math.Min(miny, bounds.Y);
                maxx = Math.Max(maxx, bounds.Right);
                maxy = Math.Max(maxy, bounds.Bottom);
            }
            var v = maxx - minx;
            var v1 = maxy - miny;
            strscrres = v + "x" + v1;
            string ss = Screen.PrimaryScreen.WorkingArea.Width.ToString();
            return strscrres;


        }
        public static bool IsConnectedToInternet()
        {
            string host = "google.com";
            bool result = false;
            Ping p = new Ping();
            try
            {
                PingReply reply = p.Send(host, 3000);
                if (reply.Status == IPStatus.Success)
                    return true;
            }
            catch (Exception exe)
            {

            }
            return result;
        }

        public static string Get_IP()
        {
            string strHostName;
            strHostName = Dns.GetHostName();
            IPHostEntry ip = Dns.GetHostByName(strHostName);
            IPAddress[] addr = ip.AddressList;
            int i;
            string p = string.Empty;
            for (i = 0; i < addr.Length; i++)
            {
                p = addr[i].ToString();
            }
            return p;
        }
        public static bool AcceptOnlyDigit(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && (e.KeyChar != (char)Keys.Back))
                return true;
            else
                return false;
        }
        public static string GetTodayDate()
        {
            return DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
        }
        public static bool screenset(string screenWidth, string screenHeight)
        {
            try
            {

                //  MessageBox.Show("Need to Change Screen Resolution For IT EXAM");
                try
                {
                    MyTactics.blogspot.com.NewResolution n =
                    new MyTactics.blogspot.com.NewResolution(Convert.ToInt32(screenWidth), Convert.ToInt32(screenHeight));
                    return true;
                }
                catch (Exception exe)
                {
                    return false;
                }
                // writereg();

            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public static bool screenset_default(string screenWidth, string screenHeight)
        {
            try
            {

                //  MessageBox.Show("Need to Change Screen Resolution For IT EXAM");
                try
                {
                    MyTactics.blogspot.com.NewResolution n =
                    new MyTactics.blogspot.com.NewResolution(Convert.ToInt32(screenWidth), Convert.ToInt32(screenHeight));
                    return true;
                }
                catch (Exception exe)
                {
                    return false;
                }
                // writereg();

            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public static void SetRegistryKey(Microsoft.Win32.RegistryKey regHive, string regKey, string regName, string regValue)
        {
            bool response = false;

            Microsoft.Win32.RegistryKey key = regHive.OpenSubKey(regKey);
            if (key == null)
            {
                regHive.CreateSubKey(regKey, Microsoft.Win32.RegistryKeyPermissionCheck.ReadWriteSubTree);
            }
            key = regHive.OpenSubKey(regKey, true);
            key.SetValue(regName, (string)regValue);
        }

        public static void KillCtrlAltDelete()
        {
            try
            {
                SetRegistryKey(Microsoft.Win32.Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "DisableTaskMgr","1");
            }
            catch (Exception exe)
            {

            }
        }
        public static void AllowCtrlAltDelete()
        {
            try
            {
                RegistryKey rkey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", true);
                rkey.DeleteValue("DisableTaskMgr", true);
                //  rkey.Close();
                try
                {
                    RegistryKey rkey2 = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", true);
                    rkey.DeleteValue("DisableTaskMgr", true);
                    rkey2.Close();
                }
                catch
                {

                }
            }
            catch (Exception exe)
            {

            }

            //MessageBox.Show("Registry Enabled !!");
        }

      
    }
}
