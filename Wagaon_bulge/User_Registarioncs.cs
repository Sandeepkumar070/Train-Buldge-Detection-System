using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Wagaon_bulge
{
    public partial class User_Registarioncs : Form
    {
        public User_Registarioncs()
        {
            InitializeComponent();
        }

        SqlConnection conn = new SqlConnection("Data Source=LOCALHOST\\SQLEXPRESS;Initial Catalog=bulge_Data;Integrated Security=True");



        private void User_Registarioncs_Load(object sender, EventArgs e)
        {
            userid_display();
        }


        // Dispay Registor User ID ================================================
        private void userid_display()
        {
            try { 
            conn.Open();
            SqlCommand cmd = new SqlCommand("Select * from loging", conn);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);   
            DataTable dataTable = new DataTable();
            conn.Close();
            adapter.Fill(dataTable);
            guna2DataGridView1.DataSource = dataTable;
            }
            catch { conn.Close(); MessageBox.Show("User ID data read error!!!! ");  }
        }


        private void adduser()
        {



            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("insert into loging ( User_ID,Password,Type) values ('"+ guna2TextBox1.Text+ "','"+ guna2TextBox2.Text + "','"+ guna2ComboBox1.SelectedItem.ToString() + "')", conn);
                cmd.ExecuteNonQuery();
                conn.Close();
                MessageBox.Show(" User add Succesfully");

            } catch(Exception ex) { conn.Close(); MessageBox.Show(ex.Message);}


        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if ((guna2TextBox1.TextLength >=1) && (guna2TextBox2.TextLength >= 1) && (guna2ComboBox1.SelectedIndex>= 1)) 
            {
                adduser();
                userid_display();
                guna2TextBox1.Clear();
                guna2TextBox2.Clear();


            }
            else { MessageBox.Show(" Please Fill the User ID , Password & Lavel Properly "); userid_display(); }
        }
    }
}
