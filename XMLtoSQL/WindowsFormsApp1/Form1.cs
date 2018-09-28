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
using WindowsFormsApp1.AdventureWorksDsTableAdapters; //You need this to access DataAdapteres
using WindowsFormsApp1.TestDbTableAdapters;             //You need this to access DataAdapters

namespace WindowsFormsApp1
{
    public partial class form1 : Form
    {
        //Declare My custom DataSet, then the table -- Leave it blank for now
        AdventureWorksDs.PersonDataTable personPerson = new AdventureWorksDs.PersonDataTable();
        //Declare the custom DataAdapter assosiated to the table
        PersonTableAdapter personTblAdapter = new PersonTableAdapter();

        TestDb.ApplicantsDataTable applicantsTable = new TestDb.ApplicantsDataTable();
        ApplicantsTableAdapter ApplicantsTblAdapter = new ApplicantsTableAdapter();

        AdventureWorksDs.EmailAddressDataTable emailAddresses = new AdventureWorksDs.EmailAddressDataTable();
        EmailAddressTableAdapter emailAdapter = new EmailAddressTableAdapter();

        AdventureWorksDs.StateProvinceDataTable stateProvinceTbl = new AdventureWorksDs.StateProvinceDataTable();
        StateProvinceTableAdapter stateProvinceDataAdapter = new StateProvinceTableAdapter();

        AdventureWorksDs.AddressTypeDataTable addressType = new AdventureWorksDs.AddressTypeDataTable();
        AddressTypeTableAdapter AddressTypeDA = new AddressTypeTableAdapter();

        AdventureWorksDs.PersonPhoneDataTable personPhoneTBL = new AdventureWorksDs.PersonPhoneDataTable();
        PersonPhoneTableAdapter personPhoneDA = new PersonPhoneTableAdapter();

        AdventureWorksDs.EmployeeDataTable EmployeeTBL = new AdventureWorksDs.EmployeeDataTable();
        EmployeeTableAdapter EmployeeDA = new EmployeeTableAdapter();

        AdventureWorksDs.DepartmentDataTable DepartmentTBL = new AdventureWorksDs.DepartmentDataTable();
        DepartmentTableAdapter DepartmentDA = new DepartmentTableAdapter();

        AdventureWorksDs.EmployeeDepartmentHistoryDataTable EmployeeDepartmentHistoryTBL = new AdventureWorksDs.EmployeeDepartmentHistoryDataTable();
        EmployeeDepartmentHistoryTableAdapter EmployeeDepartmentHistoryDA = new EmployeeDepartmentHistoryTableAdapter();

        //Declare these to be accessed by entire form
        DataSet dataSet = new DataSet();
        //SqlConnection CANNOT reference a NON STATIC STRING
        static string connString = @"Server=PL11\MTCDEVDB; Database=ImportMockaroo;Trusted_Connection=True";
        //Set the connection == to that of our Static String ^^^
        SqlConnection sqlConn = new SqlConnection(connString);

        public form1()
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
                if(label1.Text == applicantsTable.TableName)
                {
                    if (dgvData.Rows.Count > 0)
                    {
                        sqlConn.Open(); //Open SqlConnection

                        //For Every Row in the DATAGRIDVIEW
                        foreach (DataGridViewRow dr in dgvData.Rows)
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
                           
                        }

                        //Clear for another use
                        sqlCommand.Parameters.Clear();
                        statusStrip1.BackColor = Color.Green;
                        statuslbl.Text = drCount + " Rows pushed to SQL";
                    }
                }
                else if (label1.Text == personPerson.TableName)
                {
                    personTblAdapter.Update(personPerson);
                    statusStrip1.BackColor = Color.Green;
                    statuslbl.Text = "Person.Person Updated To Database";
                }
                else if (label1.Text == emailAddresses.TableName)
                {
                    emailAdapter.Update(emailAddresses);
                    statusStrip1.BackColor = Color.Green;
                    statuslbl.Text = "Person.EmailAddress Updated To Database";
                }
                else if (label1.Text == stateProvinceTbl.TableName)
                {
                    stateProvinceDataAdapter.Update(stateProvinceTbl);
                    statusStrip1.BackColor = Color.Green;
                    statuslbl.Text = "Person.StateProvince Updated To Database";
                }
                else if (label1.Text == addressType.TableName)
                {
                    AddressTypeDA.Update(addressType);
                    statusStrip1.BackColor = Color.Green;
                    statuslbl.Text = "AddressType updated to Database";
                }
                else if (label1.Text == EmployeeTBL.TableName)
                {
                    EmployeeDA.Update(EmployeeTBL);
                    statusStrip1.BackColor = Color.Green;
                    statuslbl.Text = "Employee updated to Database";
                }
                else if (label1.Text == EmployeeDepartmentHistoryTBL.TableName)
                {
                    EmployeeDepartmentHistoryDA.Update(EmployeeDepartmentHistoryTBL);
                    statusStrip1.BackColor = Color.Green;
                    statuslbl.Text = "EmployeeDepartmentHistory Updated to Database";
                }
                else if (label1.Text == personPhoneTBL.TableName)
                {
                    personPhoneDA.Update(personPhoneTBL);
                    statusStrip1.BackColor = Color.Green;
                    statuslbl.Text = "PersonPhone Updated to Database";
                }
            }
            catch (Exception ex)
            {
                //Send error message to status label
                statusStrip1.BackColor = Color.Green;
                statuslbl.Text = drCount + " Rows Pushed To SQL DB";
            }
            finally
            {
                //Close Sql connection
                sqlConn.Close();
            }
        }

        private void form1_Load(object sender, EventArgs e)
        {
            //Fill Our Data Adapter with the Entire Person.Person Table
            personTblAdapter.Fill(personPerson);
            //Declare a dataTable and use Polymorphism to make it 
            //Equal our Person.Person Tbl
            DataTable dfaultTbl = personPerson;
            //Set the DGV Datasource equal to that of our new table
            dgvData.DataSource = personPerson;
        }

        private void dgvData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            textBox1.Text = dgvData.CurrentCell.Value.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            dgvData.CurrentCell.Value = textBox1.Text;
        }

        private void dgvData_CurrentCellChanged(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = dgvData.CurrentCell.Value.ToString();
            }
            catch (Exception ex)
            {
                statuslbl.Text = ex.Message;
            }
        }

        private void applicantsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Fill the DataAdapter with the Applicants Table
            ApplicantsTblAdapter.Fill(applicantsTable);
            DataTable applicants = applicantsTable;
            dgvData.DataSource = applicantsTable;
            string drCount = dgvData.RowCount.ToString();
            statuslbl.Text = drCount.ToString() + " Rows loaded from " + applicantsTable.TableName + " Tbl.";
            label1.Text = applicants.TableName;
        }

        private void personToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Fill Our Data Adapter with the Entire Person.Person Table
            personTblAdapter.Fill(personPerson);
            //Declare a dataTable and use Polymorphism to make it 
            //Equal our Person.Person Tbl
            DataTable personTbl = personPerson;
            //Set the DGV Datasource equal to that of our new table
            dgvData.DataSource = personPerson;
            string drCount = dgvData.RowCount.ToString();
            statuslbl.Text = drCount.ToString() + " Rows loaded from " + personPerson.TableName + " Tbl.";
            label1.Text = personPerson.TableName;
        }

        private void emailAddressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            emailAdapter.Fill(emailAddresses);
            DataTable EmailTbl = emailAddresses;
            dgvData.DataSource = emailAddresses;
            string drCount = dgvData.RowCount.ToString();
            statuslbl.Text = drCount.ToString() + " Rows loaded from " + emailAddresses.TableName + " Tbl.";
            label1.Text = emailAddresses.TableName;
        }

        private void stateProvinceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stateProvinceDataAdapter.Fill(stateProvinceTbl);
            DataTable StateProv = stateProvinceTbl;
            dgvData.DataSource = stateProvinceTbl;
            string drCount = dgvData.RowCount.ToString();
            statuslbl.Text = drCount.ToString() + " Rows loaded from " + stateProvinceTbl.TableName + " Tbl.";
            label1.Text = stateProvinceTbl.TableName;
        }

        private void addressTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddressTypeDA.Fill(addressType);
            DataTable blank = addressType;
            dgvData.DataSource = addressType;
            string drCount = dgvData.RowCount.ToString();
            statuslbl.Text = drCount.ToString() + " Rows loaded from " + addressType.TableName + " Tbl.";
            label1.Text = addressType.TableName;
        }

        private void personPhoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            personPhoneDA.Fill(personPhoneTBL);
            DataTable data = personPhoneTBL;
            dgvData.DataSource = personPhoneTBL;
            string drCount = dgvData.RowCount.ToString();
            statuslbl.Text = drCount.ToString() + " Rows loaded from " + personPhoneTBL.TableName + " Tbl.";
            label1.Text = personPhoneTBL.TableName;
        }

        private void employeeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EmployeeDA.Fill(EmployeeTBL);
            DataTable dataTable = EmployeeTBL;
            dgvData.DataSource = EmployeeTBL;
            string drCount = dgvData.RowCount.ToString();
            statuslbl.Text = drCount.ToString() + " Rows loaded from " + EmployeeTBL.TableName + " Tbl.";
            label1.Text = EmployeeTBL.TableName;
        }

        private void employeeDepartmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DepartmentDA.Fill(DepartmentTBL);
            DataTable datatable = DepartmentTBL;
            dgvData.DataSource = DepartmentTBL;
            string drCount = dgvData.RowCount.ToString();
            statuslbl.Text = drCount.ToString() + " Rows loaded from " + DepartmentTBL.TableName + " Tbl.";
            label1.Text = DepartmentTBL.TableName;
        }

        private void employeeDepartmentHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EmployeeDepartmentHistoryDA.Fill(EmployeeDepartmentHistoryTBL);
            DataTable data = EmployeeDepartmentHistoryTBL;
            dgvData.DataSource = EmployeeDepartmentHistoryTBL;
            string drCount = dgvData.RowCount.ToString();
            statuslbl.Text = drCount.ToString() + " Rows loaded from " + EmployeeDepartmentHistoryTBL.TableName + " Tbl.";
            label1.Text = EmployeeDepartmentHistoryTBL.TableName;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                dgvData.Focus();
            }
        }


    }
}
