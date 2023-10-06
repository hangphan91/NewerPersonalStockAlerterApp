using System;
using System.Collections.Generic;
using System.Linq;
using HP.PersonalStocksAlerter.Models.Models;

namespace HP.PersonalStocks.Mgr.Logics
{
    public class AnnualStatisticsCalculator
    {
        public PositionLedgerCalculator PositionLedgerCalculator { get; set; }
        public BalanceHolderCalculator BalanceHolderCalculator { get; set; }
        public List<AnnualStatistics> AnnualStatisticsList { get; set; }

        public AnnualStatisticsCalculator(BalanceHolderCalculator balanceHolderCalculator,
            PositionLedgerCalculator positionLedgerCalculator)
        {
            PositionLedgerCalculator = positionLedgerCalculator;
            BalanceHolderCalculator = balanceHolderCalculator;
            AnnualStatisticsList = new List<AnnualStatistics>();
        }

        public void SetAvailableBalanceByYear(int year, AnnualStatistics currentStatistic)
        {
            if (currentStatistic == null)
                currentStatistic = new AnnualStatistics();

            var sumBalance = BalanceHolderCalculator.GetAllDepositBalanceByYear(year);
            var sumAllBalances = BalanceHolderCalculator.GetAllDepositBalance();
            var sumPositionLedger = PositionLedgerCalculator.GetSumPositionLedgerByYear(year);
            var sumAllPositionLedgers = PositionLedgerCalculator.GetSumOfHoldingPositionLedger();


            currentStatistic.AvalableBalance = sumAllBalances
                - sumAllPositionLedgers;
        }

        public AnnualStatistics GetAnnualStatisticsByYear(int year,
            List<AnnualStatistics> annualStatisticsList)
        {
            AnnualStatisticsList = annualStatisticsList;

            return AnnualStatisticsList
                .Where(statistic => statistic.Year == year)
                .FirstOrDefault();
        }

        public void GetTotalBalanceByYear(decimal currentPrice, decimal shares, AnnualStatistics annualStatistics)
        {
            annualStatistics.TotalBalance =
                annualStatistics.AvalableBalance + (shares * currentPrice);
        }

        public void IsProfitableByYear(AnnualStatistics annualStatistics)
        {
            annualStatistics.IsProfitable = annualStatistics.TotalGainInHoldingPosition > 0;
        }

        public void GetTotalGainPercentageByYear(decimal cost, AnnualStatistics annualStatistics)
        {
            if (cost != 0)
                annualStatistics.TotalGainInPositionPercentage = (annualStatistics.TotalGainInHoldingPosition / cost) * 100;
        }

        public void GetTotalGainByYear(decimal cost, AnnualStatistics annualStatistics)
        {
            annualStatistics.TotalGainInHoldingPosition =
                annualStatistics.TotalBalanceHoldInPosition - cost;
        }

        public void GetCostOfHoldingPositionByYear(decimal cost, AnnualStatistics annualStatistics)
        {
            annualStatistics.CostOfHoldingPosition = cost;
        }

        public void GetTotalBalanceHoldInPosition(decimal currentPrice, decimal shares, AnnualStatistics annualStatistics)
        {
            annualStatistics.TotalBalanceHoldInPosition = shares * currentPrice;
        }

        public void GetAveragePriceByYear(IEnumerable<PositionLedger> ledgersByYear,
            decimal shares, AnnualStatistics annualStatistics)
        {
            if (shares == 0)
                return;
            var ledgerWithAverage = ledgersByYear
            .Where(ledger => ledger.AveragePrice > 0);

            annualStatistics.AveragePrice = !ledgerWithAverage.Any() ? 0 :
                annualStatistics.TotalBalanceHoldInPosition / shares;
        }

        public AnnualStatistics CalculateStatistics(decimal currentPrice, string currentStockSymbol,
            int year, IEnumerable<PositionLedger> ledgersByYear, decimal shares,
            decimal cost)
        {
            var annualStatistics = new AnnualStatistics { Year = year };
            annualStatistics.CurrentStockSymbol = currentStockSymbol;

            GetTotalBalanceByYear(currentPrice, shares, annualStatistics);
            GetTotalBalanceHoldInPosition(currentPrice, shares, annualStatistics);

            GetCostOfHoldingPositionByYear(cost, annualStatistics);
            GetTotalGainByYear(cost, annualStatistics);
            GetTotalGainPercentageByYear(cost, annualStatistics);
            IsProfitableByYear(annualStatistics);

            GetAveragePriceByYear(ledgersByYear, shares, annualStatistics);

            SetAvailableBalanceByYear(year, annualStatistics);

            return annualStatistics;
        }


        public void SetAnnualStatistics(int year, List<AnnualStatistics> annualStatisticsList)
        {
            var currentStatistics = annualStatisticsList.Where(annual => annual.Year == year).FirstOrDefault();
            AnnualStatisticsList = annualStatisticsList;
            SetAvailableBalanceByYear(year, currentStatistics);
        }

        public void GetStatisticByYear(decimal currentPrice, string currentStockSymbol,
            int year, List<AnnualStatistics> annualStatistics)
        {
            var ledgersByYear = PositionLedgerCalculator.GetLedgerByYear(year);
            var shares = PositionLedgerCalculator.GetNumberofShares(ledgersByYear);

            var cost = PositionLedgerCalculator.GetSumPositionLedgerByYear(year);

            var sumBalance = BalanceHolderCalculator.GetAllDepositBalanceByYear(year);

           var currentAnnualStatistic = CalculateStatistics(currentPrice, currentStockSymbol,
                year, ledgersByYear, shares, cost);

            annualStatistics.Add(currentAnnualStatistic);
        }


        public void SetDefaultAnnualStatistics()
        {
            AnnualStatisticsList = new List<AnnualStatistics>();
        }

    }
}

