//using HP.PersonalStocks.Mgr.Logics;
//using HP.PersonalStocksAlerter.Models.Models;

//namespace NUnitTests;

//public class StockNatualSelectionCalculatorTests
//{

//    [Test]
//    [TestCase(3)]
//    [TestCase(1)]
//    [TestCase(6)]
//    public void TestSetAvailableBalance(int numberOfDeposit)
//    {

//        var stockNatualSelectionCalculator = new CalculatorManager();
//        var monthlyDeposit = stockNatualSelectionCalculator.PositionLedgerSummary.DepositAmountMonthly;
//        for (int i = 0; i < numberOfDeposit; i++)
//        {

//            stockNatualSelectionCalculator.PositionLedgerSummary.BalanceHolders.Add(new BalanceHolder
//            {
//                Amount = monthlyDeposit,
//                Date = DateTime.Now,
//            });
//        }
//        stockNatualSelectionCalculator.SetAvailableBalance();

//        Assert.That(monthlyDeposit * numberOfDeposit, Is.EqualTo(stockNatualSelectionCalculator.PositionLedgerSummary.Statistics.AvalableBalance));
//    }


//    [Test]
//    [TestCase(3)]
//    [TestCase(1)]
//    [TestCase(6)]
//    public void TestSetAvailableBalanceByYear(int numberOfDeposit)
//    {

//        var stockNatualSelectionCalculator = new CalculatorManager();
//        var monthlyDeposit = stockNatualSelectionCalculator.PositionLedgerSummary.DepositAmountMonthly;
//        for (int i = 0; i < numberOfDeposit; i++)
//        {
//            stockNatualSelectionCalculator.PositionLedgerSummary.BalanceHolders.Add(new BalanceHolder
//            {
//                Amount = monthlyDeposit,
//                Date = DateTime.Now,
//            });
//            stockNatualSelectionCalculator.PositionLedgerSummary.BalanceHolders.Add(new BalanceHolder
//            {
//                Amount = monthlyDeposit,
//                Date = DateTime.Now.AddYears(-1),
//            });
//        }

//        var currentStatictic = new AnnualStatistics();
//        stockNatualSelectionCalculator.PositionLedgerSummary.AnnualStatistics
//            .Add(currentStatictic);
//       // stockNatualSelectionCalculator.SetAvailableBalanceByYear(DateTime.Now.Year, currentStatictic);

//        Assert.That(monthlyDeposit * numberOfDeposit, Is.EqualTo(currentStatictic.AvalableBalance));
//      //  stockNatualSelectionCalculator.SetAvailableBalanceByYear(DateTime.Now.Year - 1, currentStatictic);

//        Assert.That(monthlyDeposit * numberOfDeposit, Is.EqualTo(currentStatictic.AvalableBalance));

//    }

//    [Test]
//    public void GetAllDepositBallance()
//    {
//       // return PositionLedgerSummary.BalanceHolders.Sum(balance => balance.Amount);

//    }
//}
