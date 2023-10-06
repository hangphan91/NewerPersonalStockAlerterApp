using System;
using NodaTime;
using Skender.Stock.Indicators;
using System.Collections.Generic;
using YahooQuotesApi;
using HP.PersonalStocks.Mgr.Factories;
using System.Linq;
using System.Threading.Tasks;

namespace HP.PersonalStocks.Mgr.Helpers
{
    public class GetStockDataHelper
    {

        public List<Quote> HistoricalQuotes { get; set; }
        public List<StdDevResult> StdDevResults { get; set; }
        public string CurrentSticker { get; set; }
        public List<RsiResult> RsiResults { get; set; }
        public List<ChaikinOscResult> ChaikinOscResults { get; set; }

        public GetStockDataHelper(string currentTicker)
        {
            CurrentSticker = currentTicker;
        }

        public async Task GetHistoricalQuotesInfoAsync(int numberOfYearFromNow = 1)
        {
            try
            {
                HistoricalQuotes = new List<Quote>();

                var dateStartToLookUp = DateTime.Now.AddYears(-numberOfYearFromNow);
                var quotes = new YahooQuotesBuilder()
                                .WithHistoryStartDate(Instant.FromUtc(
                                                        dateStartToLookUp.Year,
                                                        1,
                                                        1,
                                                        0,
                                                        0,
                                                        0))
                                .Build();

                var result = await quotes.GetAsync(CurrentSticker, Histories.PriceHistory);

                if (result?.PriceHistory == null || !result.PriceHistory.HasValue)
                    return;

                var priceHistory = result.PriceHistory.Value;
                foreach (var item in priceHistory)
                {
                    var date = item.Date.ToDateTimeUnspecified();
                    HistoricalQuotes.Add(new Quote
                    {
                        Close = (decimal)item.Close,
                        Open = (decimal)item.Open,
                        High = (decimal)item.High,
                        Low = (decimal)item.Low,
                        Volume = item.Volume,
                        Date = date,
                    });
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to Get Historical Quotes Info. {ex.Message}");
            }
        }

        public async Task GetQuoteAndStdIndicatorAsync()
        {
            await GetHistoricalQuotesInfoAsync();
            StdDevResults = Indicator.GetStdDev(HistoricalQuotes, 10).ToList();
            if (HistoricalQuotes.Count > 114)
            {
                RsiResults = Indicator.GetRsi(HistoricalQuotes).ToList();
                ChaikinOscResults = Indicator.GetChaikinOsc(HistoricalQuotes).ToList();
            }
        }
    }
}

