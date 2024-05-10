using Microsoft.Data.Sqlite;
using System.Globalization;

namespace Water_Tracker
{
    internal class Database()
    {
        private static string connectionString = @"Data Source=water-Tracker.db";
        internal static void Create()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                //Open connection
                connection.Open();
                //Tell database to create a command
                var tableCmd = connection.CreateCommand();

                // Make the command a command text
                // "@" allows for multi-line statements
                tableCmd.CommandText =
                    @"CREATE TABLE IF NOT EXISTS drinking_water (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Date TEXT,
                        Quantity INTEGER
                        )";

                // Execute and tell databased to not return any values
                tableCmd.ExecuteNonQuery();

                // Close connetion with database
                connection.Close();
            }
        }

        internal static List<DrinkingWater> GetData()
        {
            Console.Clear();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var tableCmd = connection.CreateCommand();
                // selecting all collumns
                tableCmd.CommandText =
                    $"SELECT * FROM drinking_water ";

                List<DrinkingWater> tableData = new();

                SqliteDataReader reader = tableCmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(
                        new DrinkingWater
                        {
                            Id = reader.GetInt32(0),
                            Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", new CultureInfo("en-US")),
                            Quantity = reader.GetInt32(2)
                        }); ;
                    }
                }
                else
                {
                    Console.WriteLine("No rows found");
                }

                connection.Close();

                return tableData;
            }
        }

        internal static void Add(string date, int quantity)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText =
                   $"INSERT INTO drinking_water(date, quantity) VALUES('{date}', {quantity})";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        internal static int Remove(int Id)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = $"DELETE from drinking_water WHERE Id = '{Id}'";

                int rowCount = tableCmd.ExecuteNonQuery();

                connection.Close();

                return rowCount;
            }
        }

        internal static void UpdateIds(int deletedId)
        {
            // Retrieve all records in the desired order
            var allRecords = GetData().OrderBy(record => record.Id).ToList();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                // Begin transaction
                command.CommandText = "BEGIN";
                command.ExecuteNonQuery();

                // Drop the existing table
                command.CommandText = "DROP TABLE IF EXISTS temp_drinking_water";
                command.ExecuteNonQuery();

                // Create a temporary table to hold records in the desired order
                command.CommandText = @"
            CREATE TEMPORARY TABLE temp_drinking_water (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Date TEXT,
                Quantity INTEGER
            )";
                command.ExecuteNonQuery();

                // Insert records into the temporary table with new sequential IDs
                int newId = 1;
                foreach (var record in allRecords)
                {
                    command.CommandText = $"INSERT INTO temp_drinking_water (Id, Date, Quantity) VALUES ({newId}, '{record.Date.ToString("dd-MM-yy")}', {record.Quantity})";
                    command.ExecuteNonQuery();
                    newId++;
                }

                // Drop the original table
                command.CommandText = "DROP TABLE IF EXISTS drinking_water";
                command.ExecuteNonQuery();

                // Rename the temporary table to the original table name
                command.CommandText = "ALTER TABLE temp_drinking_water RENAME TO drinking_water";
                command.ExecuteNonQuery();

                // Commit transaction
                command.CommandText = "COMMIT";
                command.ExecuteNonQuery();

                connection.Close();
            }
        }


        internal static void Update(string date, int quantity, int Id)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"UPDATE drinking_water SET date = '{date}', quantity = {quantity} WHERE Id = {Id}";

                tableCmd.ExecuteNonQuery();

                connection.Close();
            }
        }

        internal static bool CheckID(int Id)
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM drinking_water WHERE Id = {Id})";
                int checkQuery = Convert.ToInt32(checkCmd.ExecuteScalar());

                connection.Close();

                if (checkQuery == 1) { return true; }
                return false;
            }
        }
    }
}
