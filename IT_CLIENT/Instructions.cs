using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;


namespace IT_CLIENT
{
    public partial class Instructions : Form
    {
        DataTable dtthis = new DataTable();
        string session = "";
        public Instructions()
        {
            InitializeComponent();
            
        }
        public Instructions(DataTable dt,string esession)
        {
            InitializeComponent();

            if (dt.Rows.Count > 0)
            {
                lblseat.Text = dt.Rows[0]["Seatno"].ToString();
                dtthis = dt;
                session = esession;

                timer1.Start();

            }
        }

      
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            DateTime dt1 = DateTime.Now;
            lbltime.Text = dt1.ToString();

            if (session.Equals("Morning"))
            {
                Check_Login_Time();
            }
            else if (session.Equals("Afternoon"))
            {
                Check_Login_Time();
            }
            timer1.Start();

        }
        public void Check_Login_Time()
        {
            int HOUR = DateTime.Now.Hour, MINUTE = DateTime.Now.Minute, SECONDS = DateTime.Now.Second;
            DateTime dateTime = DateTime.Now;
            string sysUIFormat = DateTime.Now.ToString("tt", CultureInfo.InvariantCulture);
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
            //MINUTE = Convert.ToInt32(dt.Rows[0]["MIN"].ToString());

            if (HOUR >= 10 && HOUR <= 13) //Morning Batch
            {
                if (HOUR == 10)
                {
                    if (MINUTE >= 50 && MINUTE <= 59)
                    {
                        //LOGIN
                        timer1.Stop();
                        Start_Btn.Visible = true;
                    }

                }
                else
                {
                    timer1.Stop();
                    Start_Btn.Visible = true;
                }



            }
            else if (HOUR >= 14 && HOUR <= 17) //Afternoon Batch
            {
                if (HOUR == 14)
                {
                    if (MINUTE >= 50 && MINUTE <= 59)
                    {
                        timer1.Stop();
                        Start_Btn.Visible = true;
                    }

                }
                else
                {
                    timer1.Stop();
                    Start_Btn.Visible = true;
                }


            }
            

        }

      
        private void Start_Btn_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            Final_Exam FE = new Final_Exam(dtthis, session);
            this.Hide();
            FE.Show();
        }
    }

}
