using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HP.PersonalStocks.Mgr.Helpers;
using HP.PersonalStocks.Mgr.Logics;
using HP.PersonalStocksAlerter.Models.Models;
using Skender.Stock.Indicators;

namespace HP.PersonalStocks.Mgr
{
    public class StockNaturalSelectionMgr
    {
        public string CurrentStockSymbol { get; set; }
        public GetStockDataHelper GetStockDataHelper { get; set; }
        public List<Quote> HistoricalQuotes { get; set; }
        public int BuyEveryNumberOfDate { get; set; } = 15;
        public int NumberOfYearToLookBack { get; set; }
        public Logics.CalculatorManager CalculatorManager { get; set; }
        public bool IncludeHistoricalProfit { get; set; }

        public StockNaturalSelectionMgr(string currentSymbol, int numberOfYear = 10, bool includeHistoricalProfit = true)
        {
            CurrentStockSymbol = currentSymbol;
            NumberOfYearToLookBack = numberOfYear;
            CalculatorManager = new CalculatorManager();
            IncludeHistoricalProfit = includeHistoricalProfit;
            CalculatorManager.SetCurrentPositionLedgerSummaryAndStatistics(numberOfYear);
        }

        public async Task StartToCalculateAsync()
        {
            await GetHistorialStocksAsync();

            if (HistoricalQuotes.Count == 0)
                return;

            var startDate = DateTime.Now.AddYears(-NumberOfYearToLookBack);

            var boughtMonthsAndYears = new Dictionary<string, bool>();
            foreach (var quote in HistoricalQuotes)
            {
                var exStartDate = startDate;
                string key = (quote.Date.Month.ToString() + quote.Date.Year.ToString());

                if (quote.Date.Day >= startDate.Day && !boughtMonthsAndYears.ContainsKey(key))
                {
                    startDate = quote.Date;
                    boughtMonthsAndYears.Add(key, true);
                    CalculatorManager.DepositAndAddToBalance(exStartDate, startDate);
                }

                BuyStockOnDate(startDate);
            }

            var curentPrice = HistoricalQuotes.OrderByDescending(quote => quote.Date).First().Open;
            CalculatorManager.CalculatOverallStatistic(curentPrice, CurrentStockSymbol, NumberOfYearToLookBack);
        }

        private async Task GetHistorialStocksAsync()
        {
            GetStockDataHelper = new GetStockDataHelper(CurrentStockSymbol);
            await GetStockDataHelper.GetHistoricalQuotesInfoAsync(NumberOfYearToLookBack);
            HistoricalQuotes = GetStockDataHelper.HistoricalQuotes;
        }

        private void BuyStockOnDate(DateTime startDate)
        {
            var currentStockData = GetStockDataByDate(startDate);
            if (currentStockData == null)
                return;

            //if stock still gain more than 10% buy more
            var averagePrice = CalculatorManager.PositionLedgerCalculator.GetAveragePrice();

            var twentyPercentBelowAveragePrice = GetLowerBarrierPrice(averagePrice);
            var tenPercentHigherThanAveragePrice = GetUpperBarrierPrice(averagePrice);

            var currentBalance = CalculatorManager.SetAvailableBalance();

            var toAddPosition = new PositionHolder();
            if (WillSellStocBecauseOfLoss(currentStockData, twentyPercentBelowAveragePrice))
            {
                // if stock lost more than 20% sell all.
                CalculatorManager.PositionLedgerCalculator
                      .PerformSellingStocks(startDate, currentStockData, tenPercentHigherThanAveragePrice);
            }
            else if (WillBuyStockBecauseOfGainning(currentStockData, tenPercentHigherThanAveragePrice))
            {
                //if stock gains more than 10% we buy here
               ResetAvailableBalanceAndAddPositionLedger(startDate,
                    currentStockData, tenPercentHigherThanAveragePrice, currentBalance);
            }
        }

        public PositionHolder ResetAvailableBalanceAndAddPositionLedger(DateTime startDate,
            Quote currentStockData, decimal tenPercentHigherThanAveragePrice, decimal currentBalance)
        {
            PositionHolder toAddPosition = DefineStockToAddToPosition(startDate, currentStockData, currentBalance);
            var cost = toAddPosition.Price * toAddPosition.NumberOfShare;
            CalculatorManager.StatisticsCalculator
                .SetStatisticAvailableBalance(CalculatorManager.StatisticsCalculator.Statistics);
            CalculatorManager.PositionLedgerCalculator.AddLedger(tenPercentHigherThanAveragePrice,
                toAddPosition, cost);
            return toAddPosition;
        }

        private static bool WillBuyStockBecauseOfGainning(Quote currentStockData, decimal tenPercentHigherThanAveragePrice)
        {
            return tenPercentHigherThanAveragePrice < currentStockData.Open;
        }


        private static bool WillSellStocBecauseOfLoss(Quote currentStockData, decimal twentyPercentBelowAveragePrice)
        {
            return twentyPercentBelowAveragePrice > currentStockData.Open;
        }

        private static decimal GetUpperBarrierPrice(decimal averagePrice)
        {
            return averagePrice * (decimal)1.1;
        }

        private static decimal GetLowerBarrierPrice(decimal averagePrice)
        {
            return averagePrice * (decimal)0.8;
        }

        private PositionHolder DefineStockToAddToPosition(DateTime acquiredDate, Quote currentStockData, decimal currentBalance)
        {

            var toAddPosition = new PositionHolder
            {
                AcquiredDate = acquiredDate,
                Price = currentStockData.Open,
                NumberOfShare = currentBalance / currentStockData.Open,
                Symbol = CurrentStockSymbol,
            };

            return toAddPosition;
        }

        private Quote GetStockDataByDate(DateTime date)
        {
            var stockData = HistoricalQuotes
                .Where(historyQuote => historyQuote.Date.Year == date.Year
                && historyQuote.Date.Month == date.Month)
                .Where(quote => quote != null);
            var currentStockData = stockData
                .Where(historyQuote => historyQuote.Date.Day == date.Day)
                .FirstOrDefault();

            if (currentStockData == null)
                currentStockData = stockData.FirstOrDefault();

            return currentStockData;
        }
    }
}

