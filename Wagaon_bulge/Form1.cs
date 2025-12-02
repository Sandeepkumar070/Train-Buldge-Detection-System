using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web.UI;
using System.Windows.Forms;

namespace Wagaon_bulge
{
    public partial class Form1 : Form
    {
        SqlConnection conn = new SqlConnection("Data Source=LOCALHOST\\SQLEXPRESS;Initial Catalog=bulge_Data;Integrated Security=True");



        public Form1()
        {
            InitializeComponent();
            guna2Button3.Visible = false;  // user Registration button hide 
        }

        // Loging data cencel // 
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            
            guna2TextBox2.Clear();
            guna2TextBox1.Clear();
        }

        // Admin Selection 
        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (guna2ComboBox1.SelectedIndex == 2) { guna2Button3.Visible = true; } else { guna2Button3.Visible = false; }
            

        }
        // Test Running form open =======================================================================================================
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (guna2ComboBox1.SelectedIndex == 2)
            {
                bool adminloginstatus = adminloging();

                if (adminloginstatus)
                {
                    Form2 f2 = new Form2();
                    f2.ShowDialog();
                } else { MessageBox.Show("Invilid Login"); }
            } 

            if ((guna2ComboBox1.SelectedIndex == 0) ||(guna2ComboBox1.SelectedIndex == 1))
            {
               
               if (loging_verfy(guna2TextBox1.Text, guna2TextBox2.Text))
                {
                    guna2TextBox1.Clear();
                    guna2TextBox2.Clear();
                    Form2 f2 = new Form2();
                    f2.ShowDialog();

                }
               else { MessageBox.Show("User ID and Password are invilid!!!"); }
                     
            }


        }


        // Admin login check //
        private bool adminloging()
        {
            bool login_status = false;
            if (guna2ComboBox1.SelectedIndex == 2) 
            {
                if ((guna2TextBox1.Text == "Admin") && (guna2TextBox2.Text == "Password"))
                {
                    guna2TextBox1.Clear();
                    guna2TextBox2.Clear();
                    login_status = true;

                }
                else {  login_status = false; }
            
            
            }

            return login_status;


        }

        // user id and password chceking logic 
        private bool loging_verfy(string userid, string password)
        {
            bool user_ok = false;   
            SqlCommand cmd = new SqlCommand(" select User_ID from loging where User_ID ='"+userid+"'and Password ='"+password+"'", conn);
            SqlDataAdapter adp = new SqlDataAdapter(cmd);   
            DataTable dt = new DataTable(); 
            adp.Fill(dt);
            if (dt.Rows.Count > 0) { user_ok = true; } else { user_ok = false; }
            return user_ok;
        }











      
        // user Registratuion entry form =============================================================
        private void guna2Button3_Click_1(object sender, EventArgs e)
        {
            if (guna2ComboBox1.SelectedIndex == 2)
            {
                bool adminloginstatus = adminloging();

                if (adminloginstatus)
                {
                    User_Registarioncs user_page = new User_Registarioncs();
                    user_page.ShowDialog();
                }
                else { MessageBox.Show("Invilid Login"); }
            }

        }
    }
}
