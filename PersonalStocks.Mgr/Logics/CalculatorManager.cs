using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using HP.PersonalStocksAlerter.Models.Models;

namespace HP.PersonalStocks.Mgr.Logics
{
    public class CalculatorManager
    {
        private PositionLedgerSummary _positionLedgerSummary;

        public BalanceHolderCalculator BalanceHolderCalculator { get; set; }
        public PositionLedgerCalculator PositionLedgerCalculator { get; set; }
        public AnnualStatisticsCalculator AnnualStatisticsCalculator { get; set; }
        public StatisticsCalculator StatisticsCalculator { get; set; }
        public PositionLedgerSummaryCalculator PositionLedgerSummaryCalculator { get; set; }

        public CalculatorManager()
        {
            _positionLedgerSummary = new PositionLedgerSummary();
            SetUpCalculators();
        }

        private void SetUpCalculators()
        {
            BalanceHolderCalculator = new BalanceHolderCalculator(_positionLedgerSummary.BalanceHolders);
            PositionLedgerCalculator = new PositionLedgerCalculator(_positionLedgerSummary.PositionLedgers);
            AnnualStatisticsCalculator = new AnnualStatisticsCalculator(BalanceHolderCalculator, PositionLedgerCalculator);
            StatisticsCalculator = new StatisticsCalculator(BalanceHolderCalculator, PositionLedgerCalculator);
            PositionLedgerSummaryCalculator = new PositionLedgerSummaryCalculator(_positionLedgerSummary, StatisticsCalculator);
        }

        public decimal SetAvailableBalance(Statistics statistics = null,
            List<AnnualStatistics> annualStatisticsList = null)
        {
            if (statistics == null)
                statistics = _positionLedgerSummary.Statistics;

            if (annualStatisticsList == null)
                annualStatisticsList = _positionLedgerSummary.AnnualStatistics;

            StatisticsCalculator.SetStatisticAvailableBalance(statistics);

            var years = BalanceHolderCalculator.GetYears().Where(year => year > 2000);

            foreach (var year in years)
            {
                AnnualStatisticsCalculator.SetAnnualStatistics(year, annualStatisticsList);
            }
            return StatisticsCalculator.Statistics.AvalableBalance;
        }

        public void CalculateAnnualRevenue(decimal currentPrice, string currentStockSymbol)
        {
            var years = BalanceHolderCalculator.GetYears();

            foreach (var year in years)
                AnnualStatisticsCalculator.GetStatisticByYear(currentPrice, currentStockSymbol,
                               year, _positionLedgerSummary.AnnualStatistics);
        }

        public void CalculatOverallStatistic(decimal currentPrice,
            string currentStockSymbol, int numberOfYearToLookBack)
        {
            //  SetUpCalculators();
            var allShares = PositionLedgerCalculator.GetAllShares();

            var cost = PositionLedgerCalculator.GetSumOfHoldingPositionLedger();
            var sumBalance = BalanceHolderCalculator.GetAllDepositBalance();
            decimal lostFromAllSell = PositionLedgerCalculator.GetLossFromAllSoldPositions();

            PositionLedgerSummaryCalculator
                 .CalculatePositionLedgerSummary(currentPrice, currentStockSymbol,
                 numberOfYearToLookBack, allShares, cost, lostFromAllSell);

            CalculateAnnualRevenue(currentPrice, currentStockSymbol);
        }

        public void SetCurrentPositionLedgerSummaryAndStatistics(int numberOfYear)
        {
            BalanceHolderCalculator.SetDefaultBalanceHolders(numberOfYear);
            PositionLedgerCalculator.SetDefaultPositionLedgers();

            StatisticsCalculator.SetDefaultStatistics();
            AnnualStatisticsCalculator.SetDefaultAnnualStatistics();

            SetAvailableBalance(_positionLedgerSummary.Statistics,
                _positionLedgerSummary.AnnualStatistics);
        }

        public void DepositAndAddToBalance(DateTime exStartDate, DateTime startDate)
        {
            DateTime depositDate = GetDepositDate(startDate);

            if (IsQualifiedToAddDepositToCurrentBalance(exStartDate, startDate, depositDate))
            {
                BalanceHolderCalculator.SetBalanceHolders(startDate,
                    _positionLedgerSummary.DepositAmountMonthly, _positionLedgerSummary.BalanceHolders);
                SetAvailableBalance(_positionLedgerSummary.Statistics, _positionLedgerSummary.AnnualStatistics);
            }
        }

        private static bool IsQualifiedToAddDepositToCurrentBalance(DateTime exStartDate, DateTime startDate, DateTime depositDate)
        {
            return true;
           // return (exStartDate <= depositDate && depositDate < startDate) || startDate == exStartDate;
        }

        private DateTime GetDepositDate(DateTime startDate)
        {
            return new DateTime(startDate.Year,
                                               startDate.Month,
                                               _positionLedgerSummary.DepositDateOfMonth,
                                               startDate.Hour,
                                               startDate.Minute,
                                               startDate.Second);
        }

        public PositionLedgerSummary GetPositionLedgerSummary()
        {
            return new PositionLedgerSummary
            {
                AnnualStatistics = AnnualStatisticsCalculator.AnnualStatisticsList,
                DepositAmountMonthly = _positionLedgerSummary.DepositAmountMonthly,
                BalanceHolders = BalanceHolderCalculator.BalanceHolders,
                CurrentPrice = _positionLedgerSummary.CurrentPrice,
                DepositDateOfMonth = _positionLedgerSummary.DepositDateOfMonth,
                NumberOfYearToLookBack = _positionLedgerSummary.NumberOfYearToLookBack,
                PositionLedgers = PositionLedgerCalculator.PositionLedgers,
                Statistics = StatisticsCalculator.Statistics
            };
        }
    }
}

