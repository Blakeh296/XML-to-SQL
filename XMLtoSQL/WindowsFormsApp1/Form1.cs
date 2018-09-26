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
                if(cbDataBase.Text == applicantsTable.TableName)
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
                else if (cbDataBase.Text == personPerson.TableName)
                {
                    personTblAdapter.Update(personPerson);
                    statusStrip1.BackColor = Color.Green;
                    statuslbl.Text = "Person.Person Updated To Database";
                }
                else if (cbDataBase.Text == emailAddresses.TableName)
                {
                    emailAdapter.Update(emailAddresses);
                    statusStrip1.BackColor = Color.Green;
                    statuslbl.Text = "Person.EmailAddress Updated To Database";
                }
                else if (cbDataBase.Text == stateProvinceTbl.TableName)
                {
                    stateProvinceDataAdapter.Update(stateProvinceTbl);
                    statusStrip1.BackColor = Color.Green;
                    statuslbl.Text = "Person.StateProvince Updated To Database";
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



        private void btnAddBlankRow_Click(object sender, EventArgs e)
        {
            //Create a Person Row from My Customer AdventureWorksDs
            AdventureWorksDs.PersonRow newRow = personPerson.NewPersonRow();
            //Give the NewRow some place holder data
            newRow.FirstName = "PLACE HOLDER";
            newRow.LastName = "PLACE HOLDER";
            //Add a new row to the PersonPerson Tbl, & make it our custom row from above
            personPerson.AddPersonRow(newRow);
            //Refresh the DataGridView
            dgvData.Refresh();
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
            //Add the tables to the ComboBox
            cbDataBase.Items.Add(personPerson.TableName);
            cbDataBase.Items.Add(applicantsTable.TableName);
            cbDataBase.Items.Add(emailAddresses.TableName);
            cbDataBase.Items.Add(stateProvinceTbl.TableName);
            //Set a default Table
            cbDataBase.Text = personPerson.TableName;
        }

        private void cbDataBase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbDataBase.Text == applicantsTable.TableName)
            {
                //Fill the DataAdapter with the Applicants Table
                ApplicantsTblAdapter.Fill(applicantsTable);
                DataTable applicants = applicantsTable;
                dgvData.DataSource = applicantsTable;
            }
            else if (cbDataBase.Text == personPerson.TableName)
            {
                //Fill Our Data Adapter with the Entire Person.Person Table
                personTblAdapter.Fill(personPerson);
                //Declare a dataTable and use Polymorphism to make it 
                //Equal our Person.Person Tbl
                DataTable personTbl = personPerson;
                //Set the DGV Datasource equal to that of our new table
                dgvData.DataSource = personPerson;
            }
            else if (cbDataBase.Text == emailAddresses.TableName)
            {
                emailAdapter.Fill(emailAddresses);
                DataTable EmailTbl = emailAddresses;
                dgvData.DataSource = emailAddresses;
            }
            else if (cbDataBase.Text == stateProvinceTbl.TableName)
            {
                stateProvinceDataAdapter.Fill(stateProvinceTbl);
                DataTable StateProv = stateProvinceTbl;
                dgvData.DataSource = stateProvinceTbl;
            }
        }
    }
}
