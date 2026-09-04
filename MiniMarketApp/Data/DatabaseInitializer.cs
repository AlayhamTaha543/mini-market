namespace MiniMarketApp.Data;

public static class DatabaseInitializer
{
    public static void Initialize(DatabaseContext databaseContext)
    {
        using var connection = databaseContext.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
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
    Role INTEGER NOT NULL,
    IsActive INTEGER NOT NULL DEFAULT 1,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS Sales (
    SaleId INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    TotalAmount NUMERIC NOT NULL,
    IsActive INTEGER NOT NULL DEFAULT 1,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

CREATE TABLE IF NOT EXISTS SaleItems (
    SaleItemId INTEGER PRIMARY KEY AUTOINCREMENT,
    SaleId INTEGER NOT NULL,
    ProductId INTEGER NOT NULL,
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
    Quantity INTEGER NOT NULL,
    UnitCostPrice NUMERIC NOT NULL,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);";

        command.ExecuteNonQuery();
    }
}
