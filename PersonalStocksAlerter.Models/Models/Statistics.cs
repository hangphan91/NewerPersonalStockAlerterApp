using System;
namespace HP.PersonalStocksAlerter.Models.Models
{
    public class Statistics
    {
        public Statistics() { }

        public string CurrentStockSymbol { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal AvalableBalance { get; set; }
        public decimal CostOfHoldingPosition { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal TotalGainInHoldingPosition { get; set; }
        public decimal TotalGainInPositionPercentage { get; set; }
        public decimal TotalBalanceHoldInPosition { get; set; }
        public decimal OverallGain { get; set; }
        public decimal OverallGainPercentage { get; set; }
        public bool IsProfitable { get; set; }

    }
}

