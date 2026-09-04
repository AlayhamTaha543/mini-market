using Microsoft.Data.Sqlite;
using MiniMarketApp.Models;

namespace MiniMarketApp.Data.Repositories;

public sealed class SaleRepository : ISaleRepository
{
    private readonly DatabaseContext _databaseContext;

    public SaleRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public int CreateSale(Sale sale, IReadOnlyList<SaleItem> items)
    {
        using var connection = _databaseContext.CreateConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            var saleId = InsertSale(connection, transaction, sale);

            foreach (var item in items)
            {
                InsertSaleItem(connection, transaction, saleId, item);
                DeductProductStock(connection, transaction, item.ProductId, item.Quantity);
            }

            transaction.Commit();
            return saleId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static int InsertSale(SqliteConnection connection, SqliteTransaction transaction, Sale sale)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = @"
INSERT INTO Sales (UserId, TotalAmount, TotalCost, Discount, IsActive)
VALUES ($userId, $totalAmount, $totalCost, $discount, $isActive);
SELECT last_insert_rowid();";

        command.Parameters.AddWithValue("$userId", sale.UserId);
        command.Parameters.AddWithValue("$totalAmount", sale.TotalAmount);
        command.Parameters.AddWithValue("$totalCost", sale.TotalCost);
        command.Parameters.AddWithValue("$discount", sale.Discount);
        command.Parameters.AddWithValue("$isActive", sale.IsActive ? 1 : 0);

        return Convert.ToInt32((long)command.ExecuteScalar()!);
    }

    private static void InsertSaleItem(SqliteConnection connection, SqliteTransaction transaction, int saleId, SaleItem item)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = @"
INSERT INTO SaleItems (SaleId, ProductId, ProductName, Quantity, UnitSellPrice, UnitCostPrice, LineTotal)
VALUES ($saleId, $productId, $productName, $quantity, $unitSellPrice, $unitCostPrice, $lineTotal);";

        command.Parameters.AddWithValue("$saleId", saleId);
        command.Parameters.AddWithValue("$productId", item.ProductId);
        command.Parameters.AddWithValue("$productName", item.ProductName);
        command.Parameters.AddWithValue("$quantity", item.Quantity);
        command.Parameters.AddWithValue("$unitSellPrice", item.UnitSellPrice);
        command.Parameters.AddWithValue("$unitCostPrice", item.UnitCostPrice);
        command.Parameters.AddWithValue("$lineTotal", item.LineTotal);

        command.ExecuteNonQuery();
    }

    private static void DeductProductStock(SqliteConnection connection, SqliteTransaction transaction, int productId, int quantity)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = @"
UPDATE Products
SET Quantity = Quantity - $quantity,
    UpdatedAt = datetime('now')
WHERE ProductId = $productId
  AND IsActive = 1
  AND Quantity >= $quantity;";

        command.Parameters.AddWithValue("$quantity", quantity);
        command.Parameters.AddWithValue("$productId", productId);

        var updatedRows = command.ExecuteNonQuery();
        if (updatedRows != 1)
        {
            throw new InsufficientStockException("Insufficient stock for one or more products.");
        }
    }
}
