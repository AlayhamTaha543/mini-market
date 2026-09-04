using Microsoft.Data.Sqlite;

namespace MiniMarketApp.Data;

public sealed class DatabaseContext
{
    private readonly string _connectionString;

    public DatabaseContext(string databaseFilePath = "mini-market.db")
    {
        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = databaseFilePath,
        }.ToString();
    }

    public SqliteConnection CreateConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var pragmaCommand = connection.CreateCommand();
        pragmaCommand.CommandText = "PRAGMA foreign_keys = ON;";
        pragmaCommand.ExecuteNonQuery();

        return connection;
    }
}
