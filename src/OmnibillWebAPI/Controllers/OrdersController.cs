namespace Omnibill.WebAPI.Controllers
{
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    //[ProducesResponseType(StatusCodes.Status401Unauthorized)] // TODO (Cameron): Intercept via middleware to add problem details.
    //[ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger logger;

        public OrdersController(ILogger<OrdersController> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Post()
        {
            return null;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Order))]
        public IActionResult Get(int id)
        {
            var stream = new FileStream(@"temp.pdf", FileMode.Open);
            return File(stream, "application/pdf", "invoice.pdf");
        }
    }
}
