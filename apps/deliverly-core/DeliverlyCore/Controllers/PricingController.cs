using Microsoft.AspNetCore.Mvc;

namespace DeliverlyCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PricingController : ControllerBase
    {
        [HttpGet(Name = "GetPricingConfig")]
        public IEnumerable<PricingConfig> Get()
        {
            return [new PricingConfig
                {
                    Id = Guid.NewGuid(),
                    EffectiveDate = DateTime.Now,
                    BasePrice = 50.00m,
                    SurgeMultiplier = 1.5,
                    PricingSource = "Base",
                    Status = "Active",
                }
            ];
        }
    }
}
