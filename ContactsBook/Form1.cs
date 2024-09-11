using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContactsBook
{
    
    public partial class Phonebook : Form
    {
        SqlConnection conner;
        SqlCommand cmd;
        SqlDataReader reader;


        DataTable contacts = new DataTable();
        bool editing = false;
        string stringConnection = "Server=ACER;Database=curvin;Trusted_Connection=True;";
        public Phonebook()
        {
            InitializeComponent();
        }

        private void firstNameLabel_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void LoadContactsFromDatabase()
        {
            using (SqlConnection con = new SqlConnection(stringConnection))
            {
                try
                {
                    con.Open();
                    string query = "SELECT name, ContactID, phoneNumber, email FROM curvinPhoneBookTable";  // Replace "Contacts" with your actual table name
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    contacts.Clear();

                    while (reader.Read())
                    {
                        contacts.Rows.Add(reader["Name"], reader["ContactID"], reader["phoneNumber"], reader["Email"]);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading data: " + ex.Message);
                }
            }
        }

        private void Phonebook_Load(object sender, EventArgs e)
        {
            contacts.Columns.Add("Name");
            contacts.Columns.Add("Contact ID");
            contacts.Columns.Add("Phone");
            contacts.Columns.Add("Email");

            LoadContactsFromDatabase();

            // Set data source
            contactsDataGrid.DataSource = contacts;
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            nameTextBox.Text = "";
            contactIdTextBox.Text = "";
            phoneNumTextBox.Text = "";
            emailTextBox.Text = "";

        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            LoadContactsFromDatabase();
        }


        private void editButton_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(stringConnection))
            {
                try
                {
                    con.Open();
                    string query;

                    if (editing) // Update existing record
                    {
                        query = "UPDATE curvinPhoneBookTable SET name = @Name, phoneNumber = @Phone, email = @Email WHERE ContactID = @ContactID";
                    }
                    else // Insert new record
                    {
                        query = "INSERT INTO curvinPhoneBookTable (name, ContactID, phoneNumber, email) VALUES (@Name, @ContactID, @Phone, @Email)";
                    }

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Name", nameTextBox.Text);
                    cmd.Parameters.AddWithValue("@ContactID", contactIdTextBox.Text);
                    cmd.Parameters.AddWithValue("@Phone", phoneNumTextBox.Text);
                    cmd.Parameters.AddWithValue("@Email", emailTextBox.Text);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show(editing ? "Contact updated successfully." : "New contact added successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving data: " + ex.Message);
                }
            }

            // Update the grid
            if (editing)
            {
                contacts.Rows[contactsDataGrid.CurrentCell.RowIndex]["Name"] = nameTextBox.Text;
                contacts.Rows[contactsDataGrid.CurrentCell.RowIndex]["Contact ID"] = contactIdTextBox.Text;
                contacts.Rows[contactsDataGrid.CurrentCell.RowIndex]["Phone"] = phoneNumTextBox.Text;
                contacts.Rows[contactsDataGrid.CurrentCell.RowIndex]["Email"] = emailTextBox.Text;
            }
            else
            {
                contacts.Rows.Add(nameTextBox.Text, contactIdTextBox.Text, phoneNumTextBox.Text, emailTextBox.Text);
            }

            // Clear form fields
            nameTextBox.Text = "";
            contactIdTextBox.Text = "";
            phoneNumTextBox.Text = "";
            emailTextBox.Text = "";
            editing = false;
        }


        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (contactsDataGrid.CurrentCell != null)
            {
                string contactID = contacts.Rows[contactsDataGrid.CurrentCell.RowIndex]["Contact ID"].ToString();

                using (SqlConnection con = new SqlConnection(stringConnection))
                {
                    try
                    {
                        con.Open();
                        string query = "DELETE FROM curvinPhoneBookTable WHERE ContactID = @ContactID";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@ContactID", contactID);
                        cmd.ExecuteNonQuery();

                        // Remove from DataGridView
                        contacts.Rows[contactsDataGrid.CurrentCell.RowIndex].Delete();

                        MessageBox.Show("Contact deleted successfully.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting contact: " + ex.Message);
                    }
                }
            }
        }



        private void contactsDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            nameTextBox.Text = contacts.Rows[contactsDataGrid.CurrentCell.RowIndex].ItemArray[0].ToString();
            contactIdTextBox.Text = contacts.Rows[contactsDataGrid.CurrentCell.RowIndex].ItemArray[1].ToString();
            phoneNumTextBox.Text = contacts.Rows[contactsDataGrid.CurrentCell.RowIndex].ItemArray[2].ToString();
            emailTextBox.Text = contacts.Rows[contactsDataGrid.CurrentCell.RowIndex].ItemArray[3].ToString();
            editing = true;
        }

        private void searchTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void emailTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void contactsDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            string search = searchTextBox.Text;

            using (SqlConnection con = new SqlConnection(stringConnection))
            {
                try
                {
                    con.Open();
                    // Use a parameterized query to prevent SQL injection
                    string query = "SELECT * FROM curvinPhoneBookTable WHERE phoneNumber = @PhoneNumber";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@PhoneNumber", search); // Add the parameter for the search query

                    SqlDataReader reader = cmd.ExecuteReader();

                    // Clear the DataGridView to avoid showing old data
                    contacts.Clear();

                    if (reader.Read()) // If a record is found
                    {
                        // Populate the DataGridView (optional)
                        contacts.Rows.Add(reader["Name"], reader["ContactID"], reader["phoneNumber"], reader["Email"]);

                        // Load data into text boxes
                        nameTextBox.Text = reader["Name"].ToString();
                        contactIdTextBox.Text = reader["ContactID"].ToString();
                        phoneNumTextBox.Text = reader["phoneNumber"].ToString();
                        emailTextBox.Text = reader["Email"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("No record found.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading data: " + ex.Message);
                }
            }
        }

    }
}
