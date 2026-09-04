using MiniMarketApp.Models;

namespace MiniMarketApp.Data.Repositories;

public interface ISaleRepository
{
    int CreateSale(Sale sale, IReadOnlyList<SaleItem> items);
}
