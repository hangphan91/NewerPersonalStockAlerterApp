using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HP.PersonalStocks.Mgr;
using HP.PersonalStocksAlerter.Api.ActionResults;
using HP.PersonalStocksAlerter.Api.Requests;
using HP.PersonalStocksAlerter.Api.Responses;
using HP.PersonalStocksAlerter.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace HP.PersonalStocksAlerter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonalAlertController : ControllerBase
    {
        [HttpPost]
        [Route("GetMultipleAlerts")]
        public async Task<AlertMultipleActionResult> GetMultipleAlertReultAsync([FromBody] MultipleAlertApiRequest request)
        {
            try
            {

                if (!ModelState.IsValid)
                    return new AlertMultipleActionResult
                    {
                        Exception = new Exception("InValid Input Data")
                    };
                var result = new AlertMultipleActionResult();

                foreach (var item in request.StockStickers)
                {

                    var single = await GetSingleAlertAsync(new AlertApiRequest { StickerSymbol = item });
                    result.AlertResults.Add(single);
                }

                return result;
            }
            catch (Exception ex)
            {
                return new AlertMultipleActionResult(ex);
            }
        }
        [HttpPost]
        [Route("GetSingleAlert")]
        public async Task<AlertResultActionResult> GetAlertResultAsync([FromBody] AlertApiRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new AlertResultActionResult(
                        new AlertApiResonse { Exception = new Exception("InValid Input") });

                AlertResult result = await GetSingleAlertAsync(request);
                var response = new AlertApiResonse
                {
                    AlertResult = result,
                    Exception = result.Success ? null : new Exception(result.ErrorMessage)
                };
                return new AlertResultActionResult(response);
            }
            catch (Exception ex)
            {
                return new AlertResultActionResult(new AlertApiResonse
                {
                    Exception = ex,
                    AlertResult = new AlertResult
                    {
                        Success = false,
                        ErrorMessage = "Errors in execute your request, please retry later."
                    }
                });
            }
        }

        private static async Task<AlertResult> GetSingleAlertAsync(AlertApiRequest request)
        {
            var symbol = request.StickerSymbol.ToUpper();

            var mgr = new AlertMgr(symbol);
            await mgr.GetStockDataAsync();

            return await mgr.GetAlertResultAsync();
        }
    }
}
