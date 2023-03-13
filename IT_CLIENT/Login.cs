using IT_CLIENT.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace IT_CLIENT
{
    public partial class Login : Form
    {
        Exam_WebService.Exam_IT webervice_it = new Exam_WebService.Exam_IT(); 
        DataSet ds = new DataSet();
        Taskbar Tsk = new Taskbar();
        StartBtn stbtn = new StartBtn();
        const int MF_BYPOSITION = 0x400;
        string esession = "";
        Thread child;
            Instructions ins;
        int flag = 0;
        DataTable dt_time;
        [DllImport("User32")]        
        private static extern int RemoveMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("User32")]

        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("User32")]

        private static extern int GetMenuItemCount(IntPtr hWnd);
        public Login()
        {
            InitializeComponent();
        }
        public Login(DataTable dt, string Index_No)
        {
            InitializeComponent();
            lbldate.Text = Common.GetTodayDate();
            lblclg.Text = Index_No;
            rdbsci.Checked = false;
            dt_time = dt;
            webervice_it.Url = Common.GetServerURLBy_Cindex(Index_No.Replace("J", "").Substring(0, 2), Index_No.Replace("J", "").Substring(3, 2)) +"Exam_IT.asmx";
          
        }
      
        public string GetPaper(string Url)
        {

            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(Url);
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            string result = sr.ReadToEnd();
            sr.Close();
            myResponse.Close();

            return result;
        }
        public void CheckExamTime()
        {
            try
            {
                string ss = lblclg.Text;
                string stream = "";
                if (rdbsci.Checked){stream = "97";}
                else if (rdbcom.Checked){stream = "99";}
                else if (rdbart.Checked){stream = "98";}

                int HOUR = 0, MINUTE = 0, SECONDS = 0;
                string sysUIFormat = dt_time.Rows[0]["AMPM"].ToString(), Batch = "";
                MINUTE = Convert.ToInt32(dt_time.Rows[0]["MIN"].ToString());
                HOUR = Convert.ToInt32(dt_time.Rows[0]["HR"].ToString());
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

                    if (dt_time.Rows[0]["DD"].ToString() == "11") { Batch = "B1"; }
                    else if (dt_time.Rows[0]["DD"].ToString() == "12") { Batch = "B3"; }
                    else if (dt_time.Rows[0]["DD"].ToString() == "13") { Batch = "B5"; }
                    else if (dt_time.Rows[0]["DD"].ToString() == "14") { Batch = "B7"; }

                }
                else if (HOUR >= 14 && HOUR <= 17) //Afternoon Batch
                {

                    if (dt_time.Rows[0]["DD"].ToString() == "11") { Batch = "B2"; }
                    else if (dt_time.Rows[0]["DD"].ToString() == "12") { Batch = "B4"; }
                    else if (dt_time.Rows[0]["DD"].ToString() == "13") { Batch = "B6"; }
                    else if (dt_time.Rows[0]["DD"].ToString() == "14") { Batch = "B8"; }
                }
                else
                {

                }
               
                string seatno = "", sign = "", clg = "";
                try
                {                   
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        clg = lblclg.Text;
                        seatno = txtseat.Text;
                        sign = txtsign.Text;
                    });
                }
                catch (Exception exe)
                {
                    clg = lblclg.Text;
                    seatno = txtseat.Text;
                    sign = txtsign.Text;
                }
                ds = webervice_it.Login(clg.Replace("J", ""), seatno, sign, Common.Get_IP(), stream,Batch,Common.GetMAC());
                if (ds.Tables["Status"].Rows.Count > 0)
                {
                    if (ds.Tables["Status"].Rows[0]["Status"].ToString() == "SUCCESS")
                    {
                        if (ds.Tables["Login_Details"].Rows[0]["SeatNo"].ToString().Trim() != txtseat.Text.Trim())
                        {
                            MessageBox.Show("Retry Login... !!! Server Might be Busy");
                            btnlogin.Enabled = true;
                            Exit.Enabled = true;
                            return;
                        }
                        MessageBox.Show(ds.Tables["Status"].Rows[0]["Message"].ToString());
                        string batch = ds.Tables["Login_Details"].Rows[0]["Batch"].ToString().Trim();

                        if ((batch.Equals("B1")) || (batch.Equals("B3")) || (batch.Equals("B5")) || (batch.Equals("B7")))
                        {
                            esession = "Morning";
                        }
                        else if ((batch.Equals("B2")) || (batch.Equals("B4")) || (batch.Equals("B6")) || (batch.Equals("B8")))
                        {
                            esession = "Afternoon";
                        }
                        else
                        {
                            MessageBox.Show("Invalid Batch ");
                            Application.Exit();
                        }


                        ds.Tables["Login_Details"].Columns.Add("Paper", typeof(string));
                        string PageText = GetPaper(""+Common.GetServerURLBy_Cindex(lblclg.Text.Replace("J", "").Substring(0, 2), lblclg.Text.Replace("J", "").Substring(3, 2)) +"TXT/"+ ds.Tables["Login_Details"].Rows[0]["Paper_Id"].ToString() + ".txt");
                        if (PageText.Equals(""))
                        {
                            MessageBox.Show("No Internet Connection Rerun Application Again!!! Error Code:- 1000");
                        }
                        else
                        {
                            ds.Tables["Login_Details"].Columns.Add("PageText", typeof(string));
                            ds.Tables["Login_Details"].Rows[0]["PageText"] = PageText;
                            if (Convert.ToInt32((ds.Tables["Login_Details"].Rows[0]["Relogin_Count"].ToString() == "") ? "0" : ds.Tables["Login_Details"].Rows[0]["Relogin_Count"].ToString()) > 1)
                            {
                                Common.WriteAnsFile(ds.Tables["Login_Details"].Rows[0]["SeatNo"].ToString(), "Relogin ");
                            }
                            else
                            {
                                Common.WriteAnsFile(ds.Tables["Login_Details"].Rows[0]["SeatNo"].ToString(), "Start of the file");
                            }
                                
                            Common.WriteAnsFile(ds.Tables["Login_Details"].Rows[0]["SeatNo"].ToString(), ds.Tables["Login_Details"].Rows[0]["SeatNo"].ToString());
                            Common.WriteAnsFile(ds.Tables["Login_Details"].Rows[0]["SeatNo"].ToString(),  ds.Tables["Status"].Rows[0]["Message"].ToString());
                            Common.WriteAnsFile(ds.Tables["Login_Details"].Rows[0]["SeatNo"].ToString(), "¶" + ds.Tables["Login_Details"].Rows[0]["Paper_ID"].ToString());
                            Common.WriteAnsFile(ds.Tables["Login_Details"].Rows[0]["SeatNo"].ToString(), DateTime.Now.ToString()+"");

                            flag = 1;
                            timer1.Start();


                        }
                    }
                    else
                    {
                        MessageBox.Show(ds.Tables["Status"].Rows[0]["Message"].ToString());

                        this.Invoke((MethodInvoker)delegate ()
                        {
                            btnlogin.Text = "Login";
                            btnlogin.Enabled = true;
                            Exit.Enabled = true;
                        });


                    }
                }
                else
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        btnlogin.Text = "Login";
                        Exit.Enabled = true;
                    });
                    MessageBox.Show("Login Fail");
                }

            }
            catch (Exception exe)
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    btnlogin.Text = "Login";
                    Exit.Enabled = true;
                });

                MessageBox.Show("Please Check Internet Connectivity");
            }

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (flag == 1)
            {
                this.timer1.Stop();
                ins = new Instructions(ds.Tables["Login_Details"], esession);
                ins.Show();

                this.Close();

            }
        }
        public string Mac_Add()
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
        private void btnlogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (rdbsci.Checked == false && rdbcom.Checked == false && rdbart.Checked == false)
                {
                    MessageBox.Show("Please select one of the streams");
                    return;
                }
                if (txtseat.Text == "")
                {
                    MessageBox.Show("Enter Valid Student Number.");
                    return;
                }
                if (!Char.IsLetter(txtseat.Text[0]))
                {
                    MessageBox.Show("Enter first character is letter in Seat no entry");
                    return;
                }
                if (!Char.IsDigit(txtseat.Text[1]))
                {
                    MessageBox.Show("Enter second character is digit in seatno entry");
                    return;
                }
                if (!Char.IsDigit(txtseat.Text[2]))
                {
                    MessageBox.Show("Enter third character is digit in seatno entry");
                    return;
                }
                if (!Char.IsDigit(txtseat.Text[3]))
                {
                    MessageBox.Show("Enter fourth character is digit in seatno entry ");
                    return;
                }
                if (!Char.IsDigit(txtseat.Text[4]))
                {
                    MessageBox.Show("Enter fifth character is digit in seatno entry");
                    return;
                }
                if (!Char.IsDigit(txtseat.Text[5]))
                {
                    MessageBox.Show("Enter sixth character is digit in seatno entry");
                    return;
                }
                if (!Char.IsDigit(txtseat.Text[6]))
                {
                    MessageBox.Show("Enter seventh character is digit in seatno entry");
                    return;
                }
                if (txtsign.Text == "")
                {
                    MessageBox.Show("Please write Password.");
                    return;
                }
                if (txtsign.Text.Length < 7)
                {
                    MessageBox.Show("Password Should Not Less than 8 Digit");
                    return;
                }
                else
                {
                    flag = 0;
                    timer1.Start();
                    btnlogin.Enabled = false;
                    child = new Thread(CheckExamTime);
                    child.IsBackground = false;
                    btnlogin.Text = "Wait..";
                    Exit.Enabled = false;
                    child.Start();


                    if (rdbsci.Checked)
                    {
                       

                    }
                    else if (rdbcom.Checked)
                    {
                      

                    }
                    else if (rdbart.Checked)
                    {
                       


                    }
                }


            }


            catch (Exception ae)
            {
                //  MessageBox.Show("ERROR ");
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            ShowControl();
            string line1 = "";
            using (StreamReader reader = new StreamReader(@"c:\ITEXAM" + Resources.ResourceManager.GetString("Registry_Name") + "\\screen.txt"))
            {
                line1 = reader.ReadLine() ?? "";
            }
            string[] spscreen = line1.Split('x');
            Common.screenset_default(spscreen[0], spscreen[1]);
            Application.Exit();
        }
        private void ShowControl()
        {
            Tsk.Show();
            Common.AllowCtrlAltDelete();
            stbtn.Show();
        }
    }
}
