namespace MiniMarketApp.Models;

public sealed class Loss
{
    public int LossId { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public LossReason Reason { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCostPrice { get; set; }
    public DateTime CreatedAt { get; set; }
}
