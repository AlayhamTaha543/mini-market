using Microsoft.Data.Sqlite;
using MiniMarketApp.Helpers;
using MiniMarketApp.Models;

namespace MiniMarketApp.Data.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly DatabaseContext _databaseContext;

    public ProductRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public int Add(Product product)
    {
        using var connection = _databaseContext.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
INSERT INTO Products (Name, Barcode, UnitSellPrice, UnitCostPrice, Quantity, LowStockThreshold, IsActive)
VALUES ($name, $barcode, $unitSellPrice, $unitCostPrice, $quantity, $lowStockThreshold, 1);
SELECT last_insert_rowid();";

        command.Parameters.AddWithValue("$name", product.Name);
        command.Parameters.AddWithValue("$barcode", (object?)product.Barcode ?? DBNull.Value);
        command.Parameters.AddWithValue("$unitSellPrice", product.UnitSellPrice);
        command.Parameters.AddWithValue("$unitCostPrice", product.UnitCostPrice);
        command.Parameters.AddWithValue("$quantity", product.Quantity);
        command.Parameters.AddWithValue("$lowStockThreshold", product.LowStockThreshold);

        return Convert.ToInt32((long)command.ExecuteScalar()!);
    }

    public Product? GetById(int productId)
    {
        using var connection = _databaseContext.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
SELECT ProductId, Name, Barcode, UnitSellPrice, UnitCostPrice, Quantity, LowStockThreshold, IsActive
FROM Products
WHERE ProductId = $productId;";

        command.Parameters.AddWithValue("$productId", productId);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return Map(reader);
    }

    public List<Product> GetByBarcode(string barcode)
    {
        using var connection = _databaseContext.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
SELECT ProductId, Name, Barcode, UnitSellPrice, UnitCostPrice, Quantity, LowStockThreshold, IsActive
FROM Products
WHERE Barcode = $barcode AND IsActive = 1
ORDER BY Name;";

        command.Parameters.AddWithValue("$barcode", barcode);

        var products = new List<Product>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            products.Add(Map(reader));
        }

        return products;
    }

    public List<Product> GetLowStockProducts()
    {
        using var connection = _databaseContext.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
SELECT ProductId, Name, Barcode, UnitSellPrice, UnitCostPrice, Quantity, LowStockThreshold, IsActive
FROM Products
WHERE IsActive = 1 AND Quantity <= LowStockThreshold
ORDER BY Name;";

        var products = new List<Product>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            products.Add(Map(reader));
        }

        return products;
    }

    public void Update(Product product)
    {
        using var connection = _databaseContext.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
UPDATE Products
SET Name = $name,
    Barcode = $barcode,
    UnitSellPrice = $unitSellPrice,
    UnitCostPrice = $unitCostPrice,
    Quantity = $quantity,
    LowStockThreshold = $lowStockThreshold,
    UpdatedAt = datetime('now')
WHERE ProductId = $productId;";

        command.Parameters.AddWithValue("$productId", product.ProductId);
        command.Parameters.AddWithValue("$name", product.Name);
        command.Parameters.AddWithValue("$barcode", (object?)product.Barcode ?? DBNull.Value);
        command.Parameters.AddWithValue("$unitSellPrice", product.UnitSellPrice);
        command.Parameters.AddWithValue("$unitCostPrice", product.UnitCostPrice);
        command.Parameters.AddWithValue("$quantity", product.Quantity);
        command.Parameters.AddWithValue("$lowStockThreshold", product.LowStockThreshold);

        command.ExecuteNonQuery();
    }

    public void SoftDelete(int productId)
    {
        using var connection = _databaseContext.CreateConnection();

        using (var historyCommand = connection.CreateCommand())
        {
            historyCommand.CommandText = @"
SELECT EXISTS(
    SELECT 1 FROM SaleItems WHERE ProductId = $productId
    UNION ALL
    SELECT 1 FROM Losses WHERE ProductId = $productId
);";
            historyCommand.Parameters.AddWithValue("$productId", productId);

            var hasHistory = Convert.ToInt32((long)historyCommand.ExecuteScalar()!) == 1;
            if (hasHistory)
            {
                throw new ProductDeletionBlockedException(LocalizedStrings.Get("ProductDeleteBlockedMessage"));
            }
        }

        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = @"
UPDATE Products
SET IsActive = 0,
    UpdatedAt = datetime('now')
WHERE ProductId = $productId;";
            command.Parameters.AddWithValue("$productId", productId);
            command.ExecuteNonQuery();
        }
        catch (SqliteException sqliteException) when (sqliteException.SqliteErrorCode == 19)
        {
            throw new ProductDeletionBlockedException(LocalizedStrings.Get("ProductDeleteBlockedMessage"));
        }
    }

    private static Product Map(SqliteDataReader reader)
    {
        return new Product
        {
            ProductId = reader.GetInt32(0),
            Name = reader.GetString(1),
            Barcode = reader.IsDBNull(2) ? null : reader.GetString(2),
            UnitSellPrice = reader.GetDecimal(3),
            UnitCostPrice = reader.GetDecimal(4),
            Quantity = reader.GetInt32(5),
            LowStockThreshold = reader.GetInt32(6),
            IsActive = reader.GetInt32(7) == 1,
        };
    }
}
