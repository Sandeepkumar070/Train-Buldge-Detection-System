using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Wagaon_bulge
{
    public partial class Form3 : Form
    {
        SqlConnection con_report = new SqlConnection("Data Source=LOCALHOST\\SQLEXPRESS;Initial Catalog=bulge_Data;Integrated Security=True");

        // Report parameters Variable 
        String Test_No, Test_Date, Test_Time, Site_Location, Report_Generated_by;
        int LH_bulge, RH_bulge, total_bulge, Total_checked, total_bulge_both;

        private void reportViewer1_Load(object sender, EventArgs e)
        {

        }

        public Form3()
        {
            InitializeComponent();
        }


        // get all test no from test log database =========================================
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string date_selected = guna2DateTimePicker1.Text;
            if (date_selected != null && date_selected.Length > 0)
            {

                try
                {

                    // DT1.Rows.Clear();

                    con_report.Open();
                    SqlCommand cmd_1 = new SqlCommand("SELECT Test_No FROM Test_No_Logging where Test_Date = '" + date_selected + "'", con_report);
                    SqlDataReader rd = cmd_1.ExecuteReader();

                    guna2ComboBox1.Items.Clear();
                    while (rd.Read())
                    {
                        guna2ComboBox1.Items.Add(rd["Test_No"].ToString());
                    }

                    if (guna2ComboBox1.Items.Count == 0) { MessageBox.Show("Test Record Not Found "); }

                    con_report.Close();

                }
                catch
                {

                    con_report.Close(); MessageBox.Show(" Error ");
                }

            }
            else { MessageBox.Show("Please select the Test Date to Load the Test No"); }



        }

        private void Form3_Load(object sender, EventArgs e)
        {
            String Data_Filepath = "C:\\Extratech\\Wagaon_bulge\\Site_Location.txt";
            String[] Site_Location_Read = File.ReadAllLines(Data_Filepath);
            guna2TextBox2.Text = Site_Location_Read[0];
            guna2DateTimePicker1.Value = DateTime.Today;
            // this.reportViewer1.RefreshReport();
        }
        // Report generate logic ===============================================================
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            /// <summary>
            /// This method gets the parameters value and adds them to the ReportParameterCollection. It then sets the parameters to the reportViewer1 and refreshes the report.
            /// </summary>
            get_parametes_value();
            get_test_result_summary();
            ReportParameterCollection rp = new ReportParameterCollection();
            rp.Add(new ReportParameter("Site_Location", guna2TextBox2.Text));
            rp.Add(new ReportParameter("Report_Generated_by", guna2TextBox1.Text));
            rp.Add(new ReportParameter("Test_No", Test_No));
            rp.Add(new ReportParameter("Test_Date", Test_Date));
            rp.Add(new ReportParameter("LH_bulge", LH_bulge.ToString()));
            rp.Add(new ReportParameter("RH_Bulge", RH_bulge.ToString()));
            rp.Add(new ReportParameter("Total_Bulge", total_bulge.ToString()));
            rp.Add(new ReportParameter("Total_Checked", Total_checked.ToString()));
            rp.Add(new ReportParameter("Test_Time", Test_Time));
            this.reportViewer1.LocalReport.SetParameters(rp);
            this.reportViewer1.ProcessingMode = ProcessingMode.Local;
            reportViewer1.LocalReport.DataSources.Clear();
            get_bulge_data();
            this.reportViewer1.RefreshReport();
        }

        // get_ parameters_Value ==========================================================

        private void get_parametes_value()
        {            //Create a SqlCommand object to execute a query on the database
            SqlCommand cmd = new SqlCommand("select * from Test_No_Logging where Test_No ='"+ guna2ComboBox1.SelectedItem.ToString() + "'", con_report);

            try
            {
                //Create a SqlDataAdapter object to fill a DataTable with the results of the query
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                con_report.Close();
                //Check if the DataTable contains any rows
                if (dt.Rows.Count > 0)
                {
                    //If so, assign the values of the columns to the corresponding variables
                    Test_No = dt.Rows[0]["Test_No"].ToString();
                    Test_Date = dt.Rows[0]["Test_Date"].ToString();
                    Test_Time = dt.Rows[0]["Test_Start_Time"].ToString();

                }

            }
            //Catch any errors and display a message
            catch { con_report.Close(); MessageBox.Show("Test Parameters read error"); }

        }

        private void get_bulge_data()
        {
            SqlCommand cmd = new SqlCommand(" select * from Bulge_Report_tbl where Test_No ='"+ guna2ComboBox1.SelectedItem.ToString() + "'", con_report);
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            con_report.Close();
            if (dt.Rows.Count > 0)
            {
                //If so, assign the values of the columns to the corresponding variables
                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSet_Report", dt));

            }

        }

        private void get_test_result_summary()
        {
            LH_bulge = 0;
            RH_bulge = 0;
            total_bulge = 0;
            Total_checked = 0;
            total_bulge_both = 0;   
            try
            {
                SqlCommand cmd = new SqlCommand("select count(*) from Bulge_Report_tbl where Test_No ='" + guna2ComboBox1.SelectedItem.ToString() + "'", con_report);
                con_report.Open();
                Total_checked = (int)cmd.ExecuteScalar();
                con_report.Close();
            }
            catch { Total_checked = 0; con_report.Close(); }

            try
            {
                SqlCommand cmd = new SqlCommand("select count(*) from Bulge_Report_tbl where Test_No ='" + guna2ComboBox1.SelectedItem.ToString() + "'and Result_LH ='NG'", con_report);
                con_report.Open();
                LH_bulge = (int)cmd.ExecuteScalar();
                con_report.Close();
            }
            catch { LH_bulge = 0; con_report.Close(); }

            try
            {
                SqlCommand cmd = new SqlCommand("select count(*) from Bulge_Report_tbl where Test_No ='" + guna2ComboBox1.SelectedItem.ToString() + "'and Result_RH ='NG'", con_report);
                con_report.Open();
                RH_bulge = (int)cmd.ExecuteScalar();
                con_report.Close();
            }
            catch { RH_bulge = 0; con_report.Close(); }

            try
            {
                SqlCommand cmd = new SqlCommand("select count(*) from Bulge_Report_tbl where Test_No ='" + guna2ComboBox1.SelectedItem.ToString() + "'and Result_LH ='NG'and Result_RH ='NG' ", con_report);
                con_report.Open();
                total_bulge_both = (int)cmd.ExecuteScalar();
                con_report.Close();
            }
            catch { RH_bulge = 0; con_report.Close(); }



            total_bulge = LH_bulge + RH_bulge - total_bulge_both;
        }







    }
}
