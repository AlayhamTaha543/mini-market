using Microsoft.Data.Sqlite;
using MiniMarketApp.Data;
using MiniMarketApp.Data.Repositories;
using MiniMarketApp.Models;

namespace MiniMarketApp.Tests;

public sealed class ProductRepositorySoftDeleteTests
{
    [Fact]
    public void SoftDelete_ProductWithoutHistory_SetsProductInactive()
    {
        var databasePath = CreateTempDatabasePath();

        try
        {
            var context = new DatabaseContext(databasePath);
            DatabaseInitializer.Initialize(context);
            var repository = new ProductRepository(context);

            var productId = repository.Add(new Product
            {
                Name = "Product A",
                Barcode = "111",
                UnitSellPrice = 25m,
                UnitCostPrice = 20m,
                Quantity = 3,
                LowStockThreshold = 5,
            });

            repository.SoftDelete(productId);

            var product = repository.GetById(productId);
            Assert.NotNull(product);
            Assert.False(product!.IsActive);
        }
        finally
        {
            DeleteIfExists(databasePath);
        }
    }

    [Fact]
    public void SoftDelete_ProductWithSalesHistory_ThrowsBlockedException()
    {
        var databasePath = CreateTempDatabasePath();

        try
        {
            var context = new DatabaseContext(databasePath);
            DatabaseInitializer.Initialize(context);
            var repository = new ProductRepository(context);

            var productId = repository.Add(new Product
            {
                Name = "Product B",
                Barcode = "222",
                UnitSellPrice = 40m,
                UnitCostPrice = 30m,
                Quantity = 8,
                LowStockThreshold = 5,
            });

            var userId = InsertUser(context);
            var saleId = InsertSale(context, userId, totalAmount: 80m, discount: 0m, totalCost: 60m);
            InsertSaleItem(context, saleId, productId, "Product B", quantity: 2, unitSellPrice: 40m, unitCostPrice: 30m);

            Assert.Throws<ProductDeletionBlockedException>(() => repository.SoftDelete(productId));
        }
        finally
        {
            DeleteIfExists(databasePath);
        }
    }

    private static int InsertUser(DatabaseContext context)
    {
        using var connection = context.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
INSERT INTO Users (UserName, PasswordHash, Role, IsActive)
VALUES ($userName, $passwordHash, $role, 1);
SELECT last_insert_rowid();";
        command.Parameters.AddWithValue("$userName", "owner");
        command.Parameters.AddWithValue("$passwordHash", "hash");
        command.Parameters.AddWithValue("$role", (int)UserRole.Owner);

        return Convert.ToInt32((long)command.ExecuteScalar()!);
    }

    private static int InsertSale(DatabaseContext context, int userId, decimal totalAmount, decimal discount, decimal totalCost)
    {
        using var connection = context.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
INSERT INTO Sales (UserId, TotalAmount, Discount, TotalCost, IsActive)
VALUES ($userId, $totalAmount, $discount, $totalCost, 1);
SELECT last_insert_rowid();";
        command.Parameters.AddWithValue("$userId", userId);
        command.Parameters.AddWithValue("$totalAmount", totalAmount);
        command.Parameters.AddWithValue("$discount", discount);
        command.Parameters.AddWithValue("$totalCost", totalCost);

        return Convert.ToInt32((long)command.ExecuteScalar()!);
    }

    private static void InsertSaleItem(
        DatabaseContext context,
        int saleId,
        int productId,
        string productName,
        int quantity,
        decimal unitSellPrice,
        decimal unitCostPrice)
    {
        using var connection = context.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
INSERT INTO SaleItems (SaleId, ProductId, ProductName, Quantity, UnitSellPrice, UnitCostPrice, LineTotal)
VALUES ($saleId, $productId, $productName, $quantity, $unitSellPrice, $unitCostPrice, $lineTotal);";
        command.Parameters.AddWithValue("$saleId", saleId);
        command.Parameters.AddWithValue("$productId", productId);
        command.Parameters.AddWithValue("$productName", productName);
        command.Parameters.AddWithValue("$quantity", quantity);
        command.Parameters.AddWithValue("$unitSellPrice", unitSellPrice);
        command.Parameters.AddWithValue("$unitCostPrice", unitCostPrice);
        command.Parameters.AddWithValue("$lineTotal", unitSellPrice * quantity);

        command.ExecuteNonQuery();
    }

    private static string CreateTempDatabasePath()
    {
        return Path.Combine(Path.GetTempPath(), $"mini-market-tests-{Guid.NewGuid():N}.db");
    }

    private static void DeleteIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
