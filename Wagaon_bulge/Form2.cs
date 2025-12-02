using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using ActUtlTypeLib;



namespace Wagaon_bulge

{
    public partial class Form2 : Form
    {
        PLC plc_fx5u = new PLC();
       
        
        SqlConnection conn_1 = new SqlConnection("Data Source=LOCALHOST\\SQLEXPRESS;Initial Catalog=bulge_Data;Integrated Security=True");

        
        
        // Variable declearation 

        int LH_S_UP, LH_S_DOWN, RH_S_UP, RH_S_DOWN,bogie_counter,count_sensor;
        int bulge_image_log_trigger;
        int lhs_bulge_count=0, rhs_bulge_count=0, total_bulge_count = 0;
        public Form2()
        {
            InitializeComponent();
            
        }

       


        // inatia Program logic at time of application load ===========================================
        private void Form2_Load(object sender, EventArgs e)
        {
            Scan_timer.Enabled = true; 
            Scan_timer.Interval = 200;
            
            guna2TextBox6.Text = plc_fx5u.PLc_communication(); // PLC Communication Test 
            
            if (guna2TextBox6.Text == "Connected") { Scan_timer.Start(); } else { Scan_timer.Stop(); }
        }

        // Loop Program of application ===============================================================
        private void Scan_timer_Tick(object sender, EventArgs e)
        {
            guna2TextBox9.Text = DateTime.Now.ToString("dd-MM-yyyy / HH:mm:ss");

            Sensor_Status(); // Read Sesnor status from PLC 

            if ((bogie_counter==1)&&(Test_No_logged==false) && (test_no_generated == false))
            {
                Test_history_loggging();
                lhs_bulge_count = 0;
                rhs_bulge_count = 0;
                total_bulge_count = 0;
            } 

            if (bogie_counter == 0)
            {
                Test_No_logged = false;
                test_no_generated = false;
               

            }

            if (bogie_counter >= 2)
            {
                bulge_image_log_trigger = plc_fx5u.image_resultlog_read();
                if (bulge_image_log_trigger==1) 
                { bulge_result_logging(); }
                total_bulge_count = lhs_bulge_count + rhs_bulge_count;

            }


            guna2TextBox15.Text = lhs_bulge_count.ToString();
            guna2TextBox16.Text = rhs_bulge_count.ToString();
            guna2TextBox17.Text = total_bulge_count.ToString();
            guna2TextBox12.Text = guna2TextBox5.Text;
        }


        // Read Sesnor status from PLC ================================================================================================================================================
        private void Sensor_Status()
        {

            LH_S_UP = plc_fx5u.LH_Sensor_1();
            LH_S_DOWN = plc_fx5u.LH_Sensor_2();
            RH_S_UP = plc_fx5u.RH_Sensor_1();
            RH_S_DOWN = plc_fx5u.RH_Sensor_2();
            bogie_counter = plc_fx5u.boggie_no_read();
            count_sensor = plc_fx5u.Count_Sensor_1();

            if (LH_S_UP == 1) { guna2TextBox1.FillColor = Color.GreenYellow; guna2TextBox1.Text = "ON"; } else { guna2TextBox1.FillColor = Color.Red; guna2TextBox1.Text = "OFF"; }
            if (LH_S_DOWN == 1) { guna2TextBox2.FillColor = Color.GreenYellow; guna2TextBox2.Text = "ON"; } else { guna2TextBox2.FillColor = Color.Red; guna2TextBox2.Text = "OFF"; }
            if (RH_S_UP == 1) { guna2TextBox3.FillColor = Color.GreenYellow; guna2TextBox3.Text = "ON"; } else { guna2TextBox3.FillColor = Color.Red; guna2TextBox3.Text = "OFF"; }
            if (RH_S_DOWN == 1) { guna2TextBox4.FillColor = Color.GreenYellow; guna2TextBox4.Text = "ON"; } else { guna2TextBox4.FillColor = Color.Red; guna2TextBox4.Text = "OFF"; }
            if (count_sensor == 1) { guna2TextBox19.FillColor = Color.GreenYellow; guna2TextBox19.Text = "ON"; } else { guna2TextBox19.FillColor = Color.Red; guna2TextBox19.Text = "OFF"; }
            guna2TextBox13.Text = bogie_counter.ToString(); // Boggi counter actual value 

           if (bogie_counter==1) { guna2TextBox14.Text = "Engine"; }
           if (bogie_counter >= 2) { int bogie_1_count = bogie_counter-1; guna2TextBox14.Text = "Wagon No. "+ bogie_1_count.ToString() + ""; }




        }

      
        // Get LHS Side Image from FTP Location ======================================
        string filepath_lhs = "";
        private void getimage_LHS()
        {
            generateimage_path_lhs(); 
            Bitmap bmp_lhs = new Bitmap(filepath_lhs);
            guna2PictureBox1.Image = bmp_lhs;
            guna2PictureBox1.SizeMode=PictureBoxSizeMode.StretchImage;
        }

        // Get RHS Side Image from FTP Location ======================================
        string filepath_rhs = "";
        private void getimage_RHS()
        {
            generateimage_path_rhs();
            Bitmap bmp_rhs = new Bitmap(filepath_rhs);
            guna2PictureBox2.Image = bmp_rhs;
            guna2PictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        // generate image path left side  =========================================================
        private void generateimage_path_lhs()
        {   


            filepath_lhs = "C:\\ftp file\\CAMERA1\\" + guna2TextBox10.Text + "" ;
        }

        //  generate image path right side ============================================

        private void generateimage_path_rhs()
        {


            filepath_rhs = "C:\\ftp file\\CAMERA2\\" + guna2TextBox11.Text + "";
        }



        // get latest image from FTP ( right side camera2 image ) =================================

        String lastLH_file_addres = "";
        String lastRH_file_addres = "";
        private void get_latest_image_RH()
        {
            
            try
            {
                var directory = new DirectoryInfo("C:\\ftp file\\CAMERA2");
                var myFile = (from f in directory.GetFiles()
                              orderby f.LastWriteTime descending
                              select f).First();

                guna2TextBox11.Text = myFile.Name;
                lastRH_file_addres = guna2TextBox11.Text;
            }
            catch { guna2TextBox11.Text = lastRH_file_addres; }                                 // Right Side latest image address 
        }

        // get latest image from FTP ( Left side camera1 image ) ===================================

            private void get_latest_image_LH()
        {
            try
            {
                var directory = new DirectoryInfo("C:\\ftp file\\CAMERA1");
                var myFile = (from f in directory.GetFiles()
                              orderby f.LastWriteTime descending
                              select f).First();

                guna2TextBox10.Text = myFile.Name;
                lastLH_file_addres = guna2TextBox10.Text;
            } catch { guna2TextBox10.Text = lastLH_file_addres; }


            //int index = myFile.Name.LastIndexOf("_AlarmIn-1");
            //if (index >= 0)
            //{



            //    guna2TextBox1.Text = guna2TextBox10.Text.Substring(23, 6);
            //}
            //guna2TextBox2.Text = DateTime.Now.ToString("HHmmss");
        }


        // Report Generate Page 
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            // bulge_image_LH_log();

            
            
                Form3 f3 = new Form3();
                f3.ShowDialog();  
            
        }


        // Auto test no generated logic ========================================================
        bool test_no_generated = false;
        private string testno_generate()
        {
            string test_no_final = "";
            //if (bogie_counter ==0)
            //{ test_no_generated = false; }


           
            if (test_no_generated == false)
            {
                int test_count = plc_fx5u.test_No();
                string dateime = DateTime.Now.ToString("yyyyMMddHHmm");
                test_no_final = dateime + test_count.ToString();
                test_no_generated = true;
            }



            return test_no_final;
        }
        // Test No Logging ========================================================
        bool Test_No_logged =false;

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            try
            {
                List<String> Data_Site_lacation = new List<String>();
                Data_Site_lacation.Add(guna2TextBox7.Text);
                string File_Path = "C:\\Extratech\\Wagaon_bulge\\Site_Location.txt";
                File.WriteAllLines(File_Path, Data_Site_lacation);
                MessageBox.Show("Site Location saved.");
            }
            catch { MessageBox.Show("Site Location data Save Error!!!"); }
        }
        private bool Test_history_loggging()
        {
            
            string logging_date = DateTime.Now.ToString("dd-MM-yyyy");
            string test_start_time = DateTime.Now.ToString("HH:mm:ss");
            string test_no = testno_generate();
            guna2TextBox5.Text = test_no;
            guna2TextBox18.Text = test_start_time;
            try
            {
                SqlCommand cmd = new SqlCommand("insert into Test_No_Logging (Test_No,Test_Date,Test_Start_Time ) values ('"+ test_no + "','"+logging_date+"','"+test_start_time+"')",conn_1);
                conn_1.Open();
                cmd.ExecuteNonQuery();
                conn_1.Close();
                Test_No_logged = true; 
                
            }catch { conn_1.Close(); Test_No_logged = false; }

            return Test_No_logged;

        }
        
        // LH bulge image logging =================================================
        private void bulge_image_LH_log()
        {

            string test_result_LH = "NG";
            string test_result_RH = "OK";
            get_latest_image_LH(); // fatch image from FTP 
            getimage_LHS(); // Display Image in Picture Box LH 
            byte[] imageLH_binary = ImageToStream(filepath_lhs); // Convert Image into Binary 
            byte[] imageRH_binary = ImageToStream("C:\\ftp file\\OK_Image\\NO_IMAGE.png");
            Bitmap bmp_rhs = new Bitmap("C:\\ftp file\\OK_Image\\NO_IMAGE.png");
            guna2PictureBox2.Image = bmp_rhs;
            guna2PictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            lhs_bulge_count = lhs_bulge_count + 1;



            try
            {
                SqlCommand cmd = new SqlCommand("insert into Bulge_Report_tbl (Test_No,Wegon_No,Result_LH,Image_LH,Result_RH,Image_RH) values ('" + guna2TextBox5.Text + "','" + guna2TextBox14.Text + "','" + test_result_LH + "',@LH_Image,'"+ test_result_RH + "',@RH_Image)", conn_1);
                cmd.Parameters.AddWithValue("@LH_Image", imageLH_binary);
                cmd.Parameters.AddWithValue("@RH_Image", imageRH_binary);
                conn_1.Open() ;
                cmd.ExecuteNonQuery();
                conn_1.Close() ;

            } catch (Exception e) { conn_1.Close(); MessageBox.Show(e.Message); }




        }

        // RH bulge image logging =================================================
        private void bulge_image_RH_log()
        {

            string test_result_LH = "OK";
            string test_result_RH = "NG";
            get_latest_image_RH(); // fatch image from FTP - Camera2
            getimage_RHS(); // Display Image in Picture Box RH 
            byte[] imageLH_binary = ImageToStream("C:\\ftp file\\OK_Image\\NO_IMAGE.png");
            Bitmap bmp_lhs = new Bitmap("C:\\ftp file\\OK_Image\\NO_IMAGE.png");
            guna2PictureBox1.Image = bmp_lhs;
            guna2PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            rhs_bulge_count = rhs_bulge_count + 1;

            byte[] imageRH_binary = ImageToStream(filepath_rhs); // Convert Image into Binary 
            try
            {
                SqlCommand cmd = new SqlCommand("insert into Bulge_Report_tbl (Test_No,Wegon_No,Result_LH,Image_LH,Result_RH,Image_RH) values ('" + guna2TextBox5.Text + "','" + guna2TextBox14.Text + "','" + test_result_LH + "',@LH_Image,'" + test_result_RH + "',@RH_Image)", conn_1);
                cmd.Parameters.AddWithValue("@LH_Image", imageLH_binary);
                cmd.Parameters.AddWithValue("@RH_Image", imageRH_binary);
                conn_1.Open();
                cmd.ExecuteNonQuery();
                conn_1.Close();

            }
            catch (Exception e) { conn_1.Close(); MessageBox.Show(e.Message); }




        }
        // LH and RH both side bulge detected ============================================================
        private void bulge_image_LH_RH_log()
        {

            string test_result_LH = "NG";
            string test_result_RH = "NG";
            get_latest_image_RH(); // fatch image from FTP - Camera2
            get_latest_image_LH(); // fatch image from FTP - camera1
            getimage_RHS(); // Display Image in Picture Box RH 
            getimage_LHS();  // Display Image in Picture box LH
            byte[] imageLH_binary = ImageToStream(filepath_lhs); // Convert Image into Binary 
            byte[] imageRH_binary = ImageToStream(filepath_rhs); // Convert Image into Binary 
            lhs_bulge_count = lhs_bulge_count + 1;
            rhs_bulge_count = rhs_bulge_count + 1;

            try
            {
                SqlCommand cmd = new SqlCommand("insert into Bulge_Report_tbl (Test_No,Wegon_No,Result_LH,Image_LH,Result_RH,Image_RH) values ('" + guna2TextBox5.Text + "','"+ guna2TextBox14.Text + "','" + test_result_LH + "',@LH_Image,'" + test_result_RH + "',@RH_Image)", conn_1);
                cmd.Parameters.AddWithValue("@LH_Image", imageLH_binary);
                cmd.Parameters.AddWithValue("@RH_Image", imageRH_binary);
                conn_1.Open();
                cmd.ExecuteNonQuery();
                conn_1.Close();

            }
            catch (Exception e) { conn_1.Close(); MessageBox.Show(e.Message); }




        }
        // No bulge detected both side ==============================================================
        private void bulge_image_OK_LH_RH_log()
        {

            string test_result_LH = "OK";
            string test_result_RH = "OK";
            
            byte[] imageLH_binary = ImageToStream("C:\\ftp file\\OK_Image\\NO_IMAGE.png"); // Convert Image into Binary 
            Bitmap bmp_lhs = new Bitmap("C:\\ftp file\\OK_Image\\NO_IMAGE.png");
            guna2PictureBox1.Image = bmp_lhs;
            guna2PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            guna2PictureBox2.Image = bmp_lhs;
            guna2PictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;



            try
            {
                SqlCommand cmd = new SqlCommand("insert into Bulge_Report_tbl (Test_No,Wegon_No,Result_LH,Image_LH,Result_RH,Image_RH) values ('" + guna2TextBox5.Text + "','" + guna2TextBox14.Text + "','" + test_result_LH + "',@LH_Image,'" + test_result_RH + "',@RH_Image)", conn_1);
                cmd.Parameters.AddWithValue("@LH_Image", imageLH_binary);
                cmd.Parameters.AddWithValue("@RH_Image", imageLH_binary);
                conn_1.Open();
                cmd.ExecuteNonQuery();
                conn_1.Close();

            }
            catch (Exception e) { conn_1.Close(); MessageBox.Show(e.Message); }




        }
        // Finally Bulge image data logging ====================================================
        private void bulge_result_logging()
        {
            int LHS_bulge = plc_fx5u.LH_image_Trigger();
            int RHS_bulge = plc_fx5u.RH_image_Trigger();
           

            if ((bulge_image_log_trigger == 1)&&(LHS_bulge==0)&& (RHS_bulge == 0))
            {
                bulge_image_OK_LH_RH_log();
               
            }
            if ((bulge_image_log_trigger == 1) && (LHS_bulge == 1) && (RHS_bulge == 0))
            {
                bulge_image_LH_log();


            }

            if ((bulge_image_log_trigger == 1) && (LHS_bulge == 0) && (RHS_bulge == 1))
            {
                bulge_image_RH_log();
            }

            if ((bulge_image_log_trigger == 1) && (LHS_bulge == 1) && (RHS_bulge == 1))
            {
                bulge_image_LH_RH_log();
            }  

            plc_fx5u.reset_all_data();

        }














        // Convert image into binary file ========================================
        private byte[] ImageToStream(string fileName)
        {
            MemoryStream stream = new MemoryStream();
        tryagain:
            try
            {
                Bitmap image = new Bitmap(fileName);
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            catch 
            {
                goto tryagain;
            }
            return stream.ToArray();
        }





    }

}
