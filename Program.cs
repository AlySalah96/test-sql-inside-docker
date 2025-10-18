using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

class Program
{
    static async Task Main()
    {
        var connectionString = "Server=sqlserver2022,1433;Database=master;User Id=sa;Password=Your_password123;TrustServerCertificate=True;";
       // var connectionString = "Server=localhost,1433;Database=master;User Id=sa;Password=Your_password123;TrustServerCertificate=True;";

        try
        {
            Console.WriteLine("🚀 Connecting to SQL Server... alyyyyyyyyyyyyyyyyyyyyyyyy");
            using var masterConn = new SqlConnection(connectionString);
            await masterConn.OpenAsync();

            // 1️⃣ Create database if not exists
            var createDbCmd = new SqlCommand("IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TestDb') CREATE DATABASE TestDb;", masterConn);
            await createDbCmd.ExecuteNonQueryAsync();
            Console.WriteLine("✅ Database 'TestDb' ready.");

            // 2️⃣ Connect to TestDb
            var dbConnectionString = connectionString.Replace("Database=master", "Database=TestDb");
            using var dbConn = new SqlConnection(dbConnectionString);
            await dbConn.OpenAsync();

            // 3️⃣ Create table if not exists
            var createTableCmd = new SqlCommand(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Logs' AND xtype='U')
                CREATE TABLE Logs (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    Message NVARCHAR(200),
                    CreatedAt DATETIME DEFAULT GETDATE()
                );", dbConn);
            await createTableCmd.ExecuteNonQueryAsync();
            Console.WriteLine("✅ Table 'Logs' ready.");

            // 4️⃣ Insert a new log record
            var insertCmd = new SqlCommand("INSERT INTO Logs (Message) VALUES (@msg)", dbConn);
            insertCmd.Parameters.AddWithValue("@msg", $"Hello from container at {DateTime.Now}");
            await insertCmd.ExecuteNonQueryAsync();
            Console.WriteLine("📝 Inserted new log record.");

            // 5️⃣ Read all logs
            var readCmd = new SqlCommand("SELECT Id, Message, CreatedAt FROM Logs ORDER BY Id DESC", dbConn);
            using var reader = await readCmd.ExecuteReaderAsync();
            Console.WriteLine("📜 Current Logs:");
            while (await reader.ReadAsync())
            {
                Console.WriteLine($"  {reader.GetInt32(0)} | {reader.GetString(1)} | {reader.GetDateTime(2)}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }
}
