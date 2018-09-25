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
        DataSet dataSet = new DataSet();
        //TODO:Set up connection to server
        SqlConnection sqlConn = new SqlConnection("");

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
                statuslbl.Text = "Error.. :" + ex.Message;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //TODO:THIS
            SqlCommand sqlCommand = new SqlCommand("", );
            sqlCommand.CommandType = CommandType.StoredProcedure;

            //TODO:THIS
            try
            {
                if(dgvData.Rows.Count > 0)
                {
                    sqlConn.Open();

                    //TODO:THIS
                    foreach(DataGridViewRow dr in dgvData.Rows)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                statuslbl.Text = "Error.. :" + ex.Message;
            }
            finally
            {
                sqlConn.Close();
            }
        }
    }
}
