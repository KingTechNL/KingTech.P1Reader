using System.Net.Mime;
using KingTech.P1Reader.Services;
using Microsoft.AspNetCore.Mvc;

namespace KingTech.P1Reader.Controllers
{
    [ApiController]
    [Route("/")]
    public class P1Controller : ControllerBase
    {
        
        private readonly ILogger<P1Controller> _logger;
        private readonly IP1Receiver _receiver;

        public P1Controller(ILogger<P1Controller> logger, IP1Receiver receiver)
        {
            _logger = logger;
            _receiver = receiver;
        }

        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(type: typeof(List<P1Message>), statusCode: StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            return Ok(_receiver.LastTelegram);
        }
    }
}