using System.Globalization;
using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace HabitTracker
{
    internal class Program
    {
        static string connectionString = @"DataSource=habitTracker.db";
        static void Main(string[] args)
        {
            Batteries.Init();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = @"CREATE TABLE IF NOT EXISTS drinking_water(
                                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                Date TEXT,
                                                Quantity INTEGER
                                             )";

                tableCommand.ExecuteNonQuery();
                
                connection.Close();
            }

            GetUserInput();
        }

        static void GetUserInput()
        {
            Console.Clear();
            bool closeApp = false;
            while (closeApp == false)
            {
                Console.WriteLine("\n\nMain Menu");
                Console.WriteLine("\nWhat would you like to do?");
                Console.WriteLine("\nType 0 to Close Application");
                Console.WriteLine("\n1. View all records");
                Console.WriteLine("\n2. Insert record");
                Console.WriteLine("\n3. Delete record");
                Console.WriteLine("\n4. Update record");

                string commandInput = Console.ReadLine();

                switch (commandInput)
                {
                    case "0":
                        Console.WriteLine("\n\nClosing Application");
                        closeApp = true;
                        break;
                    case "1":
                        GetAllRecords();
                        break;
                    case "2":
                        InsertRecord();
                        break;
                    case "3":
                        DeleteRecord();
                        break;
                    case "4":
                        UpdateRecord();
                        break;
                    default:
                        Console.WriteLine("\n\nInvalid command. Please type a number between 0 and 4");
                        break;
                }
            }
        }

        private static void UpdateRecord()
        {
            Console.Clear();
            GetAllRecords();

            var recordId = GetNumberInput("\n\nPlease enter the ID of the record you want to update. Type 0 to return to main menu");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = $"Select exists(select 1 from drinking_water where Id = {recordId})";
                int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (checkQuery == 0)
                {
                    Console.WriteLine($"\n\nNo record found with ID {recordId}. Returning to main menu.");
                    connection.Close();
                    UpdateRecord();
                }

                string date = GetDateInput();
                int quantity =
                    GetNumberInput(
                        "\n\nPlease insert number of glasses of water or other measure of your choice(no decimal allowed)\n\n");

                var tableCommand = connection.CreateCommand();

                tableCommand.CommandText = $"Update drinking_water set date = '{date}', quantity ={quantity} where Id = {recordId} ";
                tableCommand.ExecuteNonQuery();

                connection.Close();
            }
        }

        private static void DeleteRecord()
        {
            Console.Clear();
            GetAllRecords();
            Console.WriteLine("\n\nPlease enter the ID of the record you want to delete. Type 0 to return to main menu");
            string idInput = Console.ReadLine();
            if (idInput == "0") GetUserInput();
            int id = Convert.ToInt32(idInput);
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = $"Delete from drinking_water where Id = {id}";
                int rowsAffected = tableCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Console.WriteLine($"\n\nRecord with ID {id} deleted successfully.");
                }
                else
                {
                    Console.WriteLine($"\n\nNo record found with ID {id}.");
                }
                connection.Close();
            }
        }

        private static void GetAllRecords()
        {
            Console.Clear();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = "Select * from drinking_water";

                List<DrikingWater> tableData = new();

                SqliteDataReader reader = tableCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    while(reader.Read())
                    {
                        DrikingWater record = new()
                        {
                            Id = reader.GetInt32(0),
                            Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yyyy", new CultureInfo("en-US")),
                            Quantity = reader.GetInt32(2)
                        };
                        tableData.Add(record);
                    }
                }
                else
                {
                    Console.WriteLine("No rows found");
                }
                connection.Close();

                foreach (var rows in tableData)
                {
                    Console.WriteLine($"{rows.Id} - {rows.Date.ToString("dd-MM-yyyy")} - Quantity: {rows.Quantity}");
                }
            }
        }

        private static void InsertRecord()
        {
            string date = GetDateInput();

            int quantity = GetNumberInput("\n\nPlease insert number of glasses or other measure of your choice(no decimals allowed)\n\n");

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = $"Insert into drinking_water(Date, Quantity) values('{date}', {quantity})";

                tableCommand.ExecuteNonQuery();

                connection.Close();
            }
        }

        private static int GetNumberInput(string message)
        {
            Console.WriteLine(message);

            string numberInput = Console.ReadLine();

            if(numberInput == "0") GetUserInput();

            int finalInput = Convert.ToInt32(numberInput);

            return finalInput;
        }

        private static string GetDateInput()
        {
            Console.WriteLine("\n\nPlease enter the date in the format dd-mm-yy. Type 0 to return to main menu");
            string dateInput = Console.ReadLine();

            if(dateInput == "0")  GetUserInput();

            return dateInput;
        }
    }

    public class DrikingWater
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
    }
}
