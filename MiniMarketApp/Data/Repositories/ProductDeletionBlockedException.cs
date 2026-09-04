namespace MiniMarketApp.Data.Repositories;

public sealed class ProductDeletionBlockedException : Exception
{
    public ProductDeletionBlockedException(string message)
        : base(message)
    {
    }
}
