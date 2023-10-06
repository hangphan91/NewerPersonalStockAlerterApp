using System;
namespace HP.PersonalStocksAlerter.Models.Models
{
	public class NatualSelectionResult
	{
        private string _message;
        private PositionLedgerSummary positionLedgerSummary;

        public string Message { get; set; }
		public PositionLedgerSummary PositionLedgerSummary { get; set; }

		public NatualSelectionResult()
		{
		}

        public NatualSelectionResult(string message, PositionLedgerSummary positionLedgerSummary)
        {
            this.Message = message;
            this.PositionLedgerSummary = positionLedgerSummary;
        }
    }
}

