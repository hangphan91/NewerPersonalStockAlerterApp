using System;
namespace HP.PersonalStocksAlerter.Models.Models
{
	public class PositionLedger
	{
		public decimal Cost { get; set; }
		public decimal Gain { get; set; }
		public decimal BuyAtGainPercentage { get; set; }
		public decimal SellAtLossPercentage { get; set; }
		public decimal AveragePrice { get; set; }
		public PositionHolder BuyPositionHolder { get; set; }
		public PositionHolder SellPositionHolder { get; set; }

		public PositionLedger()
		{
			BuyPositionHolder = new PositionHolder();
			SellPositionHolder = new PositionHolder();
		}
	}
}

