using MiniMarketApp.Data.Repositories;
using MiniMarketApp.Models;

namespace MiniMarketApp.Services;

public sealed class StockAlertService
{
    private readonly IProductRepository _productRepository;

    public StockAlertService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public List<Product> GetLowStockAlerts()
    {
        return _productRepository.GetLowStockProducts();
    }
}
