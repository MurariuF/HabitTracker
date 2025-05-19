using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace HabitTracker
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Batteries.Init();
            var connectionString = @"DataSource=habitTracker.db";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = @"CREATE TABLE IF NOT EXISTS drinking_water(
                                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                Date TEXT,
                                                Qunatity INTEGER
                                             )";

                tableCommand.ExecuteNonQuery();
                
                connection.Close();
            }
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
                    case 0:
                        Console.WriteLine("\n\nClosing Application");
                        closeApp = true;
                        break;
                    case 1:
                        GetAllRecords();
                        break;
                    case 2:
                        InsertRecord();
                        break;
                    case 3:
                        DeleteRecord();
                        break;
                    case 4:
                        UpdateRecord();
                        break;
                    default:
                        Console.WriteLine("\n\nInvalid command. Please type a number between 0 and 4");
                        break;
                }
            }
        }
    }
}
