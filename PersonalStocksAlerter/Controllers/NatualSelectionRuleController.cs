using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HP.PersonalStocks.Mgr;
using HP.PersonalStocks.Mgr.Logics;
using HP.PersonalStocksAlerter.Api.ActionResults;
using HP.PersonalStocksAlerter.Api.Responses;
using HP.PersonalStocksAlerter.Models.Models;
using Microsoft.AspNetCore.Mvc;
namespace HP.PersonalStocksAlerter.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NatualSelectionRuleController : ControllerBase
    {
        public NatualSelectionRuleController()
        {
        }

        [HttpPost]
        [Route("GetStatistic/{symbol}/{lookBackNumberOfyear}")]
        public async Task<NatualSelectionResult> GetAlertResultAsync([FromRoute] string symbol, [FromRoute] int lookBackNumberOfyear)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new NatualSelectionResult("Invalid", new Models.Models.PositionLedgerSummary());

                var stockNaturalSelectionMgr = new StockNaturalSelectionMgr(symbol.ToUpper(), lookBackNumberOfyear);
               await  stockNaturalSelectionMgr.StartToCalculateAsync();

                var response = new NatualSelectionResult("", stockNaturalSelectionMgr.CalculatorManager.GetPositionLedgerSummary());

                return response;
            }
            catch (Exception ex)
            {
                return new NatualSelectionResult(ex.Message, new Models.Models.PositionLedgerSummary());
            }
        }

        [HttpPost]
        [Route("{symbol}/{lookBackNumberOfyear}")]
        public async Task< NatualSelectionResult> GetAlertResultForTimeFrameAsync([FromRoute] string symbol, [FromRoute] int lookBackNumberOfyear)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new NatualSelectionResult("Invalid", new PositionLedgerSummary());

                var stockNaturalSelectionMgr = new StockNaturalSelectionMgr(symbol.ToUpper(),
                    numberOfYear: lookBackNumberOfyear,
                    includeHistoricalProfit: true);
                await stockNaturalSelectionMgr.StartToCalculateAsync();

                var response = new NatualSelectionResult("", stockNaturalSelectionMgr.CalculatorManager.GetPositionLedgerSummary());

                return response;
            }
            catch (Exception ex)
            {
                return new NatualSelectionResult(ex.Message, new PositionLedgerSummary());
            }
        }
        [HttpPost]
        [Route("GetStatistics/Multi/{lookBackNumberOfyear}/{displayGainOnly}")]
        public async Task< MultiNatualSelectionResult> GetAlertResultAsync(
            [FromRoute] int lookBackNumberOfyear, 
            [FromBody] List<string> symbols, [FromRoute] bool displayGainOnly = true)
        {
            try
            {
                var result = new MultiNatualSelectionResult("");
                var responses = new List<NatualSelectionResult>();

                if (!ModelState.IsValid)
                    return new MultiNatualSelectionResult("Invalid");

                foreach (var symbol in symbols)
                {
                    var response = await GetAlertResultAsync(symbol, lookBackNumberOfyear);

                    if (displayGainOnly && response.PositionLedgerSummary.Statistics.IsProfitable)
                        responses.Add(response);
                }

                return result.Convert(responses);
            }
            catch (Exception ex)
            {
                return new MultiNatualSelectionResult(ex.Message);
            }
        }
    }
}

