using Microsoft.Data.Sqlite;

namespace MiniMarketApp.Data;

public static class DatabaseInitializer
{
    public static void Initialize(DatabaseContext databaseContext)
    {
        using var connection = databaseContext.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
CREATE TABLE IF NOT EXISTS Categories (
    CategoryId INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS Products (
    ProductId INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Barcode TEXT NULL,
    UnitSellPrice NUMERIC NOT NULL,
    UnitCostPrice NUMERIC NOT NULL,
    Quantity INTEGER NOT NULL DEFAULT 0,
    LowStockThreshold INTEGER NOT NULL DEFAULT 5,
    IsActive INTEGER NOT NULL DEFAULT 1,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE INDEX IF NOT EXISTS IX_Products_Barcode ON Products(Barcode);

CREATE TABLE IF NOT EXISTS Users (
    UserId INTEGER PRIMARY KEY AUTOINCREMENT,
    UserName TEXT NOT NULL,
    PasswordHash TEXT NOT NULL,
    Role INTEGER NOT NULL CHECK (Role IN (1, 2)),
    IsActive INTEGER NOT NULL DEFAULT 1,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS Sales (
    SaleId INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    TotalAmount NUMERIC NOT NULL,
    TotalCost NUMERIC NOT NULL DEFAULT 0,
    Discount NUMERIC NOT NULL DEFAULT 0,
    IsActive INTEGER NOT NULL DEFAULT 1,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

CREATE TABLE IF NOT EXISTS SaleItems (
    SaleItemId INTEGER PRIMARY KEY AUTOINCREMENT,
    SaleId INTEGER NOT NULL,
    ProductId INTEGER NOT NULL,
    ProductName TEXT NOT NULL,
    Quantity INTEGER NOT NULL,
    UnitSellPrice NUMERIC NOT NULL,
    UnitCostPrice NUMERIC NOT NULL,
    LineTotal NUMERIC NOT NULL,
    FOREIGN KEY (SaleId) REFERENCES Sales(SaleId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

CREATE TABLE IF NOT EXISTS Losses (
    LossId INTEGER PRIMARY KEY AUTOINCREMENT,
    ProductId INTEGER NOT NULL,
    UserId INTEGER NOT NULL,
    Reason TEXT NOT NULL CHECK (Reason IN ('Expired', 'Damaged', 'Stolen', 'Other')),
    Quantity INTEGER NOT NULL,
    UnitCostPrice NUMERIC NOT NULL,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);";

        command.ExecuteNonQuery();

        EnsureColumnExists(connection, "Sales", "TotalCost", "NUMERIC NOT NULL DEFAULT 0");
        EnsureColumnExists(connection, "Sales", "Discount", "NUMERIC NOT NULL DEFAULT 0");
        EnsureColumnExists(connection, "SaleItems", "ProductName", "TEXT NOT NULL DEFAULT ''");
        EnsureColumnExists(connection, "Losses", "UserId", "INTEGER NOT NULL DEFAULT 1");
        EnsureColumnExists(connection, "Losses", "Reason", "TEXT NOT NULL DEFAULT 'Other'");
    }

    private static void EnsureColumnExists(SqliteConnection connection, string tableName, string columnName, string definition)
    {
        using var checkCommand = connection.CreateCommand();
        checkCommand.CommandText = $"PRAGMA table_info({tableName});";

        using var reader = checkCommand.ExecuteReader();
        while (reader.Read())
        {
            if (string.Equals(reader.GetString(1), columnName, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
        }

        using var alterCommand = connection.CreateCommand();
        alterCommand.CommandText = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {definition};";
        alterCommand.ExecuteNonQuery();
    }
}
