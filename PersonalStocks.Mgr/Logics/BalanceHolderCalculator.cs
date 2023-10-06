using System;
using HP.PersonalStocksAlerter.Models.Models;
using System.Collections.Generic;
using System.Linq;

namespace HP.PersonalStocks.Mgr.Logics
{
	public class BalanceHolderCalculator
	{
		public List<BalanceHolder> BalanceHolders { get; set; }
		public BalanceHolderCalculator(List<BalanceHolder> balanceHolders)
		{
			BalanceHolders = balanceHolders;
		}

		public IEnumerable<int> GetYears()
		{
			return BalanceHolders.Select(ledger => ledger.Date)
				.Select(date => date.Year).Distinct();
		}

        public decimal GetAllDepositBalance()
        {
            return BalanceHolders.Sum(balance => balance.Amount);
        }

        public decimal GetAllDepositBalanceByYear(int year)
        {
            return BalanceHolders
                .Where(ballance => ballance.Date.Year == year)
                .Sum(balance => balance.Amount);
        }

        public void SetDefaultBalanceHolders(int numberOfYear)
        {
            var startDate = DateTime.Now.AddYears(-numberOfYear);

            BalanceHolders = new List<BalanceHolder>
                {
                    new BalanceHolder
                    {
                        Amount = 0,
                        Date = startDate,
                    }
                };
        }

        public void SetBalanceHolders(DateTime startDate, decimal depositAmountMonthly, List<BalanceHolder> balanceHolders)
        {
            BalanceHolders.Add(new BalanceHolder
            {
                Amount = depositAmountMonthly,
                Date = startDate
            });
            balanceHolders = BalanceHolders;
        }

    }
}
