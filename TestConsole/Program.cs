using System;
using System.Threading.Tasks;
using HP.PersonalStocks.Mgr;

namespace TestConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
           // var mgr = new AlertMgr("MSFT");

            // var suggest = mgr.GetSuggestionForCurrentSticker();

            //var result = mgr.CheckForAlert();

            var stockNaturalSelectionMgr = new StockNaturalSelectionMgr("AAPL");
            await stockNaturalSelectionMgr.StartToCalculateAsync();
        }
    }
}
