using Microsoft.AspNetCore.Mvc;
using TraderWebApp.Interfaces;

namespace TraderWebApp.Controllers
{
    [ApiController]
    [Route("api/")]
    public class OrderController : ControllerBase
    {
        private readonly ITraderService _traderService;

        public OrderController(ITraderService traderService)
        {
            _traderService = traderService;
        }


        [HttpGet("getPlan")]
        public async Task<IActionResult> GetBestExecutionPlan([FromQuery] UserInputData input )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _traderService.CalculateBestPlanForUser(input);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest( $"Failed to get your best plan: {ex.Message}");
            }

            
        }

    }
}
