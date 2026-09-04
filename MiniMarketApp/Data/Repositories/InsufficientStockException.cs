namespace MiniMarketApp.Data.Repositories;

public sealed class InsufficientStockException : Exception
{
    public InsufficientStockException(string message)
        : base(message)
    {
    }
}
