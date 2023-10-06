using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HP.PersonalStocks.Mgr.Factories;
using HP.PersonalStocks.Mgr.Helpers;
using HP.PersonalStocksAlerter.Models.Models;
using Newtonsoft.Json;
using NodaTime;
using Skender.Stock.Indicators;
using Trady.Analysis.Indicator;
using YahooFinanceApi;
using YahooQuotesApi;

namespace HP.PersonalStocks.Mgr
{
    public class AlertMgr
    {
        public List<Quote> HistoricalQuotes { get; set; }
        public List<StdDevResult> StdDevResults { get; set; }
        public string CurrentSticker { get; set; }
        public AlertFactory Factory { get; set; }
        public decimal ExceptedLowPercentage { get; set; }
        public decimal ExceptedHighPercentage { get; set; }
        public Quote CurrentQuote { get; set; }
        public List<RsiResult> RsiResults { get; set; }
        public List<ChaikinOscResult> ChaikinOscResults { get; set; }
        public GetStockDataHelper GetStockDataHelper { get; set; }
        public AlertMgr(string currentSticker)
        {
            GetStockDataHelper = new GetStockDataHelper(currentSticker);
            CurrentSticker = currentSticker;
            RsiResults = new List<RsiResult>();
            ChaikinOscResults = new List<ChaikinOscResult>();
            HistoricalQuotes = new List<Quote>();
        }

        public async Task GetStockDataAsync()
        {
            await GetStockDataFromYahooApiAsync();
            CurrentQuote = HistoricalQuotes.OrderByDescending(q => q.Date).FirstOrDefault();
            Factory = new AlertFactory(StdDevResults, HistoricalQuotes);
        }

        private async Task GetStockDataFromYahooApiAsync()
        {
            await GetStockDataHelper.GetQuoteAndStdIndicatorAsync();

            StdDevResults = GetStockDataHelper.StdDevResults;
            HistoricalQuotes = GetStockDataHelper.HistoricalQuotes;
            ChaikinOscResults = GetStockDataHelper.ChaikinOscResults;
            RsiResults = GetStockDataHelper.RsiResults;
        }

        public async Task<AlertResult> GetAlertResultAsync()
        {
            var result = new AlertResult();
            try
            {
                result = new AlertResult
                {
                    SuggestionMessage = await GetSuggestionForCurrentStickerAsync(),
                    SuggestedActions = new List<string> {
                       // CheckForAlert().ToString(),
                        CheckForSecondAlert().ToString() },
                    Success = true,
                    Symbol = CurrentSticker,
                    CurrentRSIValue = GetRSIIndicator(),
                    CurrentChaikinOSCValue = GetChaikinOSCValue()
                };
            }
            catch (Exception ex)
            {
                result = new AlertResult
                {
                    Success = false,
                    ErrorMessage = "Failed to get Alert and Suggestion." + ex.Message
                };
            }
            finally
            {
                if (result.Success)
                {
                    var log = new LogResult
                    {
                        SuggestedActions = string.Join(", ", result.SuggestedActions),
                        SuggestionMessage = result.SuggestionMessage,
                        PostedTS = DateTime.Now,
                        CurrentPrice = CurrentQuote?.Close.ToString(),
                        StockSymbol = CurrentSticker,
                        CurrentRsiValue = result.CurrentRSIValue,
                        CurrentOSCValue = result.CurrentChaikinOSCValue
                    };
                    new WriteToText(log);
                }
            }
            return result;
        }

        private string GetChaikinOSCValue()
        {
            var instruction = $"Buy When OSC Positive. Sell When OSC Negative.";
            var osdValue = ChaikinOscResults.LastOrDefault().Oscillator.Value;
            var isPossitive = osdValue > 0;
            return $"{(isPossitive ? "Positive" : "Negative")} ({instruction})";
        }

        private string GetRSIIndicator()
        {
            var instruction = "Buy When RSI < 30. Sell When RSI > 70.";
            var rsiValue = Math.Round(RsiResults.LastOrDefault().Rsi.Value, 2).ToString();
            return $"{rsiValue} ({instruction})";
        }

        private SuggestedAction CheckForAlert()
        {
            var currentPrice = HistoricalQuotes.OrderByDescending(q => q.Date).FirstOrDefault();
            if (currentPrice?.Close >= Factory.Calculator.SellingSuggestion.LowLimit)
                return SuggestedAction.Sell;
            else if (currentPrice?.Close < Factory.Calculator.SellingSuggestion.LowLimit && SendHighLimit())
                return SuggestedAction.WaitToSell;
            else if (Factory.Calculator.BuyingSuggestion.HighLimit >= currentPrice?.Close)
                return SuggestedAction.Buy;
            else if (Factory.Calculator.BuyingSuggestion.HighLimit < currentPrice?.Close && !SendHighLimit())
                return SuggestedAction.WaitToBuy;
            return SuggestedAction.Wait;

        }
        private SuggestedAction CheckForSecondAlert()
        {
            var currentPrice = HistoricalQuotes.OrderByDescending(q => q.Date).FirstOrDefault();
            if (currentPrice?.Close <= Factory.SecondCalculator.SellingSuggestion.LowLimit)
                return SuggestedAction.StrongSell;
            else if (currentPrice?.Close >= Factory.SecondCalculator.SellingSuggestion.HighLimit)
                return SuggestedAction.Sell;
            else if (currentPrice?.Close >= Factory.SecondCalculator.HoldOrSellSuggestion.LowLimit &&
                currentPrice?.Close < Factory.SecondCalculator.HoldOrSellSuggestion.HighLimit)
                return SuggestedAction.HoldPriceCouldGoUp;
            else if (Factory.SecondCalculator.HoldOrBuySuggestion.HighLimit >= currentPrice?.Close &&
                currentPrice?.Close > Factory.SecondCalculator.HoldOrBuySuggestion.LowLimit)
                return SuggestedAction.SellPriceCouldGoDown;
            else if (Factory.SecondCalculator.BuyingSuggestion.HighLimit >= currentPrice?.Close &&
                Factory.SecondCalculator.BuyingSuggestion.LowLimit < currentPrice?.Close)
            {
                if (currentPrice?.Close < Factory.SecondCalculator.FiveBasicNumber.Mean)
                    return SuggestedAction.StrongBuy;
                return SuggestedAction.Buy;
            }
            else return SuggestedAction.Wait;
        }

        private async Task<string> GetSuggestionForCurrentStickerAsync()
        {
            await GetStockDataFromYahooApiAsync();
            var stdAlertHighLimit = new AlertInfo(2, 10);
            var stdAlertlLowLimit = new AlertInfo(2, 10);
            //var suggestionResult = Factory.GetSuggestion(stdAlertHighLimit, stdAlertlLowLimit);
            var secondSuggestionResult = Factory.GetSecondSuggestion();
            var currentPrice = Math.Round(CurrentQuote.Close, 2);
            var suggestion = $"{CurrentSticker}'s Current Price: {currentPrice}." +
                // $"1st Suggestion: {suggestionResult}\n" + 
                $"Our 2nd Suggestion: {secondSuggestionResult}";
            return suggestion;
        }

        private bool SendHighLimit()
        {
            var quoteAveragePrice = HistoricalQuotes.Average(s => s.Close);

            var currentQuotePrice = HistoricalQuotes
                .Where(q => q.Date.Date <= DateTime.Now.Date)
                .OrderByDescending(q => q.Date)
                .FirstOrDefault();
            var inHighRange = currentQuotePrice != null && currentQuotePrice.Close >= quoteAveragePrice;
            return inHighRange;
        }

    }
}
