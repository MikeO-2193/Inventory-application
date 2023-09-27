using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Configuration;
using System.Text.RegularExpressions;

namespace Inventory_appliaction
{
    public partial class frmInventory : Form
    {

        //global variables to store current item and current item Id counter
        int currentitemcounter = 1;
        Item current_item;
        //regex to allow only numeric from textbox
        private static readonly Regex NumbOnly = new Regex("^[-+]?[0-9]*\\.?[0-9]+$");

        //sql variables for loading the database
        string Connstr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Items.mdf;Integrated Security=True";
        SqlConnection Mysqlcon;
        SqlCommand mysqlcomm;


        public frmInventory()
        {
            InitializeComponent();
        }

        //load the first item in the database when the program starts
        private void frmInventory_Load(object sender, EventArgs e)
        {
            current_item = Finditem(currentitemcounter);
            displayitem(current_item);
            txtSearch.Text = currentitemcounter.ToString();


        }

        //function to find a item based on passed item id
        Item Finditem(int item_ID)
        {
            //store the connection string in a string variable
            //create a SQL connection with the string variable
            Mysqlcon = new SqlConnection(Connstr);
            //create a temp item to fill with found data from the database
            Item matchingitem = new Item();

            //open the connection variable
            using (Mysqlcon)
            {
                //crate a string with the sql command and pass it to the sqlcommand
                string sql_stat = "Select * From Items where Id=@item_ID";
                mysqlcomm = new SqlCommand(sql_stat, Mysqlcon);
                mysqlcomm.Parameters.AddWithValue("@item_ID", item_ID);
                Mysqlcon.Open();
                //open the reader and excute the command
                using (SqlDataReader reader = mysqlcomm.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //update the item variable with infomormation in the sql stream
                        matchingitem.ID = Convert.ToInt32(reader["Id"]);
                        matchingitem.Item_Name = reader["Itemname"].ToString();
                        matchingitem.Item_Amount = reader["ItemAmount"].ToString();
                        matchingitem.Item_cost = reader["ItemCost"].ToString();
                        matchingitem.Item_Price = reader["ItemPrice"].ToString();
                        matchingitem.Item_description = reader["ItemDescription"].ToString();

                    }
                    //close the connection
                    Mysqlcon.Close();
                }

            }
            //return the item
            return matchingitem;
        }

        //function to check if a location exsists in the database by number passed
        bool CheckforId(int item_ID)
        {
            //crate the connection temp item and string for database access
            Mysqlcon = new SqlConnection(Connstr);
            Item matchingitem = new Item();
            string sql_stat = "SELECT COUNT(*) from items where Id like @item_ID";
            //open the connection and determine if the item location exiists in the database
            using (mysqlcomm = new SqlCommand(sql_stat, Mysqlcon))
            {
                Mysqlcon.Open();
                mysqlcomm.Parameters.AddWithValue("@item_ID", item_ID);
                int IdCount = (int)mysqlcomm.ExecuteScalar();
                //if the location exists return true otherwise return false
                if (IdCount > 0)
                {
                    Mysqlcon.Close();
                    return true;
                }
                else
                {
                    Mysqlcon.Close();
                    return false;
                }


            }

        }

        //function to display the current item in the text boxes
        private void displayitem(Item passeditem)
        {
            txtID.Text = passeditem.ID.ToString();
            txtItemName.Text = passeditem.Item_Name.ToString();
            txtItemAmount.Text = passeditem.Item_Amount.ToString();
            txtItemCost.Text = passeditem.Item_cost.ToString();
            txtItemPrice.Text = passeditem.Item_Price.ToString();
            txtItemDescription.Text = passeditem.Item_description.ToString();
        }

        //search for a item when the button is clicked
        private void btnSearch_Click(object sender, EventArgs e)
        {
            //ensure the number in the textbox is valid for input
            if (!Numbonly(txtSearch.Text))
            {
                int currentitem = Convert.ToInt32(txtSearch.Text);
                if (CheckforId(currentitem))
                {
                    //find the item and set it to the current item variable
                    current_item = Finditem(currentitem);
                    if (current_item != null)
                    {
                        displayitem(current_item);

                    }
                }//error messages if the data is invalid or the item doesnt exist
                else
                {
                    MessageBox.Show("Error, Item Id doesnt exist!");
                }

            }
            else
            {
                MessageBox.Show("Error, Search input must be numeric!");
            }


        }

        //function to check if input is numeric
        private static bool Numbonly(string text)
        {
            return !NumbOnly.IsMatch(text);
        }

        //function to display the next time in the list
        private void btnNext_Click(object sender, EventArgs e)
        {
            //move foward the counter varible and display results if the counter variable is valid
            currentitemcounter++;
            if (CheckforId(currentitemcounter))
            {
                if (current_item != null)
                {
                    current_item = Finditem(currentitemcounter);
                    displayitem(current_item);

                }
            }
            else
            {
                MessageBox.Show("Error, Item Id doesnt exist!");
            }
        }

        //function to display the previous item in the list
        private void brnPrevious_Click(object sender, EventArgs e)
        {
            //de increment the counter and then check to make sure the id number existst in the database
            currentitemcounter--;
            if (CheckforId(currentitemcounter))
            {
                //if the item exists
                if (current_item != null)
                {
                    current_item = Finditem(currentitemcounter);
                    displayitem(current_item);

                }
            }
            else
            {
                MessageBox.Show("Error, Item Id doesnt exist!");
            }


        }

        //function to clear the form
        private void btnClear_Click(object sender, EventArgs e)
        {
            currentitemcounter = 1;
            txtSearch.Text = currentitemcounter.ToString();
            txtID.Text = "";
            txtItemName.Text = "";
            txtItemAmount.Text = "";
            txtItemCost.Text = "";
            txtItemPrice.Text = "";
            txtItemDescription.Text = "";

        }

        //function to store the information from the text boxes in the item variable
        private bool StoreItem()
        {
            //take the information from the text box and store if its valid
            if (string.IsNullOrEmpty(txtID.Text))
            {
                currentitemcounter = countdatbase() + 1;
            }

            else
            {
                currentitemcounter = Int32.Parse(txtID.Text);
            }
            Mysqlcon = new SqlConnection(Connstr);

            //arrays for quicker access
            String[] NumInput = { txtItemPrice.Text, txtItemCost.Text, txtItemAmount.Text };
            String[] TextInput = { txtItemName.Text, txtItemDescription.Text };

            //counters and item id variable
            current_item.ID = currentitemcounter;
            int count1 = NumInput.Length;
            int count2 = TextInput.Length;
            //check to make sure the input is valid if it is decrement the counter variables
            foreach (String currIn in NumInput)
            {
                if (Numbonly(currIn) || string.IsNullOrEmpty(currIn))
                {
                    MessageBox.Show("Error Check Numeric Inputs!");

                }
                else
                {
                    count1--;
                    if (count1 == 0)
                    {
                        current_item.Item_Amount = txtItemAmount.Text;
                        current_item.Item_cost = txtItemCost.Text;
                        current_item.Item_Price = txtItemPrice.Text;

                    }
                }

            }
            foreach (string currIn in TextInput)
            {
                if (string.IsNullOrEmpty(currIn))
                {
                    MessageBox.Show("Check Text Inputs!");
                }
                else
                {
                    count2--;
                    if (count2 == 0)
                    {
                        current_item.Item_Name = txtItemName.Text;
                        current_item.Item_description = txtItemDescription.Text;
                    }
                }
            }
            //return true if no errors otherwise return false
            if(count1 == 0 && count2 == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

            

        }

        private int countdatbase()
        {
            int count = 1;
            bool exit = false;

            while(!exit)
            {
                if(CheckforId(count))
                {
                    count++;
                }
                else 
                {
                    exit = true;
                }
            }

            return count;
        }

        //update the database if the information in the text box is valid
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            //get the current item counter from the box and run the function to store the item in the currentitem variable
            currentitemcounter = Int32.Parse(txtID.Text);
            Mysqlcon = new SqlConnection(Connstr);

            if (StoreItem())
            {
                //update database
                updateDatatbase(current_item.Item_Name, currentitemcounter, "Itemname");
                updateDatatbase(current_item.Item_description, currentitemcounter, "ItemDescription");
                updateDatatbase(current_item.Item_cost, currentitemcounter, "ItemCost");
                updateDatatbase(current_item.Item_Amount, currentitemcounter, "ItemAmount");
                updateDatatbase(current_item.Item_Price, currentitemcounter, "ItemPrice");
            }

        }

        //update the database based on passed variables
        private void updateDatatbase(string itemtoadd, int currentcounter, string columnname)
        {
            Mysqlcon = new SqlConnection(Connstr);
            string sql_stat = "UPDATE Items SET " + columnname + " = '" + itemtoadd + "' WHERE ID= " + currentcounter;
            using (mysqlcomm = new SqlCommand(sql_stat, Mysqlcon))
            {
                mysqlcomm.Parameters.AddWithValue(columnname, itemtoadd);
                Mysqlcon.Open();
                mysqlcomm.ExecuteNonQuery();
                Mysqlcon.Close();
            }
        }

        //when the new button is clicked add a new item to the database
        private void BtnNew_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(txtID.Text))
            {
                if(string.IsNullOrEmpty(txtID.Text) &&  string.IsNullOrEmpty(txtItemName.Text) && string.IsNullOrEmpty(txtItemAmount.Text) && string.IsNullOrEmpty(txtItemCost.Text) 
                  && string.IsNullOrEmpty(txtItemPrice.Text) && string.IsNullOrEmpty(txtItemDescription.Text))
                {
                    MessageBox.Show("Please fill in all the data before adding anew item!");
                }
                else
                {
                    if (StoreItem())
                    {
                        Mysqlcon = new SqlConnection(Connstr);
                        string sql_stat = "INSERT INTO items (Itemname, ItemAmount, ItemCost, ItemPrice, ItemDescription) VALUES (@ItemName, @ItemAmount, @ItemCost, @ItemPrice, @ItemDescription)";
                        MessageBox.Show(sql_stat);
                         using (mysqlcomm = new SqlCommand(sql_stat, Mysqlcon))
                        {  
                            //open the connection and add the items to the database
                            Mysqlcon.Open();
                            mysqlcomm.Parameters.Add("@ItemName", SqlDbType.NVarChar).Value = txtItemName.Text;
                            mysqlcomm.Parameters.Add("@ItemAmount", SqlDbType.Int).Value = txtItemAmount.Text;
                            mysqlcomm.Parameters.Add("@ItemCost", SqlDbType.Money).Value = Convert.ToDecimal(txtItemCost.Text);
                            mysqlcomm.Parameters.Add("@ItemPrice", SqlDbType.Money).Value = Convert.ToDecimal(txtItemPrice.Text);
                            mysqlcomm.Parameters.Add("@ItemDescription", SqlDbType.NVarChar).Value=txtItemDescription.Text;


                            mysqlcomm.ExecuteNonQuery();
                            Mysqlcon.Close();
                         }
                    }
                }
                
            }
            else
            {
                MessageBox.Show("Use the Clear Button, prior to adding a new record");
            }






        }


    }

    }

