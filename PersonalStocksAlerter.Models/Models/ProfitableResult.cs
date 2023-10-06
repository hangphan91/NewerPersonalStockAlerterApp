using System;
namespace HP.PersonalStocksAlerter.Models.Models
{
	public class ProfitableResult
	{
        public decimal GainPercentage { get; set; }
        public int NumberOfYearOfAnalysis { get; set; }
        public bool IsProfitable { get; set; }
		public string Symbol { get; set; }

		public ProfitableResult()
		{
		}
	}
}

