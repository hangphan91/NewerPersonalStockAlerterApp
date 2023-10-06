using HP.PersonalStocks.Mgr.Logics;
using HP.PersonalStocksAlerter.Models.Models;

namespace UnitTests;

public class TestCalculator
{
    [Fact]
    public void TestSetAvailableBalance()
    {

        var stockNatualSelectionCalculator = new StockNatualSelectionCalculator();
        var monthlyDeposit = stockNatualSelectionCalculator.PositionLedgerSummary.DepositAmountMonthly;
        stockNatualSelectionCalculator.PositionLedgerSummary.BalanceHolders.Add(new BalanceHolder
        {
            Amount = monthlyDeposit,
            Date = DateTime.Now,
        });
        stockNatualSelectionCalculator.SetAvailableBalance();

        Assert.Equal(stockNatualSelectionCalculator.PositionLedgerSummary.Statistics.AvalableBalance, monthlyDeposit);
    }
}
