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
                    //case "3":
                    //    DeleteRecord();
                    //    break;
                    //case "4":
                    //    UpdateRecord();
                    //    break;
                    default:
                        Console.WriteLine("\n\nInvalid command. Please type a number between 0 and 4");
                        break;
                }
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
                            Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", new CultureInfo("en-US")),
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
