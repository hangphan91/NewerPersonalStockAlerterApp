using System;
using System.Collections.Generic;
using System.Linq;

namespace HP.PersonalStocksAlerter.Models.Models
{
    public class MultiNatualSelectionResult
    {
        public List<ProfitableResult> ProfitableResults { get; set; }

        public string Message { get; set; }
        public MultiNatualSelectionResult(string message)
        {
            Message = message;
            ProfitableResults = new List<ProfitableResult>();
        }

        public MultiNatualSelectionResult()
        {

        }

        public MultiNatualSelectionResult Convert(List<NatualSelectionResult> responses)
        {
            var result = responses.Select(selectionResult =>
            new ProfitableResult
            {
                NumberOfYearOfAnalysis = selectionResult.PositionLedgerSummary.NumberOfYearToLookBack,
                GainPercentage = selectionResult.PositionLedgerSummary.Statistics.OverallGainPercentage,
                IsProfitable = selectionResult.PositionLedgerSummary.Statistics.IsProfitable,
                Symbol = selectionResult.PositionLedgerSummary.Statistics.CurrentStockSymbol,
            }).ToList();

            return new MultiNatualSelectionResult
            {
                ProfitableResults = result,
            };
        }
    }
}

