using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using IT_CLIENT.Class;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using IT_CLIENT.Properties;
using System.Globalization;
using System.Management;

namespace IT_CLIENT
{

    public partial class Final_Exam : Form
    {
        public TimeSpan TimeLeft;
        DataTable QDT1, QDT2, QDT3A, QDT3B;
        GetDataTable_Question getDataTable_Question = new GetDataTable_Question();
        string[] qarray = new string[5000];
        Exam_WebService.Exam_IT webervice_it = new Exam_WebService.Exam_IT();
        Thread Thred1, Thred2, Thred3A, Thred3B, Thred4, Thred5, Thred6, Thred7, Thred8, Thred9, Thred10;
        int Internet_Check = 1, Timeflag = 0;
        Taskbar Tsk = new Taskbar();
        StartBtn stbtn = new StartBtn();
        Common cmn = new Common();

        public Final_Exam(DataTable dt, string esession)
        {
            InitializeComponent();

            webervice_it.Url = Common.GetServerURLBy_Cindex(dt.Rows[0]["Index_No"].ToString().Replace("J", "").Substring(0, 2), dt.Rows[0]["Index_No"].ToString().Replace("J", "").Substring(3, 2)) + "Exam_IT.asmx";
            Timeflag = 0;
            Disable_Control();
            Check_Login_Time();
            batchlbl.Text = dt.Rows[0]["batch"].ToString();
            DataTable ServerTime = webervice_it.GetServerTime();
            if (ServerTime.Rows.Count > 0)
            {
                int HOUR = Convert.ToInt32(ServerTime.Rows[0]["HR"].ToString()), MINUTE = Convert.ToInt32(ServerTime.Rows[0]["MIN"].ToString());
                string sysUIFormat = ServerTime.Rows[0]["AMPM"].ToString();
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
                if (esession.Equals("Morning"))
                {
                    TimeLeft = TimeSpan.Parse("13:30:00") - TimeSpan.Parse("" + HOUR + ":" + MINUTE + ":00");
                }
                else if (esession.Equals("Afternoon"))
                {
                    TimeLeft = TimeSpan.Parse("17:30:00") - TimeSpan.Parse("" + HOUR + ":" + MINUTE + ":00");
                }
                if (dt.Rows[0]["HAND"].ToString().Trim() == "1" || dt.Rows[0]["HAND"].ToString().Trim() == "4") { TimeLeft = TimeLeft + TimeSpan.Parse("00:50:00"); }
                else if (dt.Rows[0]["HAND"].ToString().Trim() == "2" || dt.Rows[0]["HAND"].ToString().Trim() == "3" || dt.Rows[0]["HAND"].ToString().Trim() == "8" || dt.Rows[0]["HAND"].ToString().Trim() == "9") { TimeLeft = TimeLeft + TimeSpan.Parse("00:30:00"); }
                else if (dt.Rows[0]["HAND"].ToString().Trim() == "5") { TimeLeft = TimeLeft + TimeSpan.Parse("02:00:00"); }
                else if (dt.Rows[0]["HAND"].ToString().Trim() == "6" || dt.Rows[0]["HAND"].ToString().Trim() == "7") { TimeLeft = TimeLeft + TimeSpan.Parse("01:00:00"); }
                else if (dt.Rows[0]["HAND"].ToString().Trim() == "11") { TimeLeft = TimeLeft + TimeSpan.Parse("00:10:00"); }
                else if (dt.Rows[0]["HAND"].ToString().Trim() == "12") { TimeLeft = TimeLeft + TimeSpan.Parse("00:10:00"); }
            }
            else
            {
                MessageBox.Show("Unable To Connect Server Retry !!!");
                ShowControl();
                Application.Exit();
            }


            timer2.Start();
            Internet_Check_Timer.Start();
            timer1.Start();
            lblcat.Text = dt.Rows[0]["HAND"].ToString();
            QDT1 = getDataTable_Question.Get_First_Question();
            QDT2 = getDataTable_Question.Get_First_Question();
            LoadQuestionPaper(dt.Rows[0]["PageText"].ToString());
            lbl_seat.Text = dt.Rows[0]["seatno"].ToString();
            lbl_paperid.Text = dt.Rows[0]["paper_id"].ToString();
            lbl_Subject_Code.Text = dt.Rows[0]["stream"].ToString();
            lbl_Index.Text = dt.Rows[0]["Index_No"].ToString();
            lbl_filesize.Text = Common.AnsFileSize(lbl_seat.Text) + " Bytes";
            lbl_qnamebtn.Text = "Question No 1. FILL IN THE BLANKS";
            try
            {
                if (Convert.ToInt32((dt.Rows[0]["Relogin_Count"].ToString() == "") ? "0" : dt.Rows[0]["Relogin_Count"].ToString()) > 1)
                {
                    MessageBox.Show("ReLogin Detect Please Wait ...!!!");
                    Thred9 = new Thread(Read_ReloginData);
                    Thred9.Start();
                }
            }
            catch (Exception exe)
            {

            }

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

            if (HOUR >= 11 && HOUR <= 13) //Morning Batch
            {
                if (HOUR == 11)
                {
                    Enable_Control(); Timeflag = 1;

                }
                else if (HOUR == 13)
                {
                    if (MINUTE >= 0 && MINUTE <= 30)
                    {
                        Enable_Control(); Timeflag = 1;
                    }
                }
                else
                {
                    Enable_Control(); Timeflag = 1;
                }



            }
            else if (HOUR >= 15 && HOUR <= 17) //Afternoon Batch
            {
                if (HOUR == 15)
                {
                    Enable_Control(); Timeflag = 1;

                }
                else if (HOUR == 17)
                {
                    if (MINUTE >= 0 && MINUTE <= 30)
                    {
                        Enable_Control(); Timeflag = 1;
                    }
                }
                else
                {
                    Enable_Control(); Timeflag = 1;
                }


            }


        }
        public void LoadQuestionPaper(string Paper_Text)
        {
            try
            {
                char[] delimiters = new char[] { '¶', '~' };
                qarray = Paper_Text.Split(delimiters);

                que1.Text = qarray[0].ToString();
                que2.Text = qarray[1].ToString();
                que3.Text = qarray[2].ToString();
                que4.Text = qarray[3].ToString();
                que5.Text = qarray[4].ToString();
                que6.Text = qarray[5].ToString();
                que7.Text = qarray[6].ToString();
                que8.Text = qarray[7].ToString();
                que9.Text = qarray[8].ToString();
                que10.Text = qarray[9].ToString();
                QDT1.Rows.Add(1, qarray[0].ToString(), "");
                QDT1.Rows.Add(2, qarray[1].ToString(), "");
                QDT1.Rows.Add(3, qarray[2].ToString(), "");
                QDT1.Rows.Add(4, qarray[3].ToString(), "");
                QDT1.Rows.Add(5, qarray[4].ToString(), "");
                QDT1.Rows.Add(6, qarray[5].ToString(), "");
                QDT1.Rows.Add(7, qarray[6].ToString(), "");
                QDT1.Rows.Add(8, qarray[7].ToString(), "");
                QDT1.Rows.Add(9, qarray[8].ToString(), "");
                QDT1.Rows.Add(10, qarray[9].ToString(), "");

                //  **********assign the question -tf***************************
                que11.Text = qarray[10].ToString();
                que12.Text = qarray[11].ToString();
                que13.Text = qarray[12].ToString();
                que14.Text = qarray[13].ToString();
                que15.Text = qarray[14].ToString();
                que16.Text = qarray[15].ToString();
                que17.Text = qarray[16].ToString();
                que18.Text = qarray[17].ToString();
                que19.Text = qarray[18].ToString();
                que20.Text = qarray[19].ToString();
                QDT2.Rows.Add(1, qarray[10].ToString(), "");
                QDT2.Rows.Add(2, qarray[11].ToString(), "");
                QDT2.Rows.Add(3, qarray[12].ToString(), "");
                QDT2.Rows.Add(4, qarray[13].ToString(), "");
                QDT2.Rows.Add(5, qarray[14].ToString(), "");
                QDT2.Rows.Add(6, qarray[15].ToString(), "");
                QDT2.Rows.Add(7, qarray[16].ToString(), "");
                QDT2.Rows.Add(8, qarray[17].ToString(), "");
                QDT2.Rows.Add(9, qarray[18].ToString(), "");
                QDT2.Rows.Add(10, qarray[19].ToString(), "");

                //  **********assign the question -mcq1***************************
                que21.Text = qarray[20].ToString();
                Q3AR1Q1.Text = qarray[21].ToString();
                Q3AR2Q1.Text = qarray[22].ToString();
                Q3AR3Q1.Text = qarray[23].ToString();
                Q3AR4Q1.Text = qarray[24].ToString();


                que22.Text = qarray[25].ToString();
                Q3AR1Q2.Text = qarray[26].ToString();
                Q3AR2Q2.Text = qarray[27].ToString();
                Q3AR3Q2.Text = qarray[28].ToString();
                Q3AR4Q2.Text = qarray[29].ToString();

                que23.Text = qarray[30].ToString();
                Q3AR1Q3.Text = qarray[31].ToString();
                Q3AR2Q3.Text = qarray[32].ToString();
                Q3AR3Q3.Text = qarray[33].ToString();
                Q3AR4Q3.Text = qarray[34].ToString();

                que24.Text = qarray[35].ToString();
                Q3AR1Q4.Text = qarray[36].ToString();
                Q3AR2Q4.Text = qarray[37].ToString();
                Q3AR3Q4.Text = qarray[38].ToString();
                Q3AR4Q4.Text = qarray[39].ToString();

                que25.Text = qarray[40].ToString();
                Q3AR1Q5.Text = qarray[41].ToString();
                Q3AR2Q5.Text = qarray[42].ToString();
                Q3AR3Q5.Text = qarray[43].ToString();
                Q3AR4Q5.Text = qarray[44].ToString();

                que26.Text = qarray[45].ToString();
                Q3AR1Q6.Text = qarray[46].ToString();
                Q3AR2Q6.Text = qarray[47].ToString();
                Q3AR3Q6.Text = qarray[48].ToString();
                Q3AR4Q6.Text = qarray[49].ToString();

                que27.Text = qarray[50].ToString();
                Q3AR1Q7.Text = qarray[51].ToString();
                Q3AR2Q7.Text = qarray[52].ToString();
                Q3AR3Q7.Text = qarray[53].ToString();
                Q3AR4Q7.Text = qarray[54].ToString();

                que28.Text = qarray[55].ToString();
                Q3AR1Q8.Text = qarray[56].ToString();
                Q3AR2Q8.Text = qarray[57].ToString();
                Q3AR3Q8.Text = qarray[58].ToString();
                Q3AR4Q8.Text = qarray[59].ToString();

                que29.Text = qarray[60].ToString();
                Q3AR1Q9.Text = qarray[61].ToString();
                Q3AR2Q9.Text = qarray[62].ToString();
                Q3AR3Q9.Text = qarray[63].ToString();
                Q3AR4Q9.Text = qarray[64].ToString();

                que30.Text = qarray[65].ToString();
                Q3AR1Q10.Text = qarray[66].ToString();
                Q3AR2Q10.Text = qarray[67].ToString();
                Q3AR3Q10.Text = qarray[68].ToString();
                Q3AR4Q10.Text = qarray[69].ToString();

                //  **********assign the question -mcq2***************************

                que31.Text = qarray[70].ToString();
                Q3BR1Q11.Text = qarray[71].ToString();
                Q3BR2Q11.Text = qarray[72].ToString();
                Q3BR3Q11.Text = qarray[73].ToString();
                Q3BR4Q11.Text = qarray[74].ToString();

                que32.Text = qarray[75].ToString();
                Q3BR1Q12.Text = qarray[76].ToString();
                Q3BR2Q12.Text = qarray[77].ToString();
                Q3BR3Q12.Text = qarray[78].ToString();
                Q3BR4Q12.Text = qarray[79].ToString();

                que33.Text = qarray[80].ToString();
                Q3BR1Q13.Text = qarray[81].ToString();
                Q3BR2Q13.Text = qarray[82].ToString();
                Q3BR3Q13.Text = qarray[83].ToString();
                Q3BR4Q13.Text = qarray[84].ToString();

                que34.Text = qarray[85].ToString();
                Q3BR1Q14.Text = qarray[86].ToString();
                Q3BR2Q14.Text = qarray[87].ToString();
                Q3BR3Q14.Text = qarray[88].ToString();
                Q3BR4Q14.Text = qarray[89].ToString();

                que35.Text = qarray[90].ToString();
                Q3BR1Q15.Text = qarray[91].ToString();
                Q3BR2Q15.Text = qarray[92].ToString();
                Q3BR3Q15.Text = qarray[93].ToString();
                Q3BR4Q15.Text = qarray[94].ToString();

                que36.Text = qarray[95].ToString();
                Q3BR1Q16.Text = qarray[96].ToString();
                Q3BR2Q16.Text = qarray[97].ToString();
                Q3BR3Q16.Text = qarray[98].ToString();
                Q3BR4Q16.Text = qarray[99].ToString();

                que37.Text = qarray[100].ToString();
                Q3BR1Q17.Text = qarray[101].ToString();
                Q3BR2Q17.Text = qarray[102].ToString();
                Q3BR3Q17.Text = qarray[103].ToString();
                Q3BR4Q17.Text = qarray[104].ToString();

                que38.Text = qarray[105].ToString();
                Q3BR1Q18.Text = qarray[106].ToString();
                Q3BR2Q18.Text = qarray[107].ToString();
                Q3BR3Q18.Text = qarray[108].ToString();
                Q3BR4Q18.Text = qarray[109].ToString();

                que39.Text = qarray[110].ToString();
                Q3BR1Q19.Text = qarray[111].ToString();
                Q3BR2Q19.Text = qarray[112].ToString();
                Q3BR3Q19.Text = qarray[113].ToString();
                Q3BR4Q19.Text = qarray[114].ToString();

                que40.Text = qarray[115].ToString();
                Q3BR1Q20.Text = qarray[116].ToString();
                Q3BR2Q20.Text = qarray[117].ToString();
                Q3BR3Q20.Text = qarray[118].ToString();
                Q3BR4Q20.Text = qarray[119].ToString();

                //  **********assign the question -mcq4***************************
                que41.Text = qarray[120].ToString();
                cbox81.Text = qarray[121].ToString();
                cbox82.Text = qarray[122].ToString();
                cbox83.Text = qarray[123].ToString();
                cbox84.Text = qarray[124].ToString();
                cbox85.Text = qarray[125].ToString();
                cbox86.Text = qarray[126].ToString();

                que42.Text = qarray[127].ToString();
                cbox87.Text = qarray[128].ToString();
                cbox88.Text = qarray[129].ToString();
                cbox89.Text = qarray[130].ToString();
                cbox90.Text = qarray[131].ToString();
                cbox91.Text = qarray[132].ToString();
                cbox92.Text = qarray[133].ToString();

                que43.Text = qarray[134].ToString();

                cbox93.Text = qarray[135].ToString();
                cbox94.Text = qarray[136].ToString();
                cbox95.Text = qarray[137].ToString();
                cbox96.Text = qarray[138].ToString();
                cbox97.Text = qarray[139].ToString();
                cbox98.Text = qarray[140].ToString();

                que44.Text = qarray[141].ToString();

                cbox99.Text = qarray[142].ToString();
                cbox100.Text = qarray[143].ToString();
                cbox101.Text = qarray[144].ToString();
                cbox102.Text = qarray[145].ToString();
                cbox103.Text = qarray[146].ToString();
                cbox104.Text = qarray[147].ToString();

                que45.Text = qarray[148].ToString();

                cbox105.Text = qarray[149].ToString();
                cbox106.Text = qarray[150].ToString();
                cbox107.Text = qarray[151].ToString();
                cbox108.Text = qarray[152].ToString();
                cbox109.Text = qarray[153].ToString();
                cbox110.Text = qarray[154].ToString();

                //  **********assign the question -mcq4***************************
                que46.Text = qarray[155].ToString();
                cbox_111.Text = qarray[156].ToString();
                cbox_112.Text = qarray[157].ToString();
                cbox_113.Text = qarray[158].ToString();
                cbox_114.Text = qarray[159].ToString();
                cbox_115.Text = qarray[160].ToString();
                cbox_116.Text = qarray[161].ToString();

                ques47.Text = qarray[162].ToString();
                cbox_117.Text = qarray[163].ToString();
                cbox_118.Text = qarray[164].ToString();
                cbox_119.Text = qarray[165].ToString();
                cbox_120.Text = qarray[166].ToString();
                cbox_121.Text = qarray[167].ToString();
                cbox_122.Text = qarray[168].ToString();

                lbl_reg1.Text = qarray[169].ToString();
                lbl_regop1.Text = qarray[170].ToString();
                lbl_regop2.Text = qarray[171].ToString();

                lbl_regop3.Text = qarray[172].ToString();
                lbl_regop4.Text = qarray[173].ToString();
                lbl_regop5.Text = qarray[174].ToString();
                lbl_regop6.Text = qarray[175].ToString();
                lbl_regop7.Text = qarray[176].ToString();
                //  lbl_regop8.Text = qarray[177].ToString();


                lbl_reg2.Text = qarray[177].ToString();
                lbl_regop_21.Text = qarray[178].ToString();
                lbl_regop_22.Text = qarray[179].ToString();
                lbl_regop_23.Text = qarray[180].ToString();
                lbl_regop_24.Text = qarray[181].ToString();
                lbl_regop_25.Text = qarray[182].ToString();
                lbl_regop_26.Text = qarray[183].ToString();
                lbl_regop_27.Text = qarray[184].ToString();
                // lbl_regop_28.Text = qarray[186].ToString();

                //// **********assign the question -q7***************************

                que48.Text = qarray[185].ToString();
                que49.Text = qarray[186].ToString();
                que50.Text = qarray[187].ToString();

                //// **********assign the question -q8***************************
                que51.Text = qarray[188].ToString();
                que52.Text = qarray[189].ToString();
            }
            catch (Exception exe)
            {
                MessageBox.Show("Unable to Load Question Paper Please Close the Client & Relogin");
                btnsub.Enabled = true;
            }
        }
        private void ShowControl()
        {
            Tsk.Show();
            Common.AllowCtrlAltDelete();
            stbtn.Show();
        }

        private void btnsub_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are You Sure You Want To Finish Exam", "Finish Exam Confirmation", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                WriteFinalAns();
                GiveUp();

                //do something
            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }

        }
        private void GiveUp()
        {
            string value = "";

            if (InputBox.Show("Invigilator Confirmation", "Enter Invigilator Password:", ref value) == DialogResult.OK)
            {
                try
                {
                 
                    try
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(Common.GetAnsFilePath(lbl_seat.Text));
                        string filesizebyte = (bytes.Length).ToString();
                        string ss = webervice_it.UploadFile(lbl_seat.Text, batchlbl.Text, bytes);
                    }
                    catch (Exception exe)
                    {

                    }
                    DataSet ds = new DataSet();
                    ds = webervice_it.Login(lbl_Index.Text.Replace("J", ""), lbl_seat.Text, value, Common.Get_IP(), lbl_Subject_Code.Text, batchlbl.Text, Common.GetMAC());
                    if (ds.Tables["Status"].Rows.Count > 0)
                    {
                        if (ds.Tables["Status"].Rows[0]["Status"].ToString() == "SUCCESS")
                        {
                            AllClose();
                        }
                        else
                        {
                            MessageBox.Show("Inavalid Password");
                            GiveUp();
                        }
                    }
                    else
                    {
                        if (batchlbl.Text == value.Substring(3, 1) + value.Substring(6, 1))
                        {
                            AllClose();
                        }
                        else
                        {
                            MessageBox.Show("Inavalid Password");
                            GiveUp();
                        }
                    }
                }
                catch (Exception exe)
                {
                    AllClose();
                }
            }
        }

        private void AllClose()
        {
            try
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
            catch (Exception exe)
            {
                Application.Exit();
            }
        }

        public void WriteFinalAns()
        {
            try
            {
                Common.WriteAnsFile(lbl_seat.Text, "¶FINAL_ANS GIVEUP VERIFY BY INVIGILATOR");
                Common.WriteAnsFile(lbl_seat.Text, "¶Q1~" + DateTime.Now.ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!1!" + txtans1.Text);
                Common.WriteAnsFile(lbl_seat.Text, "!2!" + txtans2.Text);
                Common.WriteAnsFile(lbl_seat.Text, "!3!" + txtans3.Text);
                Common.WriteAnsFile(lbl_seat.Text, "!4!" + txtans4.Text);
                Common.WriteAnsFile(lbl_seat.Text, "!5!" + txtans5.Text);
                Common.WriteAnsFile(lbl_seat.Text, "!6!" + txtans6.Text);
                Common.WriteAnsFile(lbl_seat.Text, "!7!" + txtans7.Text);
                Common.WriteAnsFile(lbl_seat.Text, "!8!" + txtans8.Text);
                Common.WriteAnsFile(lbl_seat.Text, "!9!" + txtans9.Text);
                Common.WriteAnsFile(lbl_seat.Text, "!10!" + txtans10.Text);
                if (rb_tf1.Checked == true)
                {
                    QDT2.Rows[0]["Answer"] = "True";
                }
                else if (rb_tf2.Checked == true)
                {
                    QDT2.Rows[0]["Answer"] = "False";
                }
                else
                {
                    QDT2.Rows[0]["Answer"] = "";
                }


                if (rb_tf3.Checked == true)
                {
                    QDT2.Rows[1]["Answer"] = "True";
                }
                else if (rb_tf4.Checked == true)
                {
                    QDT2.Rows[1]["Answer"] = "False";
                }
                else
                {
                    QDT2.Rows[1]["Answer"] = "";
                }


                if (rb_tf5.Checked == true)
                {
                    QDT2.Rows[2]["Answer"] = "True";
                }
                else if (rb_tf6.Checked == true)
                {
                    QDT2.Rows[2]["Answer"] = "False";
                }
                else
                {
                    QDT2.Rows[2]["Answer"] = "";
                }

                if (rb_tf7.Checked == true)
                {
                    QDT2.Rows[3]["Answer"] = "True";
                }
                else if (rb_tf8.Checked == true)
                {
                    QDT2.Rows[3]["Answer"] = "False";
                }
                else
                {
                    QDT2.Rows[3]["Answer"] = "";
                }

                if (rb_tf9.Checked == true)
                {
                    QDT2.Rows[4]["Answer"] = "True";
                }
                else if (rb_tf10.Checked == true)
                {
                    QDT2.Rows[4]["Answer"] = "False";
                }
                else
                {
                    QDT2.Rows[4]["Answer"] = "";
                }

                if (rb_tf11.Checked == true)
                {
                    QDT2.Rows[5]["Answer"] = "True";
                }
                else if (rb_tf12.Checked == true)
                {
                    QDT2.Rows[5]["Answer"] = "False";
                }
                else
                {
                    QDT2.Rows[5]["Answer"] = "";
                }


                if (rb_tf13.Checked == true)
                {
                    QDT2.Rows[6]["Answer"] = "True";
                }
                else if (rb_tf14.Checked == true)
                {
                    QDT2.Rows[6]["Answer"] = "False";
                }
                else
                {
                    QDT2.Rows[6]["Answer"] = "";
                }


                if (rb_tf15.Checked == true)
                {
                    QDT2.Rows[7]["Answer"] = "True";
                }
                else if (rb_tf16.Checked == true)
                {
                    QDT2.Rows[7]["Answer"] = "False";
                }
                else
                {
                    QDT2.Rows[7]["Answer"] = "";
                }

                if (rb_tf17.Checked == true)
                {
                    QDT2.Rows[8]["Answer"] = "True";
                }
                else if (rb_tf18.Checked == true)
                {
                    QDT2.Rows[8]["Answer"] = "False";
                }
                else
                {
                    QDT2.Rows[8]["Answer"] = "";
                }

                if (rb_tf19.Checked == true)
                {
                    QDT2.Rows[9]["Answer"] = "True";
                }
                else if (rb_tf20.Checked == true)
                {
                    QDT2.Rows[9]["Answer"] = "False";
                }
                else
                {
                    QDT2.Rows[9]["Answer"] = "";
                }

                Common.WriteAnsFile(lbl_seat.Text, "¶Q2~" + DateTime.Now.ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!1!" + QDT2.Rows[0]["Answer"].ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!2!" + QDT2.Rows[1]["Answer"].ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!3!" + QDT2.Rows[2]["Answer"].ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!4!" + QDT2.Rows[3]["Answer"].ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!5!" + QDT2.Rows[4]["Answer"].ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!6!" + QDT2.Rows[5]["Answer"].ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!7!" + QDT2.Rows[6]["Answer"].ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!8!" + QDT2.Rows[7]["Answer"].ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!9!" + QDT2.Rows[8]["Answer"].ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!10!" + QDT2.Rows[9]["Answer"]);
                string ans_m1 = "", ans_m2 = "", ans_m3 = "", ans_m4 = "", ans_m5 = "", ans_m6 = "", ans_m7 = "", ans_m8 = "", ans_m9 = "", ans_m10 = "";
                string ans_m11 = "", ans_m12 = "", ans_m13 = "", ans_m14 = "", ans_m15 = "", ans_m16 = "", ans_m17 = "", ans_m18 = "", ans_m19 = "", ans_m20 = "";
                //1
                if (Q3AR1Q1.Checked || Q3AR2Q1.Checked || Q3AR3Q1.Checked || Q3AR4Q1.Checked)
                {
                    ans_m1 += Q3AR1Q1.Checked ? "1" : "~";
                    ans_m1 += Q3AR2Q1.Checked ? "1" : "~";
                    ans_m1 += Q3AR3Q1.Checked ? "1" : "~";
                    ans_m1 += Q3AR4Q1.Checked ? "1" : "~";
                }
                else
                {
                    ans_m1 = "~~~~";
                }
                //1
                if (Q3AR1Q2.Checked || Q3AR2Q2.Checked || Q3AR3Q2.Checked || Q3AR4Q2.Checked)
                {
                    ans_m2 += Q3AR1Q2.Checked ? "1" : "~";
                    ans_m2 += Q3AR2Q2.Checked ? "1" : "~";
                    ans_m2 += Q3AR3Q2.Checked ? "1" : "~";
                    ans_m2 += Q3AR4Q2.Checked ? "1" : "~";
                }
                else
                {
                    ans_m2 = "~~~~";
                }

                //3
                if (Q3AR1Q3.Checked || Q3AR2Q3.Checked || Q3AR3Q3.Checked || Q3AR4Q3.Checked)
                {
                    ans_m3 += Q3AR1Q3.Checked ? "1" : "~";
                    ans_m3 += Q3AR2Q3.Checked ? "1" : "~";
                    ans_m3 += Q3AR3Q3.Checked ? "1" : "~";
                    ans_m3 += Q3AR4Q3.Checked ? "1" : "~";
                }
                else
                {
                    ans_m3 = "~~~~";
                }

                //4
                if (Q3AR1Q4.Checked || Q3AR2Q4.Checked || Q3AR3Q4.Checked || Q3AR4Q4.Checked)
                {
                    ans_m4 += Q3AR1Q4.Checked ? "1" : "~";
                    ans_m4 += Q3AR2Q4.Checked ? "1" : "~";
                    ans_m4 += Q3AR3Q4.Checked ? "1" : "~";
                    ans_m4 += Q3AR4Q4.Checked ? "1" : "~";
                }
                else
                {
                    ans_m4 = "~~~~";
                }

                //5
                if (Q3AR1Q5.Checked || Q3AR2Q5.Checked || Q3AR3Q5.Checked || Q3AR4Q5.Checked)
                {
                    ans_m5 += Q3AR1Q5.Checked ? "1" : "~";
                    ans_m5 += Q3AR2Q5.Checked ? "1" : "~";
                    ans_m5 += Q3AR3Q5.Checked ? "1" : "~";
                    ans_m5 += Q3AR4Q5.Checked ? "1" : "~";
                }
                else
                {
                    ans_m5 = "~~~~";
                }

                //6
                if (Q3AR1Q6.Checked || Q3AR2Q6.Checked || Q3AR3Q6.Checked || Q3AR4Q6.Checked)
                {
                    ans_m6 += Q3AR1Q6.Checked ? "1" : "~";
                    ans_m6 += Q3AR2Q6.Checked ? "1" : "~";
                    ans_m6 += Q3AR3Q6.Checked ? "1" : "~";
                    ans_m6 += Q3AR4Q6.Checked ? "1" : "~";
                }
                else
                {
                    ans_m6 = "~~~~";
                }

                //7
                if (Q3AR1Q7.Checked || Q3AR2Q7.Checked || Q3AR3Q7.Checked || Q3AR4Q7.Checked)
                {
                    ans_m7 += Q3AR1Q7.Checked ? "1" : "~";
                    ans_m7 += Q3AR2Q7.Checked ? "1" : "~";
                    ans_m7 += Q3AR3Q7.Checked ? "1" : "~";
                    ans_m7 += Q3AR4Q7.Checked ? "1" : "~";
                }
                else
                {
                    ans_m7 = "~~~~";
                }


                //8
                if (Q3AR1Q8.Checked || Q3AR2Q8.Checked || Q3AR3Q8.Checked || Q3AR4Q8.Checked)
                {
                    ans_m8 += Q3AR1Q8.Checked ? "1" : "~";
                    ans_m8 += Q3AR2Q8.Checked ? "1" : "~";
                    ans_m8 += Q3AR3Q8.Checked ? "1" : "~";
                    ans_m8 += Q3AR4Q8.Checked ? "1" : "~";
                }
                else
                {
                    ans_m8 = "~~~~";
                }

                //9
                if (Q3AR1Q9.Checked || Q3AR2Q9.Checked || Q3AR3Q9.Checked || Q3AR4Q9.Checked)
                {
                    ans_m9 += Q3AR1Q9.Checked ? "1" : "~";
                    ans_m9 += Q3AR2Q9.Checked ? "1" : "~";
                    ans_m9 += Q3AR3Q9.Checked ? "1" : "~";
                    ans_m9 += Q3AR4Q9.Checked ? "1" : "~";
                }

                else
                {
                    ans_m9 = "~~~~";
                }


                //10
                if (Q3AR1Q10.Checked || Q3AR2Q10.Checked || Q3AR3Q10.Checked || Q3AR4Q10.Checked)
                {
                    ans_m10 += Q3AR1Q10.Checked ? "1" : "~";
                    ans_m10 += Q3AR2Q10.Checked ? "1" : "~";
                    ans_m10 += Q3AR3Q10.Checked ? "1" : "~";
                    ans_m10 += Q3AR4Q10.Checked ? "1" : "~";
                }
                else
                {
                    ans_m10 = "~~~~";
                }
                Common.WriteAnsFile(lbl_seat.Text, "¶Q3A~" + DateTime.Now.ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!1!" + ans_m1);
                Common.WriteAnsFile(lbl_seat.Text, "!2!" + ans_m2);
                Common.WriteAnsFile(lbl_seat.Text, "!3!" + ans_m3);
                Common.WriteAnsFile(lbl_seat.Text, "!4!" + ans_m4);
                Common.WriteAnsFile(lbl_seat.Text, "!5!" + ans_m5);
                Common.WriteAnsFile(lbl_seat.Text, "!6!" + ans_m6);
                Common.WriteAnsFile(lbl_seat.Text, "!7!" + ans_m7);
                Common.WriteAnsFile(lbl_seat.Text, "!8!" + ans_m8);
                Common.WriteAnsFile(lbl_seat.Text, "!9!" + ans_m9);
                Common.WriteAnsFile(lbl_seat.Text, "!10!" + ans_m10);
                if (Q3BR1Q11.Checked || Q3BR2Q11.Checked || Q3BR3Q11.Checked || Q3BR4Q11.Checked)
                {
                    ans_m11 += Q3BR1Q11.Checked ? "1" : "~";
                    ans_m11 += Q3BR2Q11.Checked ? "1" : "~";
                    ans_m11 += Q3BR3Q11.Checked ? "1" : "~";
                    ans_m11 += Q3BR4Q11.Checked ? "1" : "~";
                }
                else
                {
                    ans_m11 = "~~~~";
                }
                //1
                if (Q3BR1Q12.Checked || Q3BR2Q12.Checked || Q3BR3Q12.Checked || Q3BR4Q12.Checked)
                {
                    ans_m12 += Q3BR1Q12.Checked ? "1" : "~";
                    ans_m12 += Q3BR2Q12.Checked ? "1" : "~";
                    ans_m12 += Q3BR3Q12.Checked ? "1" : "~";
                    ans_m12 += Q3BR4Q12.Checked ? "1" : "~";
                }
                else
                {
                    ans_m12 = "~~~~";
                }

                //3
                if (Q3BR1Q13.Checked || Q3BR2Q13.Checked || Q3BR3Q13.Checked || Q3BR4Q13.Checked)
                {
                    ans_m13 += Q3BR1Q13.Checked ? "1" : "~";
                    ans_m13 += Q3BR2Q13.Checked ? "1" : "~";
                    ans_m13 += Q3BR3Q13.Checked ? "1" : "~";
                    ans_m13 += Q3BR4Q13.Checked ? "1" : "~";
                }
                else
                {
                    ans_m13 = "~~~~";
                }

                //4
                if (Q3BR1Q14.Checked || Q3BR2Q14.Checked || Q3BR3Q14.Checked || Q3BR4Q14.Checked)
                {
                    ans_m14 += Q3BR1Q14.Checked ? "1" : "~";
                    ans_m14 += Q3BR2Q14.Checked ? "1" : "~";
                    ans_m14 += Q3BR3Q14.Checked ? "1" : "~";
                    ans_m14 += Q3BR4Q14.Checked ? "1" : "~";
                }
                else
                {
                    ans_m14 = "~~~~";
                }

                //5
                if (Q3BR1Q15.Checked || Q3BR2Q15.Checked || Q3BR3Q15.Checked || Q3BR4Q15.Checked)
                {
                    ans_m15 += Q3BR1Q15.Checked ? "1" : "~";
                    ans_m15 += Q3BR2Q15.Checked ? "1" : "~";
                    ans_m15 += Q3BR3Q15.Checked ? "1" : "~";
                    ans_m15 += Q3BR4Q15.Checked ? "1" : "~";
                }
                else
                {
                    ans_m15 = "~~~~";
                }

                //6
                if (Q3BR1Q16.Checked || Q3BR2Q16.Checked || Q3BR3Q16.Checked || Q3BR4Q16.Checked)
                {
                    ans_m16 += Q3BR1Q16.Checked ? "1" : "~";
                    ans_m16 += Q3BR2Q16.Checked ? "1" : "~";
                    ans_m16 += Q3BR3Q16.Checked ? "1" : "~";
                    ans_m16 += Q3BR4Q16.Checked ? "1" : "~";
                }
                else
                {
                    ans_m16 = "~~~~";
                }

                //7
                if (Q3BR1Q17.Checked || Q3BR2Q17.Checked || Q3BR3Q17.Checked || Q3BR4Q17.Checked)
                {
                    ans_m17 += Q3BR1Q17.Checked ? "1" : "~";
                    ans_m17 += Q3BR2Q17.Checked ? "1" : "~";
                    ans_m17 += Q3BR3Q17.Checked ? "1" : "~";
                    ans_m17 += Q3BR4Q17.Checked ? "1" : "~";
                }
                else
                {
                    ans_m17 = "~~~~";
                }


                //8
                if (Q3BR1Q18.Checked || Q3BR2Q18.Checked || Q3BR3Q18.Checked || Q3BR4Q18.Checked)
                {
                    ans_m18 += Q3BR1Q18.Checked ? "1" : "~";
                    ans_m18 += Q3BR2Q18.Checked ? "1" : "~";
                    ans_m18 += Q3BR3Q18.Checked ? "1" : "~";
                    ans_m18 += Q3BR4Q18.Checked ? "1" : "~";
                }
                else
                {
                    ans_m18 = "~~~~";
                }

                //9
                if (Q3BR1Q19.Checked || Q3BR2Q19.Checked || Q3BR3Q19.Checked || Q3BR4Q19.Checked)
                {
                    ans_m19 += Q3BR1Q19.Checked ? "1" : "~";
                    ans_m19 += Q3BR2Q19.Checked ? "1" : "~";
                    ans_m19 += Q3BR3Q19.Checked ? "1" : "~";
                    ans_m19 += Q3BR4Q19.Checked ? "1" : "~";
                }

                else
                {
                    ans_m19 = "~~~~";
                }


                //10
                if (Q3BR1Q20.Checked || Q3BR2Q20.Checked || Q3BR3Q20.Checked || Q3BR4Q20.Checked)
                {
                    ans_m20 += Q3BR1Q20.Checked ? "1" : "~";
                    ans_m20 += Q3BR2Q20.Checked ? "1" : "~";
                    ans_m20 += Q3BR3Q20.Checked ? "1" : "~";
                    ans_m20 += Q3BR4Q20.Checked ? "1" : "~";
                }
                else
                {
                    ans_m20 = "~~~~";
                }
                Common.WriteAnsFile(lbl_seat.Text, "¶Q3B~" + DateTime.Now.ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!1!" + ans_m11);
                Common.WriteAnsFile(lbl_seat.Text, "!2!" + ans_m12);
                Common.WriteAnsFile(lbl_seat.Text, "!3!" + ans_m13);
                Common.WriteAnsFile(lbl_seat.Text, "!4!" + ans_m14);
                Common.WriteAnsFile(lbl_seat.Text, "!5!" + ans_m15);
                Common.WriteAnsFile(lbl_seat.Text, "!6!" + ans_m16);
                Common.WriteAnsFile(lbl_seat.Text, "!7!" + ans_m17);
                Common.WriteAnsFile(lbl_seat.Text, "!8!" + ans_m18);
                Common.WriteAnsFile(lbl_seat.Text, "!9!" + ans_m19);
                Common.WriteAnsFile(lbl_seat.Text, "!10!" + ans_m10);
                string ans_mc1 = "", ans_mc2 = "", ans_mc3 = "", ans_mc4 = "", ans_mc5 = "";

                //1
                if (cbox81.Checked || cbox82.Checked || cbox83.Checked || cbox84.Checked || cbox85.Checked || cbox86.Checked)
                {
                    ans_mc1 += cbox81.Checked ? "1" : "~";
                    ans_mc1 += cbox82.Checked ? "1" : "~";
                    ans_mc1 += cbox83.Checked ? "1" : "~";
                    ans_mc1 += cbox84.Checked ? "1" : "~";
                    ans_mc1 += cbox85.Checked ? "1" : "~";
                    ans_mc1 += cbox86.Checked ? "1" : "~";
                }
                else
                {
                    ans_mc1 = "~~~~~~";

                }

                //2
                if (cbox87.Checked || cbox88.Checked || cbox89.Checked || cbox90.Checked || cbox91.Checked || cbox92.Checked)
                {

                    ans_mc2 += cbox87.Checked ? "1" : "~";
                    ans_mc2 += cbox88.Checked ? "1" : "~";
                    ans_mc2 += cbox89.Checked ? "1" : "~";
                    ans_mc2 += cbox90.Checked ? "1" : "~";
                    ans_mc2 += cbox91.Checked ? "1" : "~";
                    ans_mc2 += cbox92.Checked ? "1" : "~";

                }
                else
                {
                    ans_mc2 = "~~~~~~";

                }
                //3
                if (cbox93.Checked || cbox94.Checked || cbox95.Checked || cbox96.Checked || cbox97.Checked || cbox98.Checked)
                {

                    ans_mc3 += cbox93.Checked ? "1" : "~";
                    ans_mc3 += cbox94.Checked ? "1" : "~";
                    ans_mc3 += cbox95.Checked ? "1" : "~";
                    ans_mc3 += cbox96.Checked ? "1" : "~";
                    ans_mc3 += cbox97.Checked ? "1" : "~";
                    ans_mc3 += cbox98.Checked ? "1" : "~";
                }
                else
                {
                    ans_mc3 = "~~~~~~";

                }
                //4
                if (cbox99.Checked || cbox100.Checked || cbox101.Checked || cbox102.Checked || cbox103.Checked || cbox104.Checked)
                {

                    ans_mc4 += cbox99.Checked ? "1" : "~";
                    ans_mc4 += cbox100.Checked ? "1" : "~";
                    ans_mc4 += cbox101.Checked ? "1" : "~";
                    ans_mc4 += cbox102.Checked ? "1" : "~";
                    ans_mc4 += cbox103.Checked ? "1" : "~";
                    ans_mc4 += cbox104.Checked ? "1" : "~";
                }
                else
                {
                    ans_mc4 = "~~~~~~";

                }
                //5
                if (cbox105.Checked || cbox106.Checked || cbox107.Checked || cbox108.Checked || cbox109.Checked || cbox110.Checked)
                {

                    ans_mc5 += cbox105.Checked ? "1" : "~";
                    ans_mc5 += cbox106.Checked ? "1" : "~";
                    ans_mc5 += cbox107.Checked ? "1" : "~";
                    ans_mc5 += cbox108.Checked ? "1" : "~";
                    ans_mc5 += cbox109.Checked ? "1" : "~";
                    ans_mc5 += cbox110.Checked ? "1" : "~";

                }
                else
                {
                    ans_mc5 = "~~~~~~";

                }
                Common.WriteAnsFile(lbl_seat.Text, "¶Q4~" + DateTime.Now.ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!1!" + ans_mc1);
                Common.WriteAnsFile(lbl_seat.Text, "!2!" + ans_mc2);
                Common.WriteAnsFile(lbl_seat.Text, "!3!" + ans_mc3);
                Common.WriteAnsFile(lbl_seat.Text, "!4!" + ans_mc4);
                Common.WriteAnsFile(lbl_seat.Text, "!5!" + ans_mc5);
                string ans_mc11 = "", ans_mc12 = "";
                if (cbox_111.Checked || cbox_112.Checked || cbox_113.Checked || cbox_114.Checked || cbox_115.Checked || cbox_116.Checked)
                {
                    ans_mc11 += cbox_111.Checked ? "1" : "~";
                    ans_mc11 += cbox_112.Checked ? "1" : "~";
                    ans_mc11 += cbox_113.Checked ? "1" : "~";
                    ans_mc11 += cbox_114.Checked ? "1" : "~";
                    ans_mc11 += cbox_115.Checked ? "1" : "~";
                    ans_mc11 += cbox_116.Checked ? "1" : "~";
                }
                else
                {
                    ans_mc11 = "~~~~~~";

                }

                //2
                if (cbox_117.Checked || cbox_118.Checked || cbox_119.Checked || cbox_120.Checked || cbox_121.Checked || cbox_122.Checked)
                {
                    ans_mc12 += cbox_117.Checked ? "1" : "~";
                    ans_mc12 += cbox_118.Checked ? "1" : "~";
                    ans_mc12 += cbox_119.Checked ? "1" : "~";
                    ans_mc12 += cbox_120.Checked ? "1" : "~";
                    ans_mc12 += cbox_121.Checked ? "1" : "~";
                    ans_mc12 += cbox_122.Checked ? "1" : "~";

                }
                else
                {
                    ans_mc12 = "~~~~~~";

                }

                Common.WriteAnsFile(lbl_seat.Text, "¶Q5~" + DateTime.Now.ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!1!" + ans_mc11);
                Common.WriteAnsFile(lbl_seat.Text, "!2!" + ans_mc12);
                string ans_reg1 = txt_reg1.Text.ToUpper();
                string ans_reg2 = txt_reg2.Text.ToUpper();

                Common.WriteAnsFile(lbl_seat.Text, "¶Q6~" + DateTime.Now.ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!1!" + ans_reg1);
                Common.WriteAnsFile(lbl_seat.Text, "!2!" + ans_reg2);
                string ans_7a = txt_7a.Text.Replace("\r\n", "Ω");
                string ans_7b = txt_7b.Text.Replace("\r\n", "Ω");
                string ans_7c = txt_7c.Text.Replace("\r\n", "Ω");
                Common.WriteAnsFile(lbl_seat.Text, "¶Q7~" + DateTime.Now.ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!1!" + ans_7a);
                Common.WriteAnsFile(lbl_seat.Text, "!2!" + ans_7b);
                Common.WriteAnsFile(lbl_seat.Text, "!3!" + ans_7c);
                string ans_8a = txt_8a.Text.ToString().Replace("\r\n", "Ω");
                string ans_8b = txt_8b.Text.ToString().Replace("\r\n", "Ω");
                Common.WriteAnsFile(lbl_seat.Text, "¶Q8~" + DateTime.Now.ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!1!" + ans_8a);
                Common.WriteAnsFile(lbl_seat.Text, "!2!" + ans_8b);
                webervice_it.Final_Ans(lbl_Index.Text, lbl_seat.Text, Common.Get_IP(), lbl_paperid.Text, txtans1.Text, txtans2.Text, txtans3.Text, txtans4.Text, txtans5.Text, txtans6.Text, txtans7.Text, txtans8.Text, txtans9.Text, txtans10.Text,
                                                                                     QDT2.Rows[0]["Answer"].ToString(), QDT2.Rows[1]["Answer"].ToString(), QDT2.Rows[2]["Answer"].ToString(), QDT2.Rows[3]["Answer"].ToString(), QDT2.Rows[4]["Answer"].ToString(), QDT2.Rows[5]["Answer"].ToString(), QDT2.Rows[6]["Answer"].ToString(), QDT2.Rows[7]["Answer"].ToString(), QDT2.Rows[8]["Answer"].ToString(), QDT2.Rows[9]["Answer"].ToString(),
                                                                                    ans_m1, ans_m2, ans_m3, ans_m4, ans_m5, ans_m6, ans_m7, ans_m8, ans_m9, ans_m10,
                                                                                    ans_m11, ans_m12, ans_m13, ans_m14, ans_m15, ans_m16, ans_m17, ans_m18, ans_m19, ans_m20,
                                                                                    ans_mc1, ans_mc2, ans_mc3, ans_mc4, ans_mc5,
                                                                                    ans_mc11, ans_mc12,
                                                                                    ans_reg1, ans_reg2,
                                                                                    ans_7a, ans_7b, ans_7c,
                                                                                    ans_8a, ans_8b
                                                                                    );
            }
            catch (Exception ex)
            {

            }
        }
        // TAB Change 
        #region
        private void timer1_Tick(object sender, EventArgs e)
        {
            lbl_clk.Text = DateTime.Now.ToShortTimeString().Remove(DateTime.Now.ToShortTimeString().Length - 3) + " " + DateTime.Now.Hour.ToString();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {

            TimeLeft = TimeLeft.Subtract(TimeSpan.Parse("00:00:01"));
            if (Timeflag == 0)
            {
                Check_Login_Time();

            }
            if (TimeLeft.Minutes < 6 && TimeLeft.Hours <= 0)
            {
                if (TimeLeft.Minutes < 6)
                {
                    if (min.ForeColor == Color.Black)
                        min.ForeColor = Color.Red;
                    else
                        min.ForeColor = Color.Black;
                }
            }
            if (TimeLeft.Hours <= 0 && TimeLeft.Minutes <= 0 && TimeLeft.Seconds <= 0)
            {
                timer2.Stop();
                MessageBox.Show("Time UP");
                MessageBox.Show("Please Wait ..System might be take more time to save records");
                WriteFinalAns();
                GiveUp();
            }
            min.Text = TimeLeft.ToString();

        }

        private void btnQ1_Click(object sender, EventArgs e)
        {
            Q1BTN_Click();
        }

        private void Q1BTN_Click()
        {
            tabControl1.SelectedIndex = 0;
            lbl_qnamebtn.Text = "Question No 1. FILL IN THE BLANKS";
            btnQ1.BackColor = Color.Orange;
            btnQ2.BackColor = Color.WhiteSmoke;
            btnQ6.BackColor = Color.WhiteSmoke;
            btnQ3_A.BackColor = Color.WhiteSmoke;
            btnQ3_B.BackColor = Color.WhiteSmoke;
            btnQ4.BackColor = Color.WhiteSmoke;
            btnQ5.BackColor = Color.WhiteSmoke;
            btnQ7.BackColor = Color.WhiteSmoke;
            btnQ8.BackColor = Color.WhiteSmoke;
        }

        private void btnQ2_Click(object sender, EventArgs e)
        {
            Q2BTN_Click();
        }

        private void Q2BTN_Click()
        {
            tabControl1.SelectedIndex = 1;
            btnQ1.BackColor = Color.WhiteSmoke;
            lbl_qnamebtn.Text = "Question No 2. TRUE/FALSE";
            btnQ2.BackColor = Color.Orange;
            btnQ6.BackColor = Color.WhiteSmoke;
            btnQ3_A.BackColor = Color.WhiteSmoke;
            btnQ3_B.BackColor = Color.WhiteSmoke;
            btnQ4.BackColor = Color.WhiteSmoke;
            btnQ5.BackColor = Color.WhiteSmoke;
            btnQ7.BackColor = Color.WhiteSmoke;
            btnQ8.BackColor = Color.WhiteSmoke;
        }

        private void btnQ3_A_Click(object sender, EventArgs e)
        {
            Q3ABTN_Click();
        }

        private void Q3ABTN_Click()
        {
            tabControl1.SelectedIndex = 2;


            lbl_qnamebtn.Visible = true;
            lbl_qnamebtn.Text = "Question No 3 A. (MCQ1)SINGLE CORRECT ANS";
            btnQ1.BackColor = Color.WhiteSmoke;
            btnQ2.BackColor = Color.WhiteSmoke;
            btnQ6.BackColor = Color.WhiteSmoke;
            btnQ3_A.BackColor = Color.Orange;
            btnQ3_B.BackColor = Color.WhiteSmoke;
            btnQ4.BackColor = Color.WhiteSmoke;
            btnQ5.BackColor = Color.WhiteSmoke;
            btnQ7.BackColor = Color.WhiteSmoke;
            btnQ8.BackColor = Color.WhiteSmoke;
        }

        private void btnQ3_B_Click(object sender, EventArgs e)
        {
            Q3BBTN_Click();
        }

        private void Q3BBTN_Click()
        {
            tabControl1.SelectedIndex = 3;

            lbl_qnamebtn.Visible = true;
            lbl_qnamebtn.Text = "Question No 3 B. (MCQ2) SINGLE CORRECT ANS";
            btnQ2.BackColor = Color.WhiteSmoke;
            btnQ6.BackColor = Color.WhiteSmoke;
            btnQ1.BackColor = Color.WhiteSmoke;
            btnQ3_A.BackColor = Color.WhiteSmoke;
            btnQ3_B.BackColor = Color.Orange;
            btnQ4.BackColor = Color.WhiteSmoke;
            btnQ5.BackColor = Color.WhiteSmoke;
            btnQ7.BackColor = Color.WhiteSmoke;
            btnQ8.BackColor = Color.WhiteSmoke;
        }

        private void btnQ4_Click(object sender, EventArgs e)
        {
            Q4BTN_Click();
        }

        private void Q4BTN_Click()
        {
            tabControl1.SelectedIndex = 4;



            lbl_qnamebtn.Visible = true;
            lbl_qnamebtn.Text = "Question No 4. (MCQ3)TWO CORRECT ANS";
            btnQ2.BackColor = Color.WhiteSmoke;
            btnQ6.BackColor = Color.WhiteSmoke;
            btnQ3_A.BackColor = Color.WhiteSmoke;
            btnQ3_B.BackColor = Color.WhiteSmoke;
            btnQ1.BackColor = Color.WhiteSmoke;
            btnQ4.BackColor = Color.Orange;
            btnQ5.BackColor = Color.WhiteSmoke;
            btnQ7.BackColor = Color.WhiteSmoke;
            btnQ8.BackColor = Color.WhiteSmoke;
        }

        private void btnQ5_Click(object sender, EventArgs e)
        {
            Q5BTN_Click();
        }

        private void Q5BTN_Click()
        {
            tabControl1.SelectedIndex = 5;


            lbl_qnamebtn.Visible = true;
            lbl_qnamebtn.Text = "Question No 5. (MCQ4)THREE CORRECT ANS";
            btnQ2.BackColor = Color.WhiteSmoke;
            btnQ6.BackColor = Color.WhiteSmoke;
            btnQ3_A.BackColor = Color.WhiteSmoke;
            btnQ3_B.BackColor = Color.WhiteSmoke;
            btnQ4.BackColor = Color.WhiteSmoke;
            btnQ1.BackColor = Color.WhiteSmoke;
            btnQ5.BackColor = Color.Orange;
            btnQ7.BackColor = Color.WhiteSmoke;
            btnQ8.BackColor = Color.WhiteSmoke;
        }

        private void btnQ6_Click(object sender, EventArgs e)
        {
            Q6BTN_Click();
        }

        private void Q6BTN_Click()
        {
            tabControl1.SelectedIndex = 6;

            lbl_qnamebtn.Visible = true;
            lbl_qnamebtn.Text = "Question No 6. REARRANGE";
            btnQ2.BackColor = Color.WhiteSmoke;

            btnQ3_A.BackColor = Color.WhiteSmoke;
            btnQ3_B.BackColor = Color.WhiteSmoke;
            btnQ4.BackColor = Color.WhiteSmoke;
            btnQ5.BackColor = Color.WhiteSmoke;
            btnQ6.BackColor = Color.Orange;
            btnQ1.BackColor = Color.WhiteSmoke;
            btnQ7.BackColor = Color.WhiteSmoke;
            btnQ8.BackColor = Color.WhiteSmoke;
        }

        private void btnQ7_Click(object sender, EventArgs e)
        {
            Q7BTN_Click();
        }

        private void Q7BTN_Click()
        {
            tabControl1.SelectedIndex = 7;

            lbl_qnamebtn.Visible = true;
            lbl_qnamebtn.Text = "Question No 7.SHORT ANSWER";
            btnQ2.BackColor = Color.WhiteSmoke;
            btnQ1.BackColor = Color.WhiteSmoke;
            btnQ3_A.BackColor = Color.WhiteSmoke;
            btnQ3_B.BackColor = Color.WhiteSmoke;
            btnQ4.BackColor = Color.WhiteSmoke;
            btnQ5.BackColor = Color.WhiteSmoke;
            btnQ6.BackColor = Color.WhiteSmoke;
            btnQ7.BackColor = Color.Orange;
            btnQ8.BackColor = Color.WhiteSmoke;
        }

        private void btnQ8_Click(object sender, EventArgs e)
        {
            Q8BTN_Click();
        }

        private void Q8BTN_Click()
        {
            tabControl1.SelectedIndex = 8;
            lbl_qnamebtn.Visible = true;
            lbl_qnamebtn.Text = "Question No 8. Write A Program";
            btnQ2.BackColor = Color.WhiteSmoke;
            btnQ1.BackColor = Color.WhiteSmoke;
            btnQ3_A.BackColor = Color.WhiteSmoke;
            btnQ3_B.BackColor = Color.WhiteSmoke;
            btnQ4.BackColor = Color.WhiteSmoke;
            btnQ5.BackColor = Color.WhiteSmoke;
            btnQ6.BackColor = Color.WhiteSmoke;
            btnQ7.BackColor = Color.WhiteSmoke;
            btnQ8.BackColor = Color.Orange;
        }



        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                Q1BTN_Click();
            }
            if (tabControl1.SelectedIndex == 1)
            {
                Q2BTN_Click();
            }
            if (tabControl1.SelectedIndex == 2)
            {
                Q3ABTN_Click();
            }
            if (tabControl1.SelectedIndex == 3)
            {
                Q3BBTN_Click();
            }
            if (tabControl1.SelectedIndex == 4)
            {
                Q4BTN_Click();
            }
            if (tabControl1.SelectedIndex == 5)
            {
                Q5BTN_Click();
            }
            if (tabControl1.SelectedIndex == 6)
            {
                Q6BTN_Click();
            }
            if (tabControl1.SelectedIndex == 7)
            {
                Q7BTN_Click();
            }
            if (tabControl1.SelectedIndex == 8)
            {
                Q8BTN_Click();
            }
        }

        #endregion

        //MCQ Selection
        #region 
        private void ManageCheckGroupBox(CheckBox chk, GroupBox grp)
        {
            // Make sure the CheckBox isn't in the GroupBox.
            // This will only happen the first time.
            if (chk.Parent == grp)
            {
                int tempCheck = 0;
                foreach (CheckBox c in grp.Controls)
                {
                    if (c.Checked == true)
                    {
                        tempCheck = tempCheck + 1;
                    }
                    if (tempCheck > 2)
                    {
                        chk.Checked = false;
                    }
                }
            }


        }
        private void ManageCheckGroupBox_Three(CheckBox chk, GroupBox grp)
        {
            // Make sure the CheckBox isn't in the GroupBox.
            // This will only happen the first time.
            if (chk.Parent == grp)
            {
                int tempCheck = 0;
                foreach (CheckBox c in grp.Controls)
                {
                    if (c.Checked == true)
                    {
                        tempCheck = tempCheck + 1;
                    }
                    if (tempCheck > 3)
                    {
                        chk.Checked = false;
                    }
                }
            }
        }

        private void cbox81_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox81, groupBox106);
        }

        private void cbox82_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox82, groupBox106);
        }

        private void cbox83_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox83, groupBox106);
        }

        private void cbox84_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox84, groupBox106);
        }

        private void cbox85_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox85, groupBox106);
        }

        private void cbox86_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox86, groupBox106);
        }

        private void cbox87_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox87, groupBox105);
        }

        private void cbox88_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox88, groupBox105);
        }

        private void cbox89_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox89, groupBox105);
        }

        private void cbox90_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox90, groupBox105);
        }

        private void cbox91_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox91, groupBox105);

        }

        private void cbox92_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox92, groupBox105);
        }

        private void cbox93_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox93, groupBox104);
        }

        private void cbox94_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox94, groupBox104);
        }

        private void cbox95_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox95, groupBox104);
        }

        private void cbox96_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox96, groupBox104);
        }

        private void cbox97_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox97, groupBox104);
        }

        private void cbox98_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox98, groupBox104);
        }

        private void cbox99_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox99, groupBox103);
        }

        private void cbox100_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox100, groupBox103);
        }

        private void cbox101_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox101, groupBox103);
        }

        private void cbox102_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox102, groupBox103);
        }

        private void cbox103_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox103, groupBox103);
        }

        private void cbox104_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox104, groupBox103);
        }

        private void cbox105_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox105, groupBox137);
        }

        private void cbox106_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox106, groupBox137);
        }

        private void cbox107_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox107, groupBox137);
        }

        private void cbox108_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox108, groupBox137);
        }

        private void cbox109_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox109, groupBox137);
        }

        private void cbox110_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox(cbox110, groupBox137);
        }

        private void cbox_111_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox_Three(cbox_111, groupBox108);

        }

        private void cbox_112_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox_Three(cbox_112, groupBox108);
        }

        private void cbox_113_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox_Three(cbox_113, groupBox108);
        }

        private void cbox_114_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox_Three(cbox_114, groupBox108);
        }

        private void cbox_115_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox_Three(cbox_115, groupBox108);
        }

        private void cbox_116_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox_Three(cbox_116, groupBox108);
        }

        private void cbox_117_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox_Three(cbox_117, groupBox107);
        }

        private void cbox_118_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox_Three(cbox_118, groupBox107);
        }

        private void cbox_119_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox_Three(cbox_119, groupBox107);
        }

        private void cbox_120_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox_Three(cbox_120, groupBox107);
        }

        private void cbox_121_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox_Three(cbox_121, groupBox107);
        }

        private void cbox_122_CheckedChanged(object sender, EventArgs e)
        {
            ManageCheckGroupBox_Three(cbox_122, groupBox107);
        }
        #endregion
        private void Internet_Check_Timer_Tick(object sender, EventArgs e)
        {
            Internet_Check_Timer.Stop();
            if (Common.IsConnectedToInternet())
            {
                if (Internet_Check == 0)
                {
                    lbl_Connection_Status.Text = "1";
                    lbl_Connection_Status.ForeColor = Color.Green;
                    Internet_Check = 1;
                }
            }
            else
            {
                lbl_Connection_Status.Text = "0";
                lbl_Connection_Status.ForeColor = Color.Red;
                Internet_Check = 0;
            }
            Internet_Check_Timer.Start();


        }
        //Question Submit
        #region

        private void btn_Q1_submit_Click(object sender, EventArgs e)
        {
            Thred1 = new Thread(Question1_ThreadCall);
            Thred1.Start();

        }

        private void Question1_ThreadCall()
        {


            try
            {

                QDT1.Rows[0]["Answer"] = txtans1.Text;
                QDT1.Rows[1]["Answer"] = txtans2.Text;
                QDT1.Rows[2]["Answer"] = txtans3.Text;
                QDT1.Rows[3]["Answer"] = txtans4.Text;
                QDT1.Rows[4]["Answer"] = txtans5.Text;
                QDT1.Rows[5]["Answer"] = txtans6.Text;
                QDT1.Rows[6]["Answer"] = txtans7.Text;
                QDT1.Rows[7]["Answer"] = txtans8.Text;
                QDT1.Rows[8]["Answer"] = txtans9.Text;
                QDT1.Rows[9]["Answer"] = txtans10.Text;

                this.Invoke((MethodInvoker)delegate ()
                {
                    Loader_box.Visible = true;
                });
                Common.WriteAnsFile(lbl_seat.Text, "¶Q1~" + DateTime.Now.ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!1!" + txtans1.Text);
                Common.WriteAnsFile(lbl_seat.Text, "!2!" + txtans2.Text);
                Common.WriteAnsFile(lbl_seat.Text, "!3!" + txtans3.Text);
                Common.WriteAnsFile(lbl_seat.Text, "!4!" + txtans4.Text);
                Common.WriteAnsFile(lbl_seat.Text, "!5!" + txtans5.Text);
                Common.WriteAnsFile(lbl_seat.Text, "!6!" + txtans6.Text);
                Common.WriteAnsFile(lbl_seat.Text, "!7!" + txtans7.Text);
                Common.WriteAnsFile(lbl_seat.Text, "!8!" + txtans8.Text);
                Common.WriteAnsFile(lbl_seat.Text, "!9!" + txtans9.Text);
                Common.WriteAnsFile(lbl_seat.Text, "!10!" + txtans10.Text);

                lbl_filesize.Text = Common.AnsFileSize(lbl_seat.Text) + " Bytes";

                btnQ1.Image = ((System.Drawing.Image)(Properties.Resources.Single_Tick));
            }
            catch (Exception ex)
            {
                Send_Error(ex, "1", "TEXT");
            }
            for (int i = 0; i < QDT1.Rows.Count; i++)
            {
                if (QDT1.Rows[i]["Answer"].ToString() != "")
                {
                    Button b = (Button)this.Controls.Find("qq" + (i + 1).ToString(), true).FirstOrDefault();
                    b.BackColor = Color.Green;
                }
            }
            try
            {
                webervice_it.Question1(lbl_Index.Text, lbl_seat.Text, Common.Get_IP(), lbl_paperid.Text, txtans1.Text, txtans2.Text, txtans3.Text, txtans4.Text, txtans5.Text, txtans6.Text, txtans7.Text, txtans8.Text, txtans9.Text, txtans10.Text);
                btnQ1.Image = ((System.Drawing.Image)(Properties.Resources.double_tick));

            }
            catch (Exception exe)
            {
                Send_Error(exe, "1", "DB");
            }


            this.Invoke((MethodInvoker)delegate ()
            {
                Loader_box.Visible = false;
            });
            MessageBox.Show("Answer Submitted");


        }


        private void btn_Q2_submit_Click(object sender, EventArgs e)
        {

            Thred2 = new Thread(Question2_ThreadCall);
            Thred2.Start();
        }

        private void Question2_ThreadCall()
        {
            try
            {
                if (rb_tf1.Checked == true)
                {
                    QDT2.Rows[0]["Answer"] = "True";
                }
                else if (rb_tf2.Checked == true)
                {
                    QDT2.Rows[0]["Answer"] = "False";
                }
                else
                {
                    QDT2.Rows[0]["Answer"] = "";
                }


                if (rb_tf3.Checked == true)
                {
                    QDT2.Rows[1]["Answer"] = "True";
                }
                else if (rb_tf4.Checked == true)
                {
                    QDT2.Rows[1]["Answer"] = "False";
                }
                else
                {
                    QDT2.Rows[1]["Answer"] = "";
                }


                if (rb_tf5.Checked == true)
                {
                    QDT2.Rows[2]["Answer"] = "True";
                }
                else if (rb_tf6.Checked == true)
                {
                    QDT2.Rows[2]["Answer"] = "False";
                }
                else
                {
                    QDT2.Rows[2]["Answer"] = "";
                }

                if (rb_tf7.Checked == true)
                {
                    QDT2.Rows[3]["Answer"] = "True";
                }
                else if (rb_tf8.Checked == true)
                {
                    QDT2.Rows[3]["Answer"] = "False";
                }
                else
                {
                    QDT2.Rows[3]["Answer"] = "";
                }

                if (rb_tf9.Checked == true)
                {
                    QDT2.Rows[4]["Answer"] = "True";
                }
                else if (rb_tf10.Checked == true)
                {
                    QDT2.Rows[4]["Answer"] = "False";
                }
                else
                {
                    QDT2.Rows[4]["Answer"] = "";
                }

                if (rb_tf11.Checked == true)
                {
                    QDT2.Rows[5]["Answer"] = "True";
                }
                else if (rb_tf12.Checked == true)
                {
                    QDT2.Rows[5]["Answer"] = "False";
                }
                else
                {
                    QDT2.Rows[5]["Answer"] = "";
                }


                if (rb_tf13.Checked == true)
                {
                    QDT2.Rows[6]["Answer"] = "True";
                }
                else if (rb_tf14.Checked == true)
                {
                    QDT2.Rows[6]["Answer"] = "False";
                }
                else
                {
                    QDT2.Rows[6]["Answer"] = "";
                }


                if (rb_tf15.Checked == true)
                {
                    QDT2.Rows[7]["Answer"] = "True";
                }
                else if (rb_tf16.Checked == true)
                {
                    QDT2.Rows[7]["Answer"] = "False";
                }
                else
                {
                    QDT2.Rows[7]["Answer"] = "";
                }

                if (rb_tf17.Checked == true)
                {
                    QDT2.Rows[8]["Answer"] = "True";
                }
                else if (rb_tf18.Checked == true)
                {
                    QDT2.Rows[8]["Answer"] = "False";
                }
                else
                {
                    QDT2.Rows[8]["Answer"] = "";
                }

                if (rb_tf19.Checked == true)
                {
                    QDT2.Rows[9]["Answer"] = "True";
                }
                else if (rb_tf20.Checked == true)
                {
                    QDT2.Rows[9]["Answer"] = "False";
                }
                else
                {
                    QDT2.Rows[9]["Answer"] = "";
                }

                this.Invoke((MethodInvoker)delegate ()
                {
                    Loader_box.Visible = true;
                });

                Common.WriteAnsFile(lbl_seat.Text, "¶Q2~" + DateTime.Now.ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!1!" + QDT2.Rows[0]["Answer"]);
                Common.WriteAnsFile(lbl_seat.Text, "!2!" + QDT2.Rows[1]["Answer"]);
                Common.WriteAnsFile(lbl_seat.Text, "!3!" + QDT2.Rows[2]["Answer"]);
                Common.WriteAnsFile(lbl_seat.Text, "!4!" + QDT2.Rows[3]["Answer"]);
                Common.WriteAnsFile(lbl_seat.Text, "!5!" + QDT2.Rows[4]["Answer"]);
                Common.WriteAnsFile(lbl_seat.Text, "!6!" + QDT2.Rows[5]["Answer"]);
                Common.WriteAnsFile(lbl_seat.Text, "!7!" + QDT2.Rows[6]["Answer"]);
                Common.WriteAnsFile(lbl_seat.Text, "!8!" + QDT2.Rows[7]["Answer"]);
                Common.WriteAnsFile(lbl_seat.Text, "!9!" + QDT2.Rows[8]["Answer"]);
                Common.WriteAnsFile(lbl_seat.Text, "!10!" + QDT2.Rows[9]["Answer"]);
                this.Invoke((MethodInvoker)delegate ()
                {
                    lbl_filesize.Text = Common.AnsFileSize(lbl_seat.Text) + " Bytes";
                });
                btnQ2.Image = ((System.Drawing.Image)(Properties.Resources.Single_Tick));
            }
            catch (Exception ex)
            {
                Send_Error(ex, "2", "TEXT");
            }
            for (int i = 0; i < QDT2.Rows.Count; i++)
            {
                if (i == 9)
                {
                    if (QDT2.Rows[i]["Answer"].ToString() != "")
                    {
                        Button b = (Button)this.Controls.Find("qq20".ToString(), true).FirstOrDefault();
                        b.BackColor = Color.Green;
                    }
                    continue;
                }
                if (QDT2.Rows[i]["Answer"].ToString() != "")
                {
                    Button b = (Button)this.Controls.Find("qq1" + (i + 1).ToString(), true).FirstOrDefault();
                    b.BackColor = Color.Green;
                }
            }

            try
            {
                webervice_it.Question2(lbl_Index.Text, lbl_seat.Text, Common.Get_IP(), lbl_paperid.Text, QDT2.Rows[0]["Answer"].ToString(), QDT2.Rows[1]["Answer"].ToString(), QDT2.Rows[2]["Answer"].ToString(), QDT2.Rows[3]["Answer"].ToString(), QDT2.Rows[4]["Answer"].ToString(), QDT2.Rows[5]["Answer"].ToString(), QDT2.Rows[6]["Answer"].ToString(), QDT2.Rows[7]["Answer"].ToString(), QDT2.Rows[8]["Answer"].ToString(), QDT2.Rows[9]["Answer"].ToString());
                btnQ2.Image = ((System.Drawing.Image)(Properties.Resources.double_tick));
            }
            catch (Exception exe)
            {
                Send_Error(exe, "2", "DB");
            }
            this.Invoke((MethodInvoker)delegate ()
            {
                Loader_box.Visible = false;
            });
            btnQ2.Image = ((System.Drawing.Image)(Properties.Resources.double_tick));
            MessageBox.Show("Answer Submitted");
        }

        private void btn_Q3A_submit_Click(object sender, EventArgs e)
        {
            Thred3A = new Thread(Question3A_ThreadCall);
            Thred3A.Start();
        }

        private void Question3A_ThreadCall()
        {

            string ans_m1 = "", ans_m2 = "", ans_m3 = "", ans_m4 = "", ans_m5 = "", ans_m6 = "", ans_m7 = "", ans_m8 = "", ans_m9 = "", ans_m10 = "";
            try
            {
                //1
                if (Q3AR1Q1.Checked || Q3AR2Q1.Checked || Q3AR3Q1.Checked || Q3AR4Q1.Checked)
                {
                    ans_m1 += Q3AR1Q1.Checked ? "1" : "~";
                    ans_m1 += Q3AR2Q1.Checked ? "1" : "~";
                    ans_m1 += Q3AR3Q1.Checked ? "1" : "~";
                    ans_m1 += Q3AR4Q1.Checked ? "1" : "~";
                }
                else
                {
                    ans_m1 = "~~~~";
                }
                //1
                if (Q3AR1Q2.Checked || Q3AR2Q2.Checked || Q3AR3Q2.Checked || Q3AR4Q2.Checked)
                {
                    ans_m2 += Q3AR1Q2.Checked ? "1" : "~";
                    ans_m2 += Q3AR2Q2.Checked ? "1" : "~";
                    ans_m2 += Q3AR3Q2.Checked ? "1" : "~";
                    ans_m2 += Q3AR4Q2.Checked ? "1" : "~";
                }
                else
                {
                    ans_m2 = "~~~~";
                }

                //3
                if (Q3AR1Q3.Checked || Q3AR2Q3.Checked || Q3AR3Q3.Checked || Q3AR4Q3.Checked)
                {
                    ans_m3 += Q3AR1Q3.Checked ? "1" : "~";
                    ans_m3 += Q3AR2Q3.Checked ? "1" : "~";
                    ans_m3 += Q3AR3Q3.Checked ? "1" : "~";
                    ans_m3 += Q3AR4Q3.Checked ? "1" : "~";
                }
                else
                {
                    ans_m3 = "~~~~";
                }

                //4
                if (Q3AR1Q4.Checked || Q3AR2Q4.Checked || Q3AR3Q4.Checked || Q3AR4Q4.Checked)
                {
                    ans_m4 += Q3AR1Q4.Checked ? "1" : "~";
                    ans_m4 += Q3AR2Q4.Checked ? "1" : "~";
                    ans_m4 += Q3AR3Q4.Checked ? "1" : "~";
                    ans_m4 += Q3AR4Q4.Checked ? "1" : "~";
                }
                else
                {
                    ans_m4 = "~~~~";
                }

                //5
                if (Q3AR1Q5.Checked || Q3AR2Q5.Checked || Q3AR3Q5.Checked || Q3AR4Q5.Checked)
                {
                    ans_m5 += Q3AR1Q5.Checked ? "1" : "~";
                    ans_m5 += Q3AR2Q5.Checked ? "1" : "~";
                    ans_m5 += Q3AR3Q5.Checked ? "1" : "~";
                    ans_m5 += Q3AR4Q5.Checked ? "1" : "~";
                }
                else
                {
                    ans_m5 = "~~~~";
                }

                //6
                if (Q3AR1Q6.Checked || Q3AR2Q6.Checked || Q3AR3Q6.Checked || Q3AR4Q6.Checked)
                {
                    ans_m6 += Q3AR1Q6.Checked ? "1" : "~";
                    ans_m6 += Q3AR2Q6.Checked ? "1" : "~";
                    ans_m6 += Q3AR3Q6.Checked ? "1" : "~";
                    ans_m6 += Q3AR4Q6.Checked ? "1" : "~";
                }
                else
                {
                    ans_m6 = "~~~~";
                }

                //7
                if (Q3AR1Q7.Checked || Q3AR2Q7.Checked || Q3AR3Q7.Checked || Q3AR4Q7.Checked)
                {
                    ans_m7 += Q3AR1Q7.Checked ? "1" : "~";
                    ans_m7 += Q3AR2Q7.Checked ? "1" : "~";
                    ans_m7 += Q3AR3Q7.Checked ? "1" : "~";
                    ans_m7 += Q3AR4Q7.Checked ? "1" : "~";
                }
                else
                {
                    ans_m7 = "~~~~";
                }


                //8
                if (Q3AR1Q8.Checked || Q3AR2Q8.Checked || Q3AR3Q8.Checked || Q3AR4Q8.Checked)
                {
                    ans_m8 += Q3AR1Q8.Checked ? "1" : "~";
                    ans_m8 += Q3AR2Q8.Checked ? "1" : "~";
                    ans_m8 += Q3AR3Q8.Checked ? "1" : "~";
                    ans_m8 += Q3AR4Q8.Checked ? "1" : "~";
                }
                else
                {
                    ans_m8 = "~~~~";
                }

                //9
                if (Q3AR1Q9.Checked || Q3AR2Q9.Checked || Q3AR3Q9.Checked || Q3AR4Q9.Checked)
                {
                    ans_m9 += Q3AR1Q9.Checked ? "1" : "~";
                    ans_m9 += Q3AR2Q9.Checked ? "1" : "~";
                    ans_m9 += Q3AR3Q9.Checked ? "1" : "~";
                    ans_m9 += Q3AR4Q9.Checked ? "1" : "~";
                }

                else
                {
                    ans_m9 = "~~~~";
                }


                //10
                if (Q3AR1Q10.Checked || Q3AR2Q10.Checked || Q3AR3Q10.Checked || Q3AR4Q10.Checked)
                {
                    ans_m10 += Q3AR1Q10.Checked ? "1" : "~";
                    ans_m10 += Q3AR2Q10.Checked ? "1" : "~";
                    ans_m10 += Q3AR3Q10.Checked ? "1" : "~";
                    ans_m10 += Q3AR4Q10.Checked ? "1" : "~";
                }
                else
                {
                    ans_m10 = "~~~~";
                }



                if (ans_m1 != "~~~~" && ans_m1 != "")
                {
                    qq21.BackColor = Color.Green;
                }

                if (ans_m2 != "~~~~" && ans_m2 != "")
                {
                    qq22.BackColor = Color.Green;
                }

                if (ans_m3 != "~~~~" && ans_m3 != "")
                {
                    qq23.BackColor = Color.Green;
                }

                if (ans_m4 != "~~~~" && ans_m4 != "")
                {
                    qq24.BackColor = Color.Green;
                }

                if (ans_m5 != "~~~~" && ans_m5 != "")
                {
                    qq25.BackColor = Color.Green;
                }

                if (ans_m6 != "~~~~" && ans_m6 != "")
                {
                    qq26.BackColor = Color.Green;
                }

                if (ans_m7 != "~~~~" && ans_m7 != "")
                {
                    qq27.BackColor = Color.Green;
                }

                if (ans_m8 != "~~~~" && ans_m8 != "")
                {
                    qq28.BackColor = Color.Green;
                }
                if (ans_m9 != "~~~~" && ans_m9 != "")
                {
                    qq29.BackColor = Color.Green;
                }
                if (ans_m10 != "~~~~" && ans_m10 != "")
                {
                    qq30.BackColor = Color.Green;
                }
                this.Invoke((MethodInvoker)delegate ()
                {
                    Loader_box.Visible = true;
                });

                Common.WriteAnsFile(lbl_seat.Text, "¶Q3A~" + DateTime.Now.ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!1!" + ans_m1);
                Common.WriteAnsFile(lbl_seat.Text, "!2!" + ans_m2);
                Common.WriteAnsFile(lbl_seat.Text, "!3!" + ans_m3);
                Common.WriteAnsFile(lbl_seat.Text, "!4!" + ans_m4);
                Common.WriteAnsFile(lbl_seat.Text, "!5!" + ans_m5);
                Common.WriteAnsFile(lbl_seat.Text, "!6!" + ans_m6);
                Common.WriteAnsFile(lbl_seat.Text, "!7!" + ans_m7);
                Common.WriteAnsFile(lbl_seat.Text, "!8!" + ans_m8);
                Common.WriteAnsFile(lbl_seat.Text, "!9!" + ans_m9);
                Common.WriteAnsFile(lbl_seat.Text, "!10!" + ans_m10);
                this.Invoke((MethodInvoker)delegate ()
                {
                    lbl_filesize.Text = Common.AnsFileSize(lbl_seat.Text) + " Bytes";
                });
                btnQ3_A.Image = ((System.Drawing.Image)(Properties.Resources.Single_Tick));

            }
            catch (Exception ex)
            {
                Send_Error(ex, "3A", "TEXT");
            }

            try
            {
                webervice_it.Question3A(lbl_Index.Text, lbl_seat.Text, Common.Get_IP(), lbl_paperid.Text, ans_m1, ans_m2, ans_m3, ans_m4, ans_m5, ans_m6, ans_m7, ans_m8, ans_m9, ans_m10);
                btnQ3_A.Image = ((System.Drawing.Image)(Properties.Resources.double_tick));
            }
            catch (Exception exe)
            {
                Send_Error(exe, "3A", "DB");
            }
            this.Invoke((MethodInvoker)delegate ()
            {
                Loader_box.Visible = false;
            });

            MessageBox.Show("Answer Submitted");
        }

        private void btn_Q3B_submit_Click(object sender, EventArgs e)
        {
            Thred3B = new Thread(Question3B_ThreadCall);
            Thred3B.Start();
        }

        private void Question3B_ThreadCall()
        {
            string ans_m1 = "", ans_m2 = "", ans_m3 = "", ans_m4 = "", ans_m5 = "", ans_m6 = "", ans_m7 = "", ans_m8 = "", ans_m9 = "", ans_m10 = "";
            try
            {

                //1
                if (Q3BR1Q11.Checked || Q3BR2Q11.Checked || Q3BR3Q11.Checked || Q3BR4Q11.Checked)
                {
                    ans_m1 += Q3BR1Q11.Checked ? "1" : "~";
                    ans_m1 += Q3BR2Q11.Checked ? "1" : "~";
                    ans_m1 += Q3BR3Q11.Checked ? "1" : "~";
                    ans_m1 += Q3BR4Q11.Checked ? "1" : "~";
                }
                else
                {
                    ans_m1 = "~~~~";
                }
                //1
                if (Q3BR1Q12.Checked || Q3BR2Q12.Checked || Q3BR3Q12.Checked || Q3BR4Q12.Checked)
                {
                    ans_m2 += Q3BR1Q12.Checked ? "1" : "~";
                    ans_m2 += Q3BR2Q12.Checked ? "1" : "~";
                    ans_m2 += Q3BR3Q12.Checked ? "1" : "~";
                    ans_m2 += Q3BR4Q12.Checked ? "1" : "~";
                }
                else
                {
                    ans_m2 = "~~~~";
                }

                //3
                if (Q3BR1Q13.Checked || Q3BR2Q13.Checked || Q3BR3Q13.Checked || Q3BR4Q13.Checked)
                {
                    ans_m3 += Q3BR1Q13.Checked ? "1" : "~";
                    ans_m3 += Q3BR2Q13.Checked ? "1" : "~";
                    ans_m3 += Q3BR3Q13.Checked ? "1" : "~";
                    ans_m3 += Q3BR4Q13.Checked ? "1" : "~";
                }
                else
                {
                    ans_m3 = "~~~~";
                }

                //4
                if (Q3BR1Q14.Checked || Q3BR2Q14.Checked || Q3BR3Q14.Checked || Q3BR4Q14.Checked)
                {
                    ans_m4 += Q3BR1Q14.Checked ? "1" : "~";
                    ans_m4 += Q3BR2Q14.Checked ? "1" : "~";
                    ans_m4 += Q3BR3Q14.Checked ? "1" : "~";
                    ans_m4 += Q3BR4Q14.Checked ? "1" : "~";
                }
                else
                {
                    ans_m4 = "~~~~";
                }

                //5
                if (Q3BR1Q15.Checked || Q3BR2Q15.Checked || Q3BR3Q15.Checked || Q3BR4Q15.Checked)
                {
                    ans_m5 += Q3BR1Q15.Checked ? "1" : "~";
                    ans_m5 += Q3BR2Q15.Checked ? "1" : "~";
                    ans_m5 += Q3BR3Q15.Checked ? "1" : "~";
                    ans_m5 += Q3BR4Q15.Checked ? "1" : "~";
                }
                else
                {
                    ans_m5 = "~~~~";
                }

                //6
                if (Q3BR1Q16.Checked || Q3BR2Q16.Checked || Q3BR3Q16.Checked || Q3BR4Q16.Checked)
                {
                    ans_m6 += Q3BR1Q16.Checked ? "1" : "~";
                    ans_m6 += Q3BR2Q16.Checked ? "1" : "~";
                    ans_m6 += Q3BR3Q16.Checked ? "1" : "~";
                    ans_m6 += Q3BR4Q16.Checked ? "1" : "~";
                }
                else
                {
                    ans_m6 = "~~~~";
                }

                //7
                if (Q3BR1Q17.Checked || Q3BR2Q17.Checked || Q3BR3Q17.Checked || Q3BR4Q17.Checked)
                {
                    ans_m7 += Q3BR1Q17.Checked ? "1" : "~";
                    ans_m7 += Q3BR2Q17.Checked ? "1" : "~";
                    ans_m7 += Q3BR3Q17.Checked ? "1" : "~";
                    ans_m7 += Q3BR4Q17.Checked ? "1" : "~";
                }
                else
                {
                    ans_m7 = "~~~~";
                }


                //8
                if (Q3BR1Q18.Checked || Q3BR2Q18.Checked || Q3BR3Q18.Checked || Q3BR4Q18.Checked)
                {
                    ans_m8 += Q3BR1Q18.Checked ? "1" : "~";
                    ans_m8 += Q3BR2Q18.Checked ? "1" : "~";
                    ans_m8 += Q3BR3Q18.Checked ? "1" : "~";
                    ans_m8 += Q3BR4Q18.Checked ? "1" : "~";
                }
                else
                {
                    ans_m8 = "~~~~";
                }

                //9
                if (Q3BR1Q19.Checked || Q3BR2Q19.Checked || Q3BR3Q19.Checked || Q3BR4Q19.Checked)
                {
                    ans_m9 += Q3BR1Q19.Checked ? "1" : "~";
                    ans_m9 += Q3BR2Q19.Checked ? "1" : "~";
                    ans_m9 += Q3BR3Q19.Checked ? "1" : "~";
                    ans_m9 += Q3BR4Q19.Checked ? "1" : "~";
                }

                else
                {
                    ans_m9 = "~~~~";
                }


                //10
                if (Q3BR1Q20.Checked || Q3BR2Q20.Checked || Q3BR3Q20.Checked || Q3BR4Q20.Checked)
                {
                    ans_m10 += Q3BR1Q20.Checked ? "1" : "~";
                    ans_m10 += Q3BR2Q20.Checked ? "1" : "~";
                    ans_m10 += Q3BR3Q20.Checked ? "1" : "~";
                    ans_m10 += Q3BR4Q20.Checked ? "1" : "~";
                }
                else
                {
                    ans_m10 = "~~~~";
                }

                if (ans_m1 != "~~~~" && ans_m1 != "")
                {
                    qq31.BackColor = Color.Green;
                }

                if (ans_m2 != "~~~~" && ans_m2 != "")
                {
                    qq32.BackColor = Color.Green;
                }

                if (ans_m3 != "~~~~" && ans_m3 != "")
                {
                    qq33.BackColor = Color.Green;
                }

                if (ans_m4 != "~~~~" && ans_m4 != "")
                {
                    qq34.BackColor = Color.Green;
                }

                if (ans_m5 != "~~~~" && ans_m5 != "")
                {
                    qq35.BackColor = Color.Green;
                }

                if (ans_m6 != "~~~~" && ans_m6 != "")
                {
                    qq36.BackColor = Color.Green;
                }

                if (ans_m7 != "~~~~" && ans_m7 != "")
                {
                    qq37.BackColor = Color.Green;
                }

                if (ans_m8 != "~~~~" && ans_m8 != "")
                {
                    qq38.BackColor = Color.Green;
                }
                if (ans_m9 != "~~~~" && ans_m9 != "")
                {
                    qq39.BackColor = Color.Green;
                }
                if (ans_m10 != "~~~~" && ans_m10 != "")
                {
                    qq40.BackColor = Color.Green;
                }
                this.Invoke((MethodInvoker)delegate ()
                {
                    Loader_box.Visible = true;
                });


                Common.WriteAnsFile(lbl_seat.Text, "¶Q3B~" + DateTime.Now.ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!1!" + ans_m1);
                Common.WriteAnsFile(lbl_seat.Text, "!2!" + ans_m2);
                Common.WriteAnsFile(lbl_seat.Text, "!3!" + ans_m3);
                Common.WriteAnsFile(lbl_seat.Text, "!4!" + ans_m4);
                Common.WriteAnsFile(lbl_seat.Text, "!5!" + ans_m5);
                Common.WriteAnsFile(lbl_seat.Text, "!6!" + ans_m6);
                Common.WriteAnsFile(lbl_seat.Text, "!7!" + ans_m7);
                Common.WriteAnsFile(lbl_seat.Text, "!8!" + ans_m8);
                Common.WriteAnsFile(lbl_seat.Text, "!9!" + ans_m9);
                Common.WriteAnsFile(lbl_seat.Text, "!10!" + ans_m10);
                this.Invoke((MethodInvoker)delegate ()
                {
                    lbl_filesize.Text = Common.AnsFileSize(lbl_seat.Text) + " Bytes";
                });
                btnQ3_B.Image = ((System.Drawing.Image)(Properties.Resources.Single_Tick));

            }
            catch (Exception ex)
            {
                Send_Error(ex, "3B", "TEXT");
            }

            try
            {
                string ss = webervice_it.Question3B(lbl_Index.Text, lbl_seat.Text, Common.Get_IP(), lbl_paperid.Text, ans_m1, ans_m2, ans_m3, ans_m4, ans_m5, ans_m6, ans_m7, ans_m8, ans_m9, ans_m10);
                btnQ3_B.Image = ((System.Drawing.Image)(Properties.Resources.double_tick));
            }
            catch (Exception exe)
            {
                Send_Error(exe, "3B", "DB");
            }
            this.Invoke((MethodInvoker)delegate ()
            {
                Loader_box.Visible = false;
            });
            MessageBox.Show("Answer Submitted");
        }

        private void btn_Q4_submit_Click(object sender, EventArgs e)
        {
            Thred4 = new Thread(Question4_ThreadCall);
            Thred4.Start();
        }

        private void Question4_ThreadCall()
        {
            int i;
            string ans_mc1 = "", ans_mc2 = "", ans_mc3 = "", ans_mc4 = "", ans_mc5 = "";
            try
            {
                //1
                if (cbox81.Checked || cbox82.Checked || cbox83.Checked || cbox84.Checked || cbox85.Checked || cbox86.Checked)
                {
                    ans_mc1 += cbox81.Checked ? "1" : "~";
                    ans_mc1 += cbox82.Checked ? "1" : "~";
                    ans_mc1 += cbox83.Checked ? "1" : "~";
                    ans_mc1 += cbox84.Checked ? "1" : "~";
                    ans_mc1 += cbox85.Checked ? "1" : "~";
                    ans_mc1 += cbox86.Checked ? "1" : "~";
                }
                else
                {
                    ans_mc1 = "~~~~~~";
                    qq41.BackColor = System.Drawing.Color.Red;
                }

                //2
                if (cbox87.Checked || cbox88.Checked || cbox89.Checked || cbox90.Checked || cbox91.Checked || cbox92.Checked)
                {

                    ans_mc2 += cbox87.Checked ? "1" : "~";
                    ans_mc2 += cbox88.Checked ? "1" : "~";
                    ans_mc2 += cbox89.Checked ? "1" : "~";
                    ans_mc2 += cbox90.Checked ? "1" : "~";
                    ans_mc2 += cbox91.Checked ? "1" : "~";
                    ans_mc2 += cbox92.Checked ? "1" : "~";

                }
                else
                {
                    ans_mc2 = "~~~~~~";
                    qq42.BackColor = System.Drawing.Color.Red;
                }
                //3
                if (cbox93.Checked || cbox94.Checked || cbox95.Checked || cbox96.Checked || cbox97.Checked || cbox98.Checked)
                {

                    ans_mc3 += cbox93.Checked ? "1" : "~";
                    ans_mc3 += cbox94.Checked ? "1" : "~";
                    ans_mc3 += cbox95.Checked ? "1" : "~";
                    ans_mc3 += cbox96.Checked ? "1" : "~";
                    ans_mc3 += cbox97.Checked ? "1" : "~";
                    ans_mc3 += cbox98.Checked ? "1" : "~";
                }
                else
                {
                    ans_mc3 = "~~~~~~";
                    qq43.BackColor = System.Drawing.Color.Red;
                }
                //4
                if (cbox99.Checked || cbox100.Checked || cbox101.Checked || cbox102.Checked || cbox103.Checked || cbox104.Checked)
                {

                    ans_mc4 += cbox99.Checked ? "1" : "~";
                    ans_mc4 += cbox100.Checked ? "1" : "~";
                    ans_mc4 += cbox101.Checked ? "1" : "~";
                    ans_mc4 += cbox102.Checked ? "1" : "~";
                    ans_mc4 += cbox103.Checked ? "1" : "~";
                    ans_mc4 += cbox104.Checked ? "1" : "~";
                }
                else
                {
                    ans_mc4 = "~~~~~~";
                    qq44.BackColor = System.Drawing.Color.Red;
                }
                //5
                if (cbox105.Checked || cbox106.Checked || cbox107.Checked || cbox108.Checked || cbox109.Checked || cbox110.Checked)
                {

                    ans_mc5 += cbox105.Checked ? "1" : "~";
                    ans_mc5 += cbox106.Checked ? "1" : "~";
                    ans_mc5 += cbox107.Checked ? "1" : "~";
                    ans_mc5 += cbox108.Checked ? "1" : "~";
                    ans_mc5 += cbox109.Checked ? "1" : "~";
                    ans_mc5 += cbox110.Checked ? "1" : "~";

                }
                else
                {
                    ans_mc5 = "~~~~~~";
                    qq45.BackColor = System.Drawing.Color.Red;
                }

                if (ans_mc1 != "~~~~~~" && ans_mc1 != "")
                {
                    qq41.BackColor = Color.Green;
                }

                if (ans_mc2 != "~~~~~~" && ans_mc2 != "")
                {
                    qq42.BackColor = Color.Green;
                }

                if (ans_mc3 != "~~~~~~" && ans_mc3 != "")
                {
                    qq43.BackColor = Color.Green;
                }

                if (ans_mc4 != "~~~~~~" && ans_mc4 != "")
                {
                    qq44.BackColor = Color.Green;
                }

                if (ans_mc5 != "~~~~~~" && ans_mc5 != "")
                {
                    qq45.BackColor = Color.Green;
                }

                this.Invoke((MethodInvoker)delegate ()
                {
                    Loader_box.Visible = true;
                });


                Common.WriteAnsFile(lbl_seat.Text, "¶Q4~" + DateTime.Now.ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!1!" + ans_mc1);
                Common.WriteAnsFile(lbl_seat.Text, "!2!" + ans_mc2);
                Common.WriteAnsFile(lbl_seat.Text, "!3!" + ans_mc3);
                Common.WriteAnsFile(lbl_seat.Text, "!4!" + ans_mc4);
                Common.WriteAnsFile(lbl_seat.Text, "!5!" + ans_mc5);

                this.Invoke((MethodInvoker)delegate ()
                {
                    lbl_filesize.Text = Common.AnsFileSize(lbl_seat.Text) + " Bytes";
                });
                btnQ4.Image = ((System.Drawing.Image)(Properties.Resources.Single_Tick));
            }
            catch (Exception ex)
            {
                Send_Error(ex, "4", "TEXT");
            }


            try
            {
                webervice_it.Question4(lbl_Index.Text, lbl_seat.Text, Common.Get_IP(), lbl_paperid.Text, ans_mc1, ans_mc2, ans_mc3, ans_mc4, ans_mc5);
                btnQ4.Image = ((System.Drawing.Image)(Properties.Resources.double_tick));
            }
            catch (Exception exe)
            {
                Send_Error(exe, "4", "DB");
            }
            this.Invoke((MethodInvoker)delegate ()
            {
                Loader_box.Visible = false;
            });
            MessageBox.Show("Answer Submitted");
        }

        private void btn_Q5_submit_Click(object sender, EventArgs e)
        {
            Thred5 = new Thread(Question5_ThreadCall);
            Thred5.Start();
        }

        private void Question5_ThreadCall()
        {
            int i;
            string ans_mc1 = "", ans_mc2 = "";
            try
            {
                if (cbox_111.Checked || cbox_112.Checked || cbox_113.Checked || cbox_114.Checked || cbox_115.Checked || cbox_116.Checked)
                {
                    ans_mc1 += cbox_111.Checked ? "1" : "~";
                    ans_mc1 += cbox_112.Checked ? "1" : "~";
                    ans_mc1 += cbox_113.Checked ? "1" : "~";
                    ans_mc1 += cbox_114.Checked ? "1" : "~";
                    ans_mc1 += cbox_115.Checked ? "1" : "~";
                    ans_mc1 += cbox_116.Checked ? "1" : "~";
                }
                else
                {
                    ans_mc1 = "~~~~~~";
                    qq46.BackColor = System.Drawing.Color.Red;
                }

                //2
                if (cbox_117.Checked || cbox_118.Checked || cbox_119.Checked || cbox_120.Checked || cbox_121.Checked || cbox_122.Checked)
                {
                    ans_mc2 += cbox_117.Checked ? "1" : "~";
                    ans_mc2 += cbox_118.Checked ? "1" : "~";
                    ans_mc2 += cbox_119.Checked ? "1" : "~";
                    ans_mc2 += cbox_120.Checked ? "1" : "~";
                    ans_mc2 += cbox_121.Checked ? "1" : "~";
                    ans_mc2 += cbox_122.Checked ? "1" : "~";

                }
                else
                {
                    ans_mc2 = "~~~~~~";
                    qq47.BackColor = System.Drawing.Color.Red;
                }


                if (ans_mc1 != "~~~~~~" && ans_mc1 != "")
                {
                    qq46.BackColor = Color.Green;
                }

                if (ans_mc2 != "~~~~~~" && ans_mc2 != "")
                {
                    qq47.BackColor = Color.Green;
                }

                this.Invoke((MethodInvoker)delegate ()
                {
                    Loader_box.Visible = true;
                });

                Common.WriteAnsFile(lbl_seat.Text, "¶Q5~" + DateTime.Now.ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!1!" + ans_mc1);
                Common.WriteAnsFile(lbl_seat.Text, "!2!" + ans_mc2);
                this.Invoke((MethodInvoker)delegate ()
                {
                    lbl_filesize.Text = Common.AnsFileSize(lbl_seat.Text) + " Bytes";
                });
                btnQ5.Image = ((System.Drawing.Image)(Properties.Resources.Single_Tick));
            }
            catch (Exception exe)
            {
                Send_Error(exe, "5", "TEXT");
            }
            try
            {
                webervice_it.Question5(lbl_Index.Text, lbl_seat.Text, Common.Get_IP(), lbl_paperid.Text, ans_mc1, ans_mc2);
                btnQ5.Image = ((System.Drawing.Image)(Properties.Resources.double_tick));
            }
            catch (Exception exe)
            {
                Send_Error(exe, "5", "DB");
            }
            this.Invoke((MethodInvoker)delegate ()
            {
                Loader_box.Visible = false;
            });
            MessageBox.Show("Answer Submitted");
        }

        private void btn_Q6_submit_Click(object sender, EventArgs e)
        {
            Thred6 = new Thread(Question6_ThreadCall);
            Thred6.Start();
        }

        private void Question6_ThreadCall()
        {
            string ans_reg1 = txt_reg1.Text.ToUpper();
            string ans_reg2 = txt_reg2.Text.ToUpper();
            try
            {
                if (ans_reg1.Equals(""))
                {
                    qq48.BackColor = Color.Red;
                }
                else
                {
                    qq48.BackColor = Color.Green;
                    //cnt++;++;
                }
                //2
                if (ans_reg2.Equals(""))
                {
                    qq49.BackColor = Color.Red;
                }
                else
                {
                    qq49.BackColor = Color.Green;
                    //cnt++;++;
                }



                this.Invoke((MethodInvoker)delegate ()
                {
                    Loader_box.Visible = true;
                });

                Common.WriteAnsFile(lbl_seat.Text, "¶Q6~" + DateTime.Now.ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!1!" + ans_reg1);
                Common.WriteAnsFile(lbl_seat.Text, "!2!" + ans_reg2);
                this.Invoke((MethodInvoker)delegate ()
                {
                    lbl_filesize.Text = Common.AnsFileSize(lbl_seat.Text) + " Bytes";
                });
                btnQ6.Image = ((System.Drawing.Image)(Properties.Resources.Single_Tick));
            }
            catch (Exception exe)
            {
                Send_Error(exe, "6", "TEXT");
            }

            try
            {
                webervice_it.Question6(lbl_Index.Text, lbl_seat.Text, Common.Get_IP(), lbl_paperid.Text, ans_reg1, ans_reg2);
                btnQ6.Image = ((System.Drawing.Image)(Properties.Resources.double_tick));
            }
            catch (Exception exe)
            {
                Send_Error(exe, "6", "DB");
            }
            this.Invoke((MethodInvoker)delegate ()
            {
                Loader_box.Visible = false;
            });
            MessageBox.Show("Answer Submitted");
        }

        private void btn_Q7_submit_Click(object sender, EventArgs e)
        {
            Thred7 = new Thread(Question7_ThreadCall);
            Thred7.Start();
        }

        private void Question7_ThreadCall()
        {
            string ans_7a = "", ans_7b = "", ans_7c = "";
            try
            {
                ans_7a = txt_7a.Text.Replace("\r\n", "Ω");
                ans_7b = txt_7b.Text.Replace("\r\n", "Ω");
                ans_7c = txt_7c.Text.Replace("\r\n", "Ω");

                ans_7a = txt_7a.Text.ToString().Replace("\r\n", "Ω");




                if (ans_7a.Equals(""))
                {
                    qq50.BackColor = Color.Red;
                }

                else
                {
                    qq50.BackColor = Color.Green;
                    //cnt++;++;
                }
                //2

                ans_7b = txt_7b.Text.ToString().Replace("\r\n", "Ω");


                if (ans_7b.Equals(""))
                {
                    qq51.BackColor = Color.Red;
                }

                else
                {
                    qq51.BackColor = Color.Green;
                    //cnt++;++;
                }
                //3

                ans_7c = txt_7c.Text.ToString().Replace("\r\n", "Ω");


                if (ans_7c.Equals(""))
                {
                    qq52.BackColor = Color.Red;

                }

                else
                {
                    qq52.BackColor = Color.Green;
                    //cnt++;++;
                }
                this.Invoke((MethodInvoker)delegate ()
                {
                    Loader_box.Visible = true;
                });

                Common.WriteAnsFile(lbl_seat.Text, "¶Q7~" + DateTime.Now.ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!1!" + ans_7a);
                Common.WriteAnsFile(lbl_seat.Text, "!2!" + ans_7b);
                Common.WriteAnsFile(lbl_seat.Text, "!3!" + ans_7c);
                this.Invoke((MethodInvoker)delegate ()
                {
                    lbl_filesize.Text = Common.AnsFileSize(lbl_seat.Text) + " Bytes";
                });
                btnQ7.Image = ((System.Drawing.Image)(Properties.Resources.Single_Tick));
            }
            catch (Exception exe)
            {
                Send_Error(exe, "7", "TEXT");
            }

            try
            {
                webervice_it.Question7(lbl_Index.Text, lbl_seat.Text, Common.Get_IP(), lbl_paperid.Text, ans_7a, ans_7b, ans_7c);
                btnQ7.Image = ((System.Drawing.Image)(Properties.Resources.double_tick));
            }
            catch (Exception exe)
            {
                Send_Error(exe, "7", "DB");
            }
            this.Invoke((MethodInvoker)delegate ()
            {
                Loader_box.Visible = false;
            });
            MessageBox.Show("Answer Submitted");
        }

        private void btn_subq8_Click(object sender, EventArgs e)
        {
            Thred8 = new Thread(Question8_ThreadCall);
            Thred8.Start();
        }

        private void Question8_ThreadCall()
        {
            string ans_8a = "", ans_8b = "";
            try
            {
                ans_8a = txt_8a.Text.ToString().Replace("\r\n", "Ω");



                if (ans_8a.Equals(""))
                {
                    qq53.BackColor = Color.Red;
                }

                else
                {
                    qq53.BackColor = Color.Green;
                    //cnt++;++;
                }

                //q8b

                ans_8b = txt_8b.Text.ToString().Replace("\r\n", "Ω");


                if (ans_8b.Equals(""))
                {
                    qq54.BackColor = Color.Red;
                }

                else
                {
                    qq54.BackColor = Color.Green;
                    //cnt++;++;
                }
                this.Invoke((MethodInvoker)delegate ()
                {
                    Loader_box.Visible = true;
                });

                Common.WriteAnsFile(lbl_seat.Text, "¶Q8~" + DateTime.Now.ToString());
                Common.WriteAnsFile(lbl_seat.Text, "!1!" + ans_8a);
                Common.WriteAnsFile(lbl_seat.Text, "!2!" + ans_8b);

                this.Invoke((MethodInvoker)delegate ()
                {
                    lbl_filesize.Text = Common.AnsFileSize(lbl_seat.Text) + " Bytes";
                });
                btnQ8.Image = ((System.Drawing.Image)(Properties.Resources.Single_Tick));
            }
            catch (Exception exe)
            {
                Send_Error(exe, "8", "TEXT");
            }

            try
            {
                webervice_it.Question8(lbl_Index.Text, lbl_seat.Text, Common.Get_IP(), lbl_paperid.Text, ans_8a, ans_8b);
                btnQ8.Image = ((System.Drawing.Image)(Properties.Resources.double_tick));
            }
            catch (Exception exe)
            {
                Send_Error(exe, "8", "DB");
            }
            this.Invoke((MethodInvoker)delegate ()
            {
                Loader_box.Visible = false;
            });
            MessageBox.Show("Answer Submitted");
        }

        #endregion
        public void Read_ReloginData()
        {
            DataSet ds = new DataSet();
            ds = webervice_it.GetReloginData(lbl_Index.Text, lbl_seat.Text, lbl_paperid.Text);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables["Question1"].Rows.Count > 0)
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        txtans1.Text = ds.Tables["Question1"].Rows[0]["ans1"].ToString();
                        txtans2.Text = ds.Tables["Question1"].Rows[0]["ans2"].ToString();
                        txtans3.Text = ds.Tables["Question1"].Rows[0]["ans3"].ToString();
                        txtans4.Text = ds.Tables["Question1"].Rows[0]["ans4"].ToString();
                        txtans5.Text = ds.Tables["Question1"].Rows[0]["ans5"].ToString();
                        txtans6.Text = ds.Tables["Question1"].Rows[0]["ans6"].ToString();
                        txtans7.Text = ds.Tables["Question1"].Rows[0]["ans7"].ToString();
                        txtans8.Text = ds.Tables["Question1"].Rows[0]["ans8"].ToString();
                        txtans9.Text = ds.Tables["Question1"].Rows[0]["ans9"].ToString();
                        txtans10.Text = ds.Tables["Question1"].Rows[0]["ans10"].ToString();
                    });
                }
                if (ds.Tables["Question2"].Rows.Count > 0)
                {
                    if (ds.Tables["Question2"].Rows[0]["ans1"].ToString() == "True")
                    {
                        rb_tf1.Checked = true;
                        rb_tf2.Checked = false;
                    }
                    else if (ds.Tables["Question2"].Rows[0]["ans1"].ToString() == "False")
                    {
                        rb_tf1.Checked = false;
                        rb_tf2.Checked = true;

                    }
                    else
                    {
                        rb_tf1.Checked = false;
                        rb_tf2.Checked = false;
                    }

                    if (ds.Tables["Question2"].Rows[0]["ans2"].ToString() == "True")
                    {
                        rb_tf3.Checked = true;
                        rb_tf4.Checked = false;
                    }
                    else if (ds.Tables["Question2"].Rows[0]["ans2"].ToString() == "False")
                    {
                        rb_tf3.Checked = false;
                        rb_tf4.Checked = true;

                    }
                    else
                    {
                        rb_tf3.Checked = false;
                        rb_tf4.Checked = false;
                    }

                    if (ds.Tables["Question2"].Rows[0]["ans3"].ToString() == "True")
                    {
                        rb_tf5.Checked = true;
                        rb_tf6.Checked = false;
                    }
                    else if (ds.Tables["Question2"].Rows[0]["ans3"].ToString() == "False")
                    {
                        rb_tf5.Checked = false;
                        rb_tf6.Checked = true;

                    }
                    else
                    {
                        rb_tf5.Checked = false;
                        rb_tf6.Checked = false;
                    }

                    if (ds.Tables["Question2"].Rows[0]["ans4"].ToString() == "True")
                    {
                        rb_tf7.Checked = true;
                        rb_tf8.Checked = false;
                    }
                    else if (ds.Tables["Question2"].Rows[0]["ans4"].ToString() == "False")
                    {
                        rb_tf7.Checked = false;
                        rb_tf8.Checked = true;
                    }
                    else
                    {
                        rb_tf7.Checked = false;
                        rb_tf8.Checked = false;
                    }

                    if (ds.Tables["Question2"].Rows[0]["ans5"].ToString() == "True")
                    {
                        rb_tf9.Checked = true;
                        rb_tf10.Checked = false;
                    }
                    else if (ds.Tables["Question2"].Rows[0]["ans5"].ToString() == "False")
                    {
                        rb_tf9.Checked = false;
                        rb_tf10.Checked = true;
                    }
                    else
                    {
                        rb_tf9.Checked = false;
                        rb_tf10.Checked = false;
                    }

                    if (ds.Tables["Question2"].Rows[0]["ans6"].ToString() == "True")
                    {
                        rb_tf11.Checked = true;
                        rb_tf12.Checked = false;
                    }
                    else if (ds.Tables["Question2"].Rows[0]["ans6"].ToString() == "False")
                    {
                        rb_tf11.Checked = false;
                        rb_tf12.Checked = true;

                    }
                    else
                    {
                        rb_tf11.Checked = false;
                        rb_tf12.Checked = false;
                    }

                    if (ds.Tables["Question2"].Rows[0]["ans7"].ToString() == "True")
                    {
                        rb_tf13.Checked = true;
                        rb_tf14.Checked = false;
                    }
                    else if (ds.Tables["Question2"].Rows[0]["ans7"].ToString() == "False")
                    {
                        rb_tf13.Checked = false;
                        rb_tf14.Checked = true;

                    }
                    else
                    {
                        rb_tf13.Checked = false;
                        rb_tf14.Checked = false;
                    }

                    if (ds.Tables["Question2"].Rows[0]["ans8"].ToString() == "True")
                    {
                        rb_tf15.Checked = true;
                        rb_tf16.Checked = false;
                    }
                    else if (ds.Tables["Question2"].Rows[0]["ans8"].ToString() == "False")
                    {
                        rb_tf15.Checked = false;
                        rb_tf16.Checked = true;

                    }
                    else
                    {
                        rb_tf15.Checked = false;
                        rb_tf16.Checked = false;
                    }

                    if (ds.Tables["Question2"].Rows[0]["ans9"].ToString() == "True")
                    {
                        rb_tf17.Checked = true;
                        rb_tf18.Checked = false;
                    }
                    else if (ds.Tables["Question2"].Rows[0]["ans9"].ToString() == "False")
                    {
                        rb_tf17.Checked = false;
                        rb_tf18.Checked = true;
                    }
                    else
                    {
                        rb_tf17.Checked = false;
                        rb_tf18.Checked = false;
                    }

                    if (ds.Tables["Question2"].Rows[0]["ans10"].ToString() == "True")
                    {
                        rb_tf19.Checked = true;
                        rb_tf20.Checked = false;
                    }
                    else if (ds.Tables["Question2"].Rows[0]["ans10"].ToString() == "False")
                    {
                        rb_tf19.Checked = false;
                        rb_tf20.Checked = true;
                    }
                    else
                    {
                        rb_tf19.Checked = false;
                        rb_tf20.Checked = false;
                    }
                }
                if (ds.Tables["Question3A"].Rows.Count > 0)
                {
                    char[] characters1 = ds.Tables["Question3A"].Rows[0]["ans1"].ToString().ToCharArray();
                    if (characters1.Length == 4)
                    {
                        Q3AR1Q1.Checked = (characters1[0] == '1') ? true : false;
                        Q3AR2Q1.Checked = (characters1[1] == '1') ? true : false;
                        Q3AR3Q1.Checked = (characters1[2] == '1') ? true : false;
                        Q3AR4Q1.Checked = (characters1[3] == '1') ? true : false;
                    }
                    char[] characters2 = ds.Tables["Question3A"].Rows[0]["ans2"].ToString().ToCharArray();
                    if (characters2.Length == 4)
                    {
                        Q3AR1Q2.Checked = (characters2[0] == '1') ? true : false;
                        Q3AR2Q2.Checked = (characters2[1] == '1') ? true : false;
                        Q3AR3Q2.Checked = (characters2[2] == '1') ? true : false;
                        Q3AR4Q2.Checked = (characters2[3] == '1') ? true : false;
                    }
                    char[] characters3 = ds.Tables["Question3A"].Rows[0]["ans3"].ToString().ToCharArray();
                    if (characters3.Length == 4)
                    {
                        Q3AR1Q3.Checked = (characters3[0] == '1') ? true : false;
                        Q3AR2Q3.Checked = (characters3[1] == '1') ? true : false;
                        Q3AR3Q3.Checked = (characters3[2] == '1') ? true : false;
                        Q3AR4Q3.Checked = (characters3[3] == '1') ? true : false;
                    }
                    char[] characters4 = ds.Tables["Question3A"].Rows[0]["ans4"].ToString().ToCharArray();
                    if (characters4.Length == 4)
                    {
                        Q3AR1Q4.Checked = (characters4[0] == '1') ? true : false;
                        Q3AR2Q4.Checked = (characters4[1] == '1') ? true : false;
                        Q3AR3Q4.Checked = (characters4[2] == '1') ? true : false;
                        Q3AR4Q4.Checked = (characters4[3] == '1') ? true : false;
                    }

                    char[] characters5 = ds.Tables["Question3A"].Rows[0]["ans5"].ToString().ToCharArray();
                    if (characters5.Length == 4)
                    {
                        Q3AR1Q5.Checked = (characters5[0] == '1') ? true : false;
                        Q3AR2Q5.Checked = (characters5[1] == '1') ? true : false;
                        Q3AR3Q5.Checked = (characters5[2] == '1') ? true : false;
                        Q3AR4Q5.Checked = (characters5[3] == '1') ? true : false;
                    }

                    char[] characters6 = ds.Tables["Question3A"].Rows[0]["ans6"].ToString().ToCharArray();
                    if (characters6.Length == 4)
                    {
                        Q3AR1Q6.Checked = (characters6[0] == '1') ? true : false;
                        Q3AR2Q6.Checked = (characters6[1] == '1') ? true : false;
                        Q3AR3Q6.Checked = (characters6[2] == '1') ? true : false;
                        Q3AR4Q6.Checked = (characters6[3] == '1') ? true : false;
                    }

                    char[] characters7 = ds.Tables["Question3A"].Rows[0]["ans7"].ToString().ToCharArray();
                    if (characters7.Length == 4)
                    {
                        Q3AR1Q7.Checked = (characters7[0] == '1') ? true : false;
                        Q3AR2Q7.Checked = (characters7[1] == '1') ? true : false;
                        Q3AR3Q7.Checked = (characters7[2] == '1') ? true : false;
                        Q3AR4Q7.Checked = (characters7[3] == '1') ? true : false;
                    }


                    char[] characters8 = ds.Tables["Question3A"].Rows[0]["ans8"].ToString().ToCharArray();
                    if (characters8.Length == 4)
                    {
                        Q3AR1Q8.Checked = (characters8[0] == '1') ? true : false;
                        Q3AR2Q8.Checked = (characters8[1] == '1') ? true : false;
                        Q3AR3Q8.Checked = (characters8[2] == '1') ? true : false;
                        Q3AR4Q8.Checked = (characters8[3] == '1') ? true : false;
                    }

                    char[] characters9 = ds.Tables["Question3A"].Rows[0]["ans9"].ToString().ToCharArray();
                    if (characters9.Length == 4)
                    {
                        Q3AR1Q9.Checked = (characters9[0] == '1') ? true : false;
                        Q3AR2Q9.Checked = (characters9[1] == '1') ? true : false;
                        Q3AR3Q9.Checked = (characters9[2] == '1') ? true : false;
                        Q3AR4Q9.Checked = (characters9[3] == '1') ? true : false;
                    }

                    char[] characters10 = ds.Tables["Question3A"].Rows[0]["ans10"].ToString().ToCharArray();
                    if (characters10.Length == 4)
                    {
                        Q3AR1Q10.Checked = (characters10[0] == '1') ? true : false;
                        Q3AR2Q10.Checked = (characters10[1] == '1') ? true : false;
                        Q3AR3Q10.Checked = (characters10[2] == '1') ? true : false;
                        Q3AR4Q10.Checked = (characters10[3] == '1') ? true : false;
                    }



                }
                if (ds.Tables["Question3B"].Rows.Count > 0)
                {
                    char[] characters1 = ds.Tables["Question3B"].Rows[0]["ans11"].ToString().ToCharArray();
                    if (characters1.Length == 4)
                    {
                        Q3BR1Q11.Checked = (characters1[0] == '1') ? true : false;
                        Q3BR2Q11.Checked = (characters1[1] == '1') ? true : false;
                        Q3BR3Q11.Checked = (characters1[2] == '1') ? true : false;
                        Q3BR4Q11.Checked = (characters1[3] == '1') ? true : false;
                    }

                    char[] characters2 = ds.Tables["Question3B"].Rows[0]["ans12"].ToString().ToCharArray();
                    if (characters2.Length == 4)
                    {
                        Q3BR1Q12.Checked = (characters2[0] == '1') ? true : false;
                        Q3BR2Q12.Checked = (characters2[1] == '1') ? true : false;
                        Q3BR3Q12.Checked = (characters2[2] == '1') ? true : false;
                        Q3BR4Q12.Checked = (characters2[3] == '1') ? true : false;
                    }

                    char[] characters3 = ds.Tables["Question3B"].Rows[0]["ans13"].ToString().ToCharArray();
                    if (characters3.Length == 4)
                    {
                        Q3BR1Q13.Checked = (characters3[0] == '1') ? true : false;
                        Q3BR2Q13.Checked = (characters3[1] == '1') ? true : false;
                        Q3BR3Q13.Checked = (characters3[2] == '1') ? true : false;
                        Q3BR4Q13.Checked = (characters3[3] == '1') ? true : false;
                    }

                    char[] characters4 = ds.Tables["Question3B"].Rows[0]["ans14"].ToString().ToCharArray();
                    if (characters4.Length == 4)
                    {
                        Q3BR1Q14.Checked = (characters4[0] == '1') ? true : false;
                        Q3BR2Q14.Checked = (characters4[1] == '1') ? true : false;
                        Q3BR3Q14.Checked = (characters4[2] == '1') ? true : false;
                        Q3BR4Q14.Checked = (characters4[3] == '1') ? true : false;
                    }


                    char[] characters5 = ds.Tables["Question3B"].Rows[0]["ans15"].ToString().ToCharArray();
                    if (characters5.Length == 4)
                    {
                        Q3BR1Q15.Checked = (characters5[0] == '1') ? true : false;
                        Q3BR2Q15.Checked = (characters5[1] == '1') ? true : false;
                        Q3BR3Q15.Checked = (characters5[2] == '1') ? true : false;
                        Q3BR4Q15.Checked = (characters5[3] == '1') ? true : false;
                    }


                    char[] characters6 = ds.Tables["Question3B"].Rows[0]["ans16"].ToString().ToCharArray();
                    if (characters6.Length == 4)
                    {
                        Q3BR1Q16.Checked = (characters6[0] == '1') ? true : false;
                        Q3BR2Q16.Checked = (characters6[1] == '1') ? true : false;
                        Q3BR3Q16.Checked = (characters6[2] == '1') ? true : false;
                        Q3BR4Q16.Checked = (characters6[3] == '1') ? true : false;
                    }


                    char[] characters7 = ds.Tables["Question3B"].Rows[0]["ans17"].ToString().ToCharArray();
                    if (characters7.Length == 4)
                    {
                        Q3BR1Q17.Checked = (characters7[0] == '1') ? true : false;
                        Q3BR2Q17.Checked = (characters7[1] == '1') ? true : false;
                        Q3BR3Q17.Checked = (characters7[2] == '1') ? true : false;
                        Q3BR4Q17.Checked = (characters7[3] == '1') ? true : false;
                    }


                    char[] characters8 = ds.Tables["Question3B"].Rows[0]["ans18"].ToString().ToCharArray();
                    if (characters8.Length == 4)
                    {
                        Q3BR1Q18.Checked = (characters8[0] == '1') ? true : false;
                        Q3BR2Q18.Checked = (characters8[1] == '1') ? true : false;
                        Q3BR3Q18.Checked = (characters8[2] == '1') ? true : false;
                        Q3BR4Q18.Checked = (characters8[3] == '1') ? true : false;
                    }

                    char[] characters9 = ds.Tables["Question3B"].Rows[0]["ans19"].ToString().ToCharArray();
                    if (characters9.Length == 4)
                    {
                        Q3BR1Q19.Checked = (characters9[0] == '1') ? true : false;
                        Q3BR2Q19.Checked = (characters9[1] == '1') ? true : false;
                        Q3BR3Q19.Checked = (characters9[2] == '1') ? true : false;
                        Q3BR4Q19.Checked = (characters9[3] == '1') ? true : false;
                    }


                    char[] characters10 = ds.Tables["Question3B"].Rows[0]["ans20"].ToString().ToCharArray();
                    if (characters10.Length == 4)
                    {
                        Q3BR1Q20.Checked = (characters10[0] == '1') ? true : false;
                        Q3BR2Q20.Checked = (characters10[1] == '1') ? true : false;
                        Q3BR3Q20.Checked = (characters10[2] == '1') ? true : false;
                        Q3BR4Q20.Checked = (characters10[3] == '1') ? true : false;
                    }
                }
                if (ds.Tables["Question4"].Rows.Count > 0)
                {
                    char[] characters1 = ds.Tables["Question4"].Rows[0]["ans1"].ToString().ToCharArray();
                    if (characters1.Length == 6)
                    {
                        cbox81.Checked = (characters1[0] == '1') ? true : false;
                        cbox82.Checked = (characters1[1] == '1') ? true : false;
                        cbox83.Checked = (characters1[2] == '1') ? true : false;
                        cbox84.Checked = (characters1[3] == '1') ? true : false;
                        cbox85.Checked = (characters1[4] == '1') ? true : false;
                        cbox86.Checked = (characters1[5] == '1') ? true : false;
                    }

                    char[] characters2 = ds.Tables["Question4"].Rows[0]["ans2"].ToString().ToCharArray();
                    if (characters2.Length == 6)
                    {
                        cbox87.Checked = (characters2[0] == '1') ? true : false;
                        cbox88.Checked = (characters2[1] == '1') ? true : false;
                        cbox89.Checked = (characters2[2] == '1') ? true : false;
                        cbox90.Checked = (characters2[3] == '1') ? true : false;
                        cbox91.Checked = (characters2[4] == '1') ? true : false;
                        cbox92.Checked = (characters2[5] == '1') ? true : false;
                    }

                    char[] characters3 = ds.Tables["Question4"].Rows[0]["ans3"].ToString().ToCharArray();
                    if (characters3.Length == 6)
                    {
                        cbox93.Checked = (characters3[0] == '1') ? true : false;
                        cbox94.Checked = (characters3[1] == '1') ? true : false;
                        cbox95.Checked = (characters3[2] == '1') ? true : false;
                        cbox96.Checked = (characters3[3] == '1') ? true : false;
                        cbox97.Checked = (characters3[4] == '1') ? true : false;
                        cbox98.Checked = (characters3[5] == '1') ? true : false;
                    }

                    char[] characters4 = ds.Tables["Question4"].Rows[0]["ans4"].ToString().ToCharArray();
                    if (characters4.Length == 6)
                    {
                        cbox99.Checked = (characters4[0] == '1') ? true : false;
                        cbox100.Checked = (characters4[1] == '1') ? true : false;
                        cbox101.Checked = (characters4[2] == '1') ? true : false;
                        cbox102.Checked = (characters4[3] == '1') ? true : false;
                        cbox103.Checked = (characters4[4] == '1') ? true : false;
                        cbox104.Checked = (characters4[5] == '1') ? true : false;
                    }


                    char[] characters5 = ds.Tables["Question4"].Rows[0]["ans5"].ToString().ToCharArray();
                    if (characters5.Length == 6)
                    {
                        cbox105.Checked = (characters5[0] == '1') ? true : false;
                        cbox106.Checked = (characters5[1] == '1') ? true : false;
                        cbox107.Checked = (characters5[2] == '1') ? true : false;
                        cbox108.Checked = (characters5[3] == '1') ? true : false;
                        cbox109.Checked = (characters5[4] == '1') ? true : false;
                        cbox110.Checked = (characters5[5] == '1') ? true : false;
                    }


                }
                if (ds.Tables["Question5"].Rows.Count > 0)
                {
                    char[] characters1 = ds.Tables["Question5"].Rows[0]["ans1"].ToString().ToCharArray();
                    if (characters1.Length == 6)
                    {
                        cbox_111.Checked = (characters1[0] == '1') ? true : false;
                        cbox_112.Checked = (characters1[1] == '1') ? true : false;
                        cbox_113.Checked = (characters1[2] == '1') ? true : false;
                        cbox_114.Checked = (characters1[3] == '1') ? true : false;
                        cbox_115.Checked = (characters1[4] == '1') ? true : false;
                        cbox_116.Checked = (characters1[5] == '1') ? true : false;
                    }

                    char[] characters2 = ds.Tables["Question5"].Rows[0]["ans2"].ToString().ToCharArray();
                    if (characters2.Length == 6)
                    {
                        cbox_117.Checked = (characters2[0] == '1') ? true : false;
                        cbox_118.Checked = (characters2[1] == '1') ? true : false;
                        cbox_119.Checked = (characters2[2] == '1') ? true : false;
                        cbox_120.Checked = (characters2[3] == '1') ? true : false;
                        cbox_121.Checked = (characters2[4] == '1') ? true : false;
                        cbox_122.Checked = (characters2[5] == '1') ? true : false;
                    }
                }
                if (ds.Tables["Question6"].Rows.Count > 0)
                {
                    txt_reg1.Text = ds.Tables["Question6"].Rows[0]["ans1"].ToString();
                    txt_reg2.Text = ds.Tables["Question6"].Rows[0]["ans2"].ToString();
                }
                if (ds.Tables["Question7"].Rows.Count > 0)
                {
                    txt_7a.Text = ds.Tables["Question7"].Rows[0]["ans1"].ToString().Replace("Ω", "\r\n");
                    txt_7b.Text = ds.Tables["Question7"].Rows[0]["ans2"].ToString().Replace("Ω", "\r\n");
                    txt_7c.Text = ds.Tables["Question7"].Rows[0]["ans3"].ToString().Replace("Ω", "\r\n");
                }
                if (ds.Tables["Question8"].Rows.Count > 0)
                {
                    txt_8a.Text = ds.Tables["Question8"].Rows[0]["ans1"].ToString().Replace("Ω", "\r\n");
                    txt_8b.Text = ds.Tables["Question8"].Rows[0]["ans2"].ToString().Replace("Ω", "\r\n");
                }

            }

        }
        //Enable Disable Control
        #region 
        public void Enable_Control()
        {
            txtans1.Enabled = true;
            txtans2.Enabled = true;
            txtans3.Enabled = true;
            txtans4.Enabled = true;
            txtans5.Enabled = true;
            txtans6.Enabled = true;
            txtans7.Enabled = true;
            txtans8.Enabled = true;
            txtans9.Enabled = true;
            txtans10.Enabled = true;
            rb_tf1.Enabled = true;
            rb_tf2.Enabled = true;
            rb_tf3.Enabled = true;
            rb_tf4.Enabled = true;
            rb_tf5.Enabled = true;
            rb_tf6.Enabled = true;
            rb_tf7.Enabled = true;
            rb_tf8.Enabled = true;
            rb_tf9.Enabled = true;
            rb_tf10.Enabled = true;
            rb_tf11.Enabled = true;
            rb_tf12.Enabled = true;
            rb_tf13.Enabled = true;
            rb_tf14.Enabled = true;
            rb_tf15.Enabled = true;
            rb_tf16.Enabled = true;
            rb_tf17.Enabled = true;
            rb_tf18.Enabled = true;
            rb_tf19.Enabled = true;
            rb_tf20.Enabled = true;
            rb_tf16.Enabled = true;
            Q3AR1Q1.Enabled = true;
            Q3AR2Q1.Enabled = true;
            Q3AR3Q1.Enabled = true;
            Q3AR4Q1.Enabled = true;
            Q3AR1Q2.Enabled = true;
            Q3AR2Q2.Enabled = true;
            Q3AR3Q2.Enabled = true;
            Q3AR4Q2.Enabled = true;
            Q3AR1Q3.Enabled = true;
            Q3AR2Q3.Enabled = true;
            Q3AR3Q3.Enabled = true;
            Q3AR4Q3.Enabled = true;
            Q3AR1Q4.Enabled = true;
            Q3AR2Q4.Enabled = true;
            Q3AR3Q4.Enabled = true;
            Q3AR4Q4.Enabled = true;
            Q3AR1Q5.Enabled = true;
            Q3AR2Q5.Enabled = true;
            Q3AR3Q5.Enabled = true;
            Q3AR4Q5.Enabled = true;
            Q3AR1Q6.Enabled = true;
            Q3AR2Q6.Enabled = true;
            Q3AR3Q6.Enabled = true;
            Q3AR4Q6.Enabled = true;
            Q3AR1Q7.Enabled = true;
            Q3AR2Q7.Enabled = true;
            Q3AR3Q7.Enabled = true;
            Q3AR4Q7.Enabled = true;
            Q3AR1Q8.Enabled = true;
            Q3AR2Q8.Enabled = true;
            Q3AR3Q8.Enabled = true;
            Q3AR4Q8.Enabled = true;
            Q3AR1Q9.Enabled = true;
            Q3AR2Q9.Enabled = true;
            Q3AR3Q9.Enabled = true;
            Q3AR4Q9.Enabled = true;
            Q3AR1Q10.Enabled = true;
            Q3AR2Q10.Enabled = true;
            Q3AR3Q10.Enabled = true;
            Q3AR4Q10.Enabled = true;
            Q3BR1Q11.Enabled = true;
            Q3BR2Q11.Enabled = true;
            Q3BR3Q11.Enabled = true;
            Q3BR4Q11.Enabled = true;
            Q3BR1Q12.Enabled = true;
            Q3BR2Q12.Enabled = true;
            Q3BR3Q12.Enabled = true;
            Q3BR4Q12.Enabled = true;
            Q3BR1Q13.Enabled = true;
            Q3BR2Q13.Enabled = true;
            Q3BR3Q13.Enabled = true;
            Q3BR4Q13.Enabled = true;
            Q3BR1Q14.Enabled = true;
            Q3BR2Q14.Enabled = true;
            Q3BR3Q14.Enabled = true;
            Q3BR4Q14.Enabled = true;
            Q3BR1Q15.Enabled = true;
            Q3BR2Q15.Enabled = true;
            Q3BR3Q15.Enabled = true;
            Q3BR4Q15.Enabled = true;
            Q3BR1Q16.Enabled = true;
            Q3BR2Q16.Enabled = true;
            Q3BR3Q16.Enabled = true;
            Q3BR4Q16.Enabled = true;
            Q3BR1Q17.Enabled = true;
            Q3BR2Q17.Enabled = true;
            Q3BR3Q17.Enabled = true;
            Q3BR4Q17.Enabled = true;
            Q3BR1Q18.Enabled = true;
            Q3BR2Q18.Enabled = true;
            Q3BR3Q18.Enabled = true;
            Q3BR4Q18.Enabled = true;
            Q3BR1Q19.Enabled = true;
            Q3BR2Q19.Enabled = true;
            Q3BR3Q19.Enabled = true;
            Q3BR4Q19.Enabled = true;
            Q3BR1Q20.Enabled = true;
            Q3BR2Q20.Enabled = true;
            Q3BR3Q20.Enabled = true;
            Q3BR4Q20.Enabled = true;

            cbox81.Enabled = true;
            cbox82.Enabled = true;
            cbox83.Enabled = true;
            cbox84.Enabled = true;
            cbox85.Enabled = true;
            cbox86.Enabled = true;
            cbox87.Enabled = true;
            cbox88.Enabled = true;
            cbox89.Enabled = true;
            cbox90.Enabled = true;
            cbox91.Enabled = true;
            cbox92.Enabled = true;
            cbox93.Enabled = true;
            cbox94.Enabled = true;
            cbox95.Enabled = true;
            cbox96.Enabled = true;
            cbox97.Enabled = true;
            cbox98.Enabled = true;
            cbox99.Enabled = true;
            cbox100.Enabled = true;
            cbox101.Enabled = true;
            cbox102.Enabled = true;
            cbox103.Enabled = true;
            cbox104.Enabled = true;
            cbox105.Enabled = true;
            cbox106.Enabled = true;
            cbox107.Enabled = true;
            cbox108.Enabled = true;
            cbox109.Enabled = true;
            cbox110.Enabled = true;
            cbox_111.Enabled = true;
            cbox_112.Enabled = true;
            cbox_113.Enabled = true;
            cbox_114.Enabled = true;
            cbox_115.Enabled = true;
            cbox_116.Enabled = true;
            cbox_117.Enabled = true;
            cbox_118.Enabled = true;
            cbox_119.Enabled = true;
            cbox_120.Enabled = true;
            cbox_121.Enabled = true;
            cbox_122.Enabled = true;
            txt_reg1.Enabled = true;
            txt_reg2.Enabled = true;
            txt_7a.Enabled = true;
            txt_7b.Enabled = true;
            txt_7c.Enabled = true;
            txt_8a.Enabled = true;
            txt_8b.Enabled = true;
            btn_Q1_submit.Enabled = true;
            btn_Q2_submit.Enabled = true;
            btn_Q3A_submit.Enabled = true;
            btn_Q3B_submit.Enabled = true;
            btn_Q4_submit.Enabled = true;
            btn_Q5_submit.Enabled = true;
            btn_Q6_submit.Enabled = true;
            btn_Q7_submit.Enabled = true;
            btn_subq8.Enabled = true;
            btnsub.Enabled = true;

        }
        public void Disable_Control()
        {
            txtans1.Enabled = false;
            txtans2.Enabled = false;
            txtans3.Enabled = false;
            txtans4.Enabled = false;
            txtans5.Enabled = false;
            txtans6.Enabled = false;
            txtans7.Enabled = false;
            txtans8.Enabled = false;
            txtans9.Enabled = false;
            txtans10.Enabled = false;
            rb_tf1.Enabled = false;
            rb_tf2.Enabled = false;
            rb_tf3.Enabled = false;
            rb_tf4.Enabled = false;
            rb_tf5.Enabled = false;
            rb_tf6.Enabled = false;
            rb_tf7.Enabled = false;
            rb_tf8.Enabled = false;
            rb_tf9.Enabled = false;
            rb_tf10.Enabled = false;
            rb_tf11.Enabled = false;
            rb_tf12.Enabled = false;
            rb_tf13.Enabled = false;
            rb_tf14.Enabled = false;
            rb_tf15.Enabled = false;
            rb_tf16.Enabled = false;
            rb_tf17.Enabled = false;
            rb_tf18.Enabled = false;
            rb_tf19.Enabled = false;
            rb_tf20.Enabled = false;
            rb_tf16.Enabled = false;
            Q3AR1Q1.Enabled = false;
            Q3AR2Q1.Enabled = false;
            Q3AR3Q1.Enabled = false;
            Q3AR4Q1.Enabled = false;
            Q3AR1Q2.Enabled = false;
            Q3AR2Q2.Enabled = false;
            Q3AR3Q2.Enabled = false;
            Q3AR4Q2.Enabled = false;
            Q3AR1Q3.Enabled = false;
            Q3AR2Q3.Enabled = false;
            Q3AR3Q3.Enabled = false;
            Q3AR4Q3.Enabled = false;
            Q3AR1Q4.Enabled = false;
            Q3AR2Q4.Enabled = false;
            Q3AR3Q4.Enabled = false;
            Q3AR4Q4.Enabled = false;
            Q3AR1Q5.Enabled = false;
            Q3AR2Q5.Enabled = false;
            Q3AR3Q5.Enabled = false;
            Q3AR4Q5.Enabled = false;
            Q3AR1Q6.Enabled = false;
            Q3AR2Q6.Enabled = false;
            Q3AR3Q6.Enabled = false;
            Q3AR4Q6.Enabled = false;
            Q3AR1Q7.Enabled = false;
            Q3AR2Q7.Enabled = false;
            Q3AR3Q7.Enabled = false;
            Q3AR4Q7.Enabled = false;
            Q3AR1Q8.Enabled = false;
            Q3AR2Q8.Enabled = false;
            Q3AR3Q8.Enabled = false;
            Q3AR4Q8.Enabled = false;
            Q3AR1Q9.Enabled = false;
            Q3AR2Q9.Enabled = false;
            Q3AR3Q9.Enabled = false;
            Q3AR4Q9.Enabled = false;
            Q3AR1Q10.Enabled = false;
            Q3AR2Q10.Enabled = false;
            Q3AR3Q10.Enabled = false;
            Q3AR4Q10.Enabled = false;
            Q3BR1Q11.Enabled = false;
            Q3BR2Q11.Enabled = false;
            Q3BR3Q11.Enabled = false;
            Q3BR4Q11.Enabled = false;
            Q3BR1Q12.Enabled = false;
            Q3BR2Q12.Enabled = false;
            Q3BR3Q12.Enabled = false;
            Q3BR4Q12.Enabled = false;
            Q3BR1Q13.Enabled = false;
            Q3BR2Q13.Enabled = false;
            Q3BR3Q13.Enabled = false;
            Q3BR4Q13.Enabled = false;
            Q3BR1Q14.Enabled = false;
            Q3BR2Q14.Enabled = false;
            Q3BR3Q14.Enabled = false;
            Q3BR4Q14.Enabled = false;
            Q3BR1Q15.Enabled = false;
            Q3BR2Q15.Enabled = false;
            Q3BR3Q15.Enabled = false;
            Q3BR4Q15.Enabled = false;
            Q3BR1Q16.Enabled = false;
            Q3BR2Q16.Enabled = false;
            Q3BR3Q16.Enabled = false;
            Q3BR4Q16.Enabled = false;
            Q3BR1Q17.Enabled = false;
            Q3BR2Q17.Enabled = false;
            Q3BR3Q17.Enabled = false;
            Q3BR4Q17.Enabled = false;
            Q3BR1Q18.Enabled = false;
            Q3BR2Q18.Enabled = false;
            Q3BR3Q18.Enabled = false;
            Q3BR4Q18.Enabled = false;
            Q3BR1Q19.Enabled = false;
            Q3BR2Q19.Enabled = false;
            Q3BR3Q19.Enabled = false;
            Q3BR4Q19.Enabled = false;
            Q3BR1Q20.Enabled = false;
            Q3BR2Q20.Enabled = false;
            Q3BR3Q20.Enabled = false;
            Q3BR4Q20.Enabled = false;

            cbox81.Enabled = false;
            cbox82.Enabled = false;
            cbox83.Enabled = false;
            cbox84.Enabled = false;
            cbox85.Enabled = false;
            cbox86.Enabled = false;
            cbox87.Enabled = false;
            cbox88.Enabled = false;
            cbox89.Enabled = false;
            cbox90.Enabled = false;
            cbox91.Enabled = false;
            cbox92.Enabled = false;
            cbox93.Enabled = false;
            cbox94.Enabled = false;
            cbox95.Enabled = false;
            cbox96.Enabled = false;
            cbox97.Enabled = false;
            cbox98.Enabled = false;
            cbox99.Enabled = false;
            cbox100.Enabled = false;
            cbox101.Enabled = false;
            cbox102.Enabled = false;
            cbox103.Enabled = false;
            cbox104.Enabled = false;
            cbox105.Enabled = false;
            cbox106.Enabled = false;
            cbox107.Enabled = false;
            cbox108.Enabled = false;
            cbox109.Enabled = false;
            cbox110.Enabled = false;
            cbox_111.Enabled = false;
            cbox_112.Enabled = false;
            cbox_113.Enabled = false;
            cbox_114.Enabled = false;
            cbox_115.Enabled = false;
            cbox_116.Enabled = false;
            cbox_117.Enabled = false;
            cbox_118.Enabled = false;
            cbox_119.Enabled = false;
            cbox_120.Enabled = false;
            cbox_121.Enabled = false;
            cbox_122.Enabled = false;
            txt_reg1.Enabled = false;
            txt_reg2.Enabled = false;
            txt_7a.Enabled = false;
            txt_7b.Enabled = false;
            txt_7c.Enabled = false;
            txt_8a.Enabled = false;
            txt_8b.Enabled = false;
            btn_Q1_submit.Enabled = false;
            btn_Q2_submit.Enabled = false;
            btn_Q3A_submit.Enabled = false;
            btn_Q3B_submit.Enabled = false;
            btn_Q4_submit.Enabled = false;
            btn_Q5_submit.Enabled = false;
            btn_Q6_submit.Enabled = false;
            btn_Q7_submit.Enabled = false;
            btn_subq8.Enabled = false;
            btnsub.Enabled = false;

        }
        #endregion

        private void Send_Error(Exception ex, string QuestionNo, string Type)
        {
            try
            {
                webervice_it.ErrorLog(lbl_Index.Text, lbl_seat.Text, Common.Get_IP(), ex.ToString(), QuestionNo, Type);
            }
            catch (Exception e)
            {

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

    }

}
