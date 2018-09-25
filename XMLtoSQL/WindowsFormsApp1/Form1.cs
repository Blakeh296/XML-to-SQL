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

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        //Declare these to be accessed by entire form
        DataSet dataSet = new DataSet();
        //SqlConnection CANNOT reference a NON STATIC STRING
        static string connString = @"Server=PL11\MTCDEVDB; Database=ImportMockaroo;Trusted_Connection=True";
        //Set the connection == to that of our Static String ^^^
        SqlConnection sqlConn = new SqlConnection(connString);

        public Form1()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (ofdXML.ShowDialog() == DialogResult.OK)
                {
                    dataSet.ReadXml(ofdXML.FileName);
                    dataSet.Tables[0].Columns.Add("id");
                    dgvData.DataSource = dataSet.Tables[0];
                }
            }
            catch (Exception ex)
            {
                statusStrip1.BackColor = Color.Red;
                statuslbl.Text = "Error.. :" + ex.Message;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string drCount = dgvData.RowCount.ToString();
            statuslbl.ForeColor = Color.Black;
            statuslbl.ForeColor = Color.FromName("Control");
            statuslbl.Text = "Starting SQL Push";
            //SqlCommand gets the name of the Sql Stored Procedure & the Connection to SQL
            SqlCommand sqlCommand = new SqlCommand("InsertApplicant",sqlConn);
            //The Command type is StoredProcedure
            sqlCommand.CommandType = CommandType.StoredProcedure;

            try
            {
                if(dgvData.Rows.Count > 0)
                {
                    sqlConn.Open(); //Open SqlConnection

                    //For Every Row in the DATAGRIDVIEW
                    foreach(DataGridViewRow dr in dgvData.Rows)
                    {
                        //Set Sql Command parameters with the SQL table and the DGV
                        sqlCommand.Parameters.AddWithValue("@FirstName", dr.Cells["first_name"].Value);
                        sqlCommand.Parameters.AddWithValue("@LastName", dr.Cells["last_name"].Value);
                        sqlCommand.Parameters.AddWithValue("@SSN", dr.Cells["ssn"].Value);
                        sqlCommand.Parameters.AddWithValue("@Email", dr.Cells["email"].Value);
                        sqlCommand.Parameters.AddWithValue("@Gender", dr.Cells["gender"].Value);
                        sqlCommand.Parameters.AddWithValue("@AppID", 0);
                        sqlCommand.Parameters["@AppID"].Direction = ParameterDirection.Output;
                        sqlCommand.ExecuteNonQuery();
                        dr.Cells["id"].Value = sqlCommand.Parameters["@AppID"].Value;
                        //Clear for another use
                        sqlCommand.Parameters.Clear();
                        statusStrip1.BackColor = Color.Green;
                        statuslbl.Text = dr.Cells.Count.ToString() + " Rows pushed to SQL";
                    }
                }
            }
            catch (Exception ex)
            {
                //Send error message to status label
                statusStrip1.BackColor = Color.Green;
                statuslbl.Text = drCount + " Rows pushed to SQL";
            }
            finally
            {
                //Close Sql connection
                sqlConn.Close();
            }
        }
    }
}
