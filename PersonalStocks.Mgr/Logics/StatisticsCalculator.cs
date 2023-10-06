using System;
using System.Linq;
using HP.PersonalStocksAlerter.Models.Models;

namespace HP.PersonalStocks.Mgr.Logics
{
    public class StatisticsCalculator
    {
        public PositionLedgerCalculator PositionLedgerCalculator { get; set; }
        public BalanceHolderCalculator BalanceHolderCalculator { get; set; }
        public Statistics Statistics { get; set; }

        public StatisticsCalculator(BalanceHolderCalculator balanceHolderCalculator, PositionLedgerCalculator positionLedgerCalculator)
        {
            PositionLedgerCalculator = positionLedgerCalculator;
            BalanceHolderCalculator = balanceHolderCalculator;
            Statistics = new Statistics();
        }

        public void SetStatisticAvailableBalance(Statistics statistics)
        {
            var sumBalance = BalanceHolderCalculator.GetAllDepositBalance();
            var sumOfHoldingPositionLedger = PositionLedgerCalculator.GetSumOfHoldingPositionLedger();

            var avalableBalance = sumBalance - sumOfHoldingPositionLedger;
            Statistics = statistics;
            statistics.AvalableBalance = avalableBalance;
        }

        public void SetDefaultStatistics()
        {
            Statistics = new Statistics();
        }

        public void GetTotalBalance(decimal currentPrice, decimal allShares)
        {
            Statistics.TotalBalance = Statistics.AvalableBalance + (allShares * currentPrice);
        }

        public void GetTotalBalanceHoldInPositions(decimal currentPrice, decimal allShares)
        {
            Statistics.TotalBalanceHoldInPosition = (allShares * currentPrice);
        }

        public void GetCostOfHoldingPositions(decimal cost)
        {
            Statistics.CostOfHoldingPosition = cost;
        }

        public void GetTotalGainInPosition(decimal cost, decimal lostFromAllSell)
        {
            Statistics.TotalGainInHoldingPosition = Statistics.TotalBalanceHoldInPosition - cost - lostFromAllSell;
        }

        public void GetTotalGainInPositionPercentage(decimal cost)
        {
            if (cost != 0)
                Statistics.TotalGainInPositionPercentage = (Statistics.TotalGainInHoldingPosition / cost) * 100;
        }

        public void IsProfitable()
        {
            Statistics.IsProfitable = Statistics.OverallGain > 0;
        }

        public void SetCurrentSymbol(string currentStockSymbol)
        {
            Statistics.CurrentStockSymbol = currentStockSymbol;
        }

        public void GetAvaragePrice(decimal allShares)
        {
            if (allShares == 0)
                return;

            var ledgerWithAverage = PositionLedgerCalculator.GetLedgersWithAveragePrice();

            Statistics.AveragePrice = !ledgerWithAverage.Any() ? 0 :
                Statistics.TotalBalanceHoldInPosition / allShares;
        }

        private void GetOverallGain(decimal lostFromAllSell)
        {
            Statistics.OverallGain = Statistics.TotalGainInHoldingPosition + lostFromAllSell;
        }

        private void GetOverallGainPercentage()
        {
            var allDeposit = BalanceHolderCalculator.GetAllDepositBalance();
            if (allDeposit > 0)
            Statistics.OverallGainPercentage = Statistics.OverallGain * 100
                    / allDeposit;
        }

        public void GetStatisticsForCurrentStockSymbol(decimal currentPrice, string currentStockSymbol,
            decimal allShares, decimal cost, decimal lostFromAllSell)
        {
            GetTotalBalance(currentPrice, allShares);
            GetTotalBalanceHoldInPositions(currentPrice, allShares);
            GetCostOfHoldingPositions(cost);
            GetTotalGainInPosition(cost, lostFromAllSell);
            GetTotalGainInPositionPercentage(cost);
            SetCurrentSymbol(currentStockSymbol);
            GetAvaragePrice(allShares);
            GetOverallGain(lostFromAllSell);
            GetOverallGainPercentage();
            IsProfitable();

        }

        public decimal GetAvailableBalance()
        {
            return Statistics.AvalableBalance;
        }
    }
}

