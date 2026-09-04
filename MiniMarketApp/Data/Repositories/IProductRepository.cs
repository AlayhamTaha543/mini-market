using MiniMarketApp.Models;

namespace MiniMarketApp.Data.Repositories;

public interface IProductRepository
{
    int Add(Product product);
    Product? GetById(int productId);
    List<Product> GetByBarcode(string barcode);
    List<Product> GetLowStockProducts();
    void Update(Product product);
    void SoftDelete(int productId);
}
