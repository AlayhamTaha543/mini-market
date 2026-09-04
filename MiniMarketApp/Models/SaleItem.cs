namespace MiniMarketApp.Models;

public sealed class SaleItem
{
    public int SaleItemId { get; set; }
    public int SaleId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitSellPrice { get; set; }
    public decimal UnitCostPrice { get; set; }
    public decimal LineTotal { get; set; }
}
