namespace MiniMarketApp.Models;

public sealed class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public decimal UnitSellPrice { get; set; }
    public decimal UnitCostPrice { get; set; }
    public int Quantity { get; set; }
    public int LowStockThreshold { get; set; } = 5;
    public bool IsActive { get; set; } = true;
}
