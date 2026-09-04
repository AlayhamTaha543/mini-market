namespace MiniMarketApp.Models;

public sealed class Sale
{
    public int SaleId { get; set; }
    public int UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalCost { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}
