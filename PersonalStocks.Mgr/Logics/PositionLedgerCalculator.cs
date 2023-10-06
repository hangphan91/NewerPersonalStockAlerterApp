using System;
using System.Collections.Generic;
using System.Linq;
using HP.PersonalStocksAlerter.Models.Models;
using Skender.Stock.Indicators;

namespace HP.PersonalStocks.Mgr.Logics
{
	public class PositionLedgerCalculator
	{
        public List<PositionLedger> PositionLedgers { get; set; }

        public PositionLedgerCalculator(List<PositionLedger> positionLedgers)
		{
            PositionLedgers = positionLedgers;
		}

        public decimal GetSumOfHoldingPositionLedger()
        {
            return PositionLedgers
                .Sum(ledger => (ledger.BuyPositionHolder.Price * ledger.BuyPositionHolder.NumberOfShare));
        }

        public decimal GetSumPositionLedgerByYear(int year)
        {
            var ledgersByYear = PositionLedgers
                             .Where(ledger => ledger.BuyPositionHolder.AcquiredDate.Year == year);
            return ledgersByYear
                 .Sum(ledger => (ledger.BuyPositionHolder.Price * ledger.BuyPositionHolder.NumberOfShare));
        }

        public IEnumerable<PositionLedger> GetLedgerByYear(int year)
        {
            return PositionLedgers
                                .Where(ledger => ledger.BuyPositionHolder.AcquiredDate.Year == year ||
                                        ledger.SellPositionHolder.SoldDate.Year == year);
        }

        public decimal GetNumberofShares(IEnumerable<PositionLedger> ledgersByYear)
        {
            return ledgersByYear
                .Sum(ledger => ledger.BuyPositionHolder.NumberOfShare -
                ledger.SellPositionHolder.NumberOfShare);
        }

        public decimal GetAllShares()
        {
            return PositionLedgers
                .Sum(ledger => ledger.BuyPositionHolder.NumberOfShare - ledger.SellPositionHolder.NumberOfShare);
        }

        public decimal GetLossFromAllSoldPositions()
        {
            return PositionLedgers.Sum(ledger => ledger.Gain);
        }

        public void SetDefaultPositionLedgers()
        {
           PositionLedgers = new List<PositionLedger>();
        }

        public IEnumerable<PositionLedger> GetLedgersWithAveragePrice()
        {
            var ledgerWithAverage = PositionLedgers
                .Where(ledger => ledger.AveragePrice > 0);

            return ledgerWithAverage;
        }

        public decimal GetAveragePrice()
        {
            decimal averagePrice = 0;

            if (PositionLedgers.Any())
            {
                var positionLedgers =
                    PositionLedgers.Where(ledger => ledger.AveragePrice > 0);

                if (positionLedgers.Any())
                    averagePrice = positionLedgers.Average(ledger => ledger.BuyPositionHolder.Price);
            }

            return averagePrice;
        }

        public void PerformSellingStocks(DateTime startDate, Quote currentStockData,
           decimal tenPercentHigherThanAveragePrice)
        {
            var ledgers = PositionLedgers;

            foreach (var ledger in ledgers)
            {
                var boughtPosition = ledger.BuyPositionHolder;
                ledger.SellPositionHolder = new PositionHolder
                {
                    SoldDate = startDate,
                    NumberOfShare = boughtPosition.NumberOfShare,
                    Price = currentStockData.Open,
                    Symbol = boughtPosition.Symbol,
                    AcquiredDate = boughtPosition.AcquiredDate,
                };
                ledger.SellAtLossPercentage = tenPercentHigherThanAveragePrice;
                ledger.AveragePrice = 0;
                ledger.Gain = (currentStockData.Open - boughtPosition.Price) * boughtPosition.NumberOfShare;
                //CalculatorManager.StatisticsCalculator.Statistics.AvalableBalance += ledger.Gain;

            }
        }

        public void AddPositionLedger(PositionLedger positionLedger)
        {
            PositionLedgers.Add(positionLedger);
        }

        public void AddLedger(decimal tenPercentHigherThanAveragePrice, PositionHolder toAddPosition, decimal cost)
        {
            var ledger = new PositionLedger
            {
                BuyPositionHolder = toAddPosition,
                Cost = cost,
                BuyAtGainPercentage = tenPercentHigherThanAveragePrice,
                AveragePrice = toAddPosition.Price,
            };
            AddPositionLedger(ledger);
        }

    }
}

