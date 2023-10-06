using System;
namespace HP.PersonalStocksAlerter.Models.Models
{
	public class PositionHolder
	{
		public decimal Price { get; set; }
		public decimal NumberOfShare { get; set; }
		public DateTime AcquiredDate { get; set; }
        public DateTime SoldDate { get; set; }
        public string Symbol { get; set; }
		public PositionHolder()
		{
			
		}
	}
}

