using Microsoft.Data.Sqlite;

namespace HabitTracker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var connectionString = @"DataSource=habittracker.db";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var tableCommand = connection.CreateCommand();
                tableCommand.CommandText = "";


            }
        }
    }
}
