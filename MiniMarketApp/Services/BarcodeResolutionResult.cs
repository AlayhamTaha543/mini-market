using MiniMarketApp.Models;

namespace MiniMarketApp.Services;

public sealed class BarcodeResolutionResult
{
    public BarcodeResolutionType ResolutionType { get; init; }
    public Product? Product { get; init; }
    public List<Product> Candidates { get; init; } = [];
}
