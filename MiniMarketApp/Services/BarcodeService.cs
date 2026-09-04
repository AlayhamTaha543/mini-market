using MiniMarketApp.Data.Repositories;

namespace MiniMarketApp.Services;

public sealed class BarcodeService
{
    private readonly IProductRepository _productRepository;

    public BarcodeService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public BarcodeResolutionResult ResolveScan(string barcode)
    {
        var matches = _productRepository.GetByBarcode(barcode);
        if (matches.Count == 0)
        {
            return new BarcodeResolutionResult
            {
                ResolutionType = BarcodeResolutionType.NoMatch,
            };
        }

        if (matches.Count == 1)
        {
            return new BarcodeResolutionResult
            {
                ResolutionType = BarcodeResolutionType.SingleMatch,
                Product = matches[0],
                Candidates = matches,
            };
        }

        return new BarcodeResolutionResult
        {
            ResolutionType = BarcodeResolutionType.MultipleMatches,
            Candidates = matches,
        };
    }
}
