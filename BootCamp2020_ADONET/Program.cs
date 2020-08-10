using System;
using System.Data;
using System.Data.SqlClient;

namespace BootCamp2020_ADONET
{
    class Program
    {
        private const string windowsAuthConnectionString = "Data Source=LVAGPLTP3448\\SQL2017;" +
                                            "Initial Catalog=AdventureWorksLT2017;" +
                                            "Integrated Security=SSPI;";

        private const string userIdConnectionString = "Data Source=LVAGPLTP3448\\SQL2017;" +
                                            "Initial Catalog=AdventureWorksLT2017;" +
                                            "User Id=sa;" +
                                            "Password=sql@1981#";

        private const string productName = "MyProduct";
        private const string productNumber = "ACN-365";

        static void Main(string[] args)
        {
            
            // Read data using data reader
            //SelectDataUsingDataReader();

            // Read data using data adapter
            // SelectDataUsingDataAdapter();

            // Insert Data
            //  InsertDataUsingCommandObject(productName, productNumber);

            // Update data using data set
           //  UpdateProductNameUsingDataSet("Road-750 Black, 52", "Road-750 White, 75");

            // Search data set
            //  SearchDataSet(productName);

            // Delete data using data set
             // DeleteProductUsingDataSet(2003);

            // Create DataView
          //  CreateDataView();

            // Exception Handling
           // ExceptionHandling("abc");

            Console.ReadKey();
        }

        private static void SelectDataUsingDataReader()
        {
            // Step-1 Setup Connection Object
            Console.WriteLine("1. Setting up connection object");
            // Make Connection Object
            SqlConnection connection = new SqlConnection();
            // Assign connection string to connection object
            connection.ConnectionString = "Data Source=LVAGPLTP3448\\SQL2017;" +
                                            "Initial Catalog=AdventureWorksLT2017;" +
                                            "Integrated Security=SSPI;";

            // Step-2 Setup Command Object
            Console.WriteLine("2. Setting up command object");
            // Create Command object
            SqlCommand command = new SqlCommand();
            // Assign connection to command
            command.Connection = connection;
            // Set command type
            command.CommandType = CommandType.Text;
            // Set Command text
            command.CommandText = "SELECT * from SalesLT.Customer";

            // Step-3 Open connection to database
            Console.WriteLine("3. Opening connection to database");
            // Open connection
            connection.Open();

            // Step-4 Execute the commmand
            Console.WriteLine("4. Executing the command");
            // Read data using sqldatareader
            SqlDataReader reader = command.ExecuteReader();

            // Step-5 Read data
            Console.WriteLine("5. Reading data from reader");
            // Loop through the data rows
            Console.WriteLine();
            Console.WriteLine("List of Customers:");
            while (reader.Read())
            {
                Console.WriteLine(reader["FirstName"] + " " + reader["LastName"]);
            }
            Console.WriteLine("Finished reading data from reader");
            Console.WriteLine();
           
            // Step-6 Close database connection
            Console.WriteLine("6. Close the connection");
            connection.Close();
        }

        private static void SelectDataUsingDataAdapter()
        {
            // Step-1 Setup Connection Object
            Console.WriteLine("1. Setting up connection");
            SqlConnection connection = new SqlConnection("Data Source=LVAGPLTP3448\\SQL2017;" +
                                                            "Initial Catalog=AdventureWorksLT2017;" +
                                                            "Integrated Security=SSPI;");

            // Step-2 Setup data adapter
            Console.WriteLine("2. Setting up data adapter");
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand selectCMD = new SqlCommand("SELECT ProductNumber, Name, StandardCost FROM SalesLT.Product", connection);
            adapter.SelectCommand = selectCMD;

            // Step-3 Fill Data set
            Console.WriteLine("3. Fill data set");
            DataSet productDS = new DataSet();
            adapter.Fill(productDS, "Products");

            // Step-4 Read data from data set
            Console.WriteLine("4. Read data from data set");
            Console.WriteLine();
            Console.WriteLine("List of products");
            for (var i = 0; i < productDS.Tables[0].Rows.Count; i++)
            {
                Console.WriteLine(productDS.Tables[0].Rows[i]["ProductNumber"] + " " + productDS.Tables[0].Rows[i]["Name"]);
            }
        }

        private static void InsertDataUsingCommandObject(string productName, string productNumber)
        {
            // Insert Query
            string query = "INSERT INTO SalesLT.Product (Name, ProductNumber, StandardCost, ProductCategoryID, ProductModelID, ListPrice, SellStartDate) " +
                           "VALUES (@Name, @ProductNumber, @StandardCost, @ProductCategoryID, @ProductModelID, @ListPrice, @SellStartDate) ";

            // Create connection and command
            using (SqlConnection connection = new SqlConnection(windowsAuthConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // define parameters and their values
                command.Parameters.Add("@Name", SqlDbType.VarChar, 50).Value = productName;
                command.Parameters.Add("@ProductNumber", SqlDbType.VarChar, 50).Value = productNumber;
                command.Parameters.Add("@StandardCost", SqlDbType.Decimal).Value = 150.36;
                command.Parameters.Add("@ListPrice", SqlDbType.Decimal).Value = 148.36;
                command.Parameters.Add("@ProductCategoryID", SqlDbType.Int).Value = 1;
                command.Parameters.Add("@ProductModelID", SqlDbType.Int).Value = 1;
                command.Parameters.Add("@SellStartDate", SqlDbType.Date).Value = DateTime.Now; 

                // Open connection, Execute command, Close connection
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

                Console.WriteLine("Product is inserted.");
            }
        }

        private static void UpdateProductNameUsingDataSet(string oldProductName, string newProductName)
        {

            using (SqlConnection connection =
               new SqlConnection(windowsAuthConnectionString))
            {
                SqlDataAdapter dataAdpater = new SqlDataAdapter(
                  "SELECT ProductID, Name FROM SalesLT.Product",
                  connection);

                dataAdpater.UpdateCommand = new SqlCommand(
                   "UPDATE SalesLT.Product SET Name = @Name " +
                   "WHERE ProductID = @ProductID", connection);

                dataAdpater.UpdateCommand.Parameters.Add(
                   "@Name", SqlDbType.NVarChar, 15, "Name");

                SqlParameter parameter = dataAdpater.UpdateCommand.Parameters.Add(
                  "@ProductID", SqlDbType.Int);
                parameter.SourceColumn = "ProductID";
                parameter.SourceVersion = DataRowVersion.Original;

                DataTable productTable = new DataTable();
                dataAdpater.Fill(productTable);

                var searchString = $"Name = '{oldProductName}'";
                DataRow[] foundRows = productTable.Select(searchString);

                DataRow productRow = foundRows[0];
                productRow["Name"] = newProductName;

                dataAdpater.Update(productTable);

                Console.WriteLine("Product is updated.");
            }

        }

        private static void DeleteProductUsingDataSet(int productID)
        {

            using (SqlConnection connection =
               new SqlConnection(windowsAuthConnectionString))
            {
                SqlDataAdapter dataAdpater = new SqlDataAdapter();

                dataAdpater.SelectCommand = new SqlCommand(
                   "SELECT ProductID FROM SalesLT.Product", connection);

                dataAdpater.DeleteCommand = new SqlCommand(
                   "DELETE FROM SalesLT.Product " +
                   "WHERE ProductID = @ProductID", connection);

                SqlParameter parameter = dataAdpater.DeleteCommand.Parameters.AddWithValue("@ProductID", productID);
                parameter.SourceVersion = DataRowVersion.Original;

                DataTable productTable = new DataTable();
                dataAdpater.Fill(productTable);

                DataRow[] dataRows = productTable.Select("ProductID = " + productID);
                dataRows[0].Delete();
                
                dataAdpater.Update(productTable);

                Console.WriteLine("Product is deleted.");
            }

        }

        private static void SearchDataSet(string productName)
        {
            SqlConnection connection = new SqlConnection(windowsAuthConnectionString);

            SqlCommand selectCMD = new SqlCommand("SELECT ProductID, ProductNumber, Name, StandardCost FROM SalesLT.Product", connection);
            selectCMD.CommandTimeout = 30;

            SqlDataAdapter productDA = new SqlDataAdapter();
            productDA.SelectCommand = selectCMD;

            DataSet productDS = new DataSet();
            productDA.Fill(productDS, "Products");

            var searchString = $"Name = '{productName}'";
            DataRow[] foundRows = productDS.Tables[0].Select(searchString);

            if (foundRows.Length == 0)
            {
                Console.WriteLine("No data found");
            }

            foreach (DataRow foundRow in foundRows)
            {
                Console.WriteLine(foundRow["ProductID"] + " " + foundRow["ProductNumber"] + " " + foundRow["Name"]);
            }
        }

        private static void CreateDataView()
        {
            SqlConnection connection = new SqlConnection(userIdConnectionString);

            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand selectCMD = new SqlCommand("SELECT ProductCategoryID,ProductNumber, Name, StandardCost FROM SalesLT.Product", connection);
            adapter.SelectCommand = selectCMD;

            DataSet productDS = new DataSet();
            adapter.Fill(productDS, "Products");

            DataView dataView = new DataView(productDS.Tables[0], 
                                                "ProductCategoryID=6", 
                                                "Name", 
                                                DataViewRowState.CurrentRows);

            Console.WriteLine("List of products with ProductCategoryID=6");
            Console.WriteLine();
            for (var i = 0; i < dataView.Table.Rows.Count; i++)
            {
                Console.WriteLine(dataView.Table.Rows[i]["ProductNumber"] + " " + dataView.Table.Rows[i]["Name"]);
            }
        }

        private static void ExceptionHandling(string productName)
        {
            try
            {
                SqlConnection connection = new SqlConnection("Data Source=LVAGPLTP3448\\SQL201;" +
                                                "Initial Catalog=AdventureWorksLT2017;" +
                                                "Integrated Security=SSPI;");

                SqlCommand selectCMD = new SqlCommand("SELECT ProductID, ProductNumber, Name, StandardCost FROM SalesLT.Product", connection);
                selectCMD.CommandTimeout = 30;

                SqlDataAdapter productDA = new SqlDataAdapter();
                productDA.SelectCommand = selectCMD;

                DataSet productDS = new DataSet();
                productDA.Fill(productDS, "Products");

                var searchString = $"Name = '{productName}'";
                DataRow[] foundRows = productDS.Tables[0].Select(searchString);

                if (foundRows.Length == 0)
                {
                    Console.WriteLine("No data found");
                }

                foreach (DataRow foundRow in foundRows)
                {
                    Console.WriteLine(foundRow["ProductID"] + " " + foundRow["ProductNumber"] + " " + foundRow["Name"]);
                }
            }
            catch(SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
