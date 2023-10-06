using System;
using System.Linq;
using HP.PersonalStocksAlerter.Models.Models;

namespace HP.PersonalStocks.Mgr.Logics
{
	public class PositionLedgerSummaryCalculator
	{
        public PositionLedgerSummary PositionLedgerSummary { get; set; }
        public StatisticsCalculator StatisticsCalculator { get; set; }

        public PositionLedgerSummaryCalculator(PositionLedgerSummary positionLedgerSummary, StatisticsCalculator statisticsCalculator)
		{
            PositionLedgerSummary = positionLedgerSummary;
            StatisticsCalculator = statisticsCalculator;
		}

        public void CalculatePositionLedgerSummary(decimal currentPrice,
            string currentStockSymbol, int numberOfYearToLookBack,
            decimal allShares, decimal cost, decimal lostFromAllSell)
        {
            SetCurrentPrice(currentPrice);
            SetNumberOfYearToLookBack(numberOfYearToLookBack);

            StatisticsCalculator.GetStatisticsForCurrentStockSymbol(currentPrice,
                currentStockSymbol, allShares, cost, lostFromAllSell);
        }

        private void SetNumberOfYearToLookBack(int numberOfYearToLookBack)
        {
            PositionLedgerSummary.NumberOfYearToLookBack = numberOfYearToLookBack;
        }

        private void SetCurrentPrice(decimal currentPrice)
        {
            PositionLedgerSummary.CurrentPrice = currentPrice;
        }
    }
}

