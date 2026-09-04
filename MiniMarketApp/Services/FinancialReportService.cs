using MiniMarketApp.Data;

namespace MiniMarketApp.Services;

public sealed class FinancialReportService
{
    private readonly DatabaseContext _databaseContext;

    public FinancialReportService(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public FinancialSummary GetSummary()
    {
        using var connection = _databaseContext.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
SELECT
COALESCE((SELECT SUM(LineTotal) FROM SaleItems), 0) - COALESCE((SELECT SUM(Quantity * UnitCostPrice) FROM SaleItems), 0) AS GrossProfit,
COALESCE((SELECT SUM(Quantity * UnitCostPrice) FROM Losses), 0) AS LossValue;";

        using var reader = command.ExecuteReader();
        reader.Read();

        var grossProfit = reader.GetDecimal(0);
        var lossValue = reader.GetDecimal(1);

        return new FinancialSummary
        {
            GrossProfit = grossProfit,
            LossValue = lossValue,
            NetProfit = grossProfit - lossValue,
        };
    }
}
