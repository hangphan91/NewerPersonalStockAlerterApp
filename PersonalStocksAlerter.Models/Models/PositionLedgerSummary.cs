using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HP.PersonalStocksAlerter.Models.Models
{
	public class PositionLedgerSummary
	{
		public PositionLedgerSummary()
		{
            PositionLedgers = new List<PositionLedger>();
            BalanceHolders = new List<BalanceHolder>();
            Statistics = new Statistics();
            AnnualStatistics = new List<AnnualStatistics>();
		}
        [JsonIgnore]
        public List<PositionLedger> PositionLedgers { get; set; }
        [JsonIgnore]
        public List<BalanceHolder> BalanceHolders { get; set; }

        public int NumberOfYearToLookBack { get; set; } = 10;
        public int DepositDateOfMonth { set; get; } = 5;
        public int DepositAmountMonthly { set; get; } = 2000;
        public Statistics Statistics { get; set; }
        public List<AnnualStatistics> AnnualStatistics { get; set; }
        public decimal CurrentPrice { get; set; }
    }
}

