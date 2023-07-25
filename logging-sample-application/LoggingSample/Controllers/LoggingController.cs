using Microsoft.AspNetCore.Mvc;

namespace LoggingSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoggingController : ControllerBase
    {
        private readonly ILogger<LoggingController> _logger;

        public LoggingController(ILogger<LoggingController> logger)
        {
            _logger = logger;
        }

        [HttpGet("string-interpolation")]
        public void StringInterpolation()
        {
            var vehicleId = Guid.NewGuid();
            _logger.LogInformation($"Retailed vehicle with id {vehicleId}.");
        }

        [HttpGet("high-performance")]
        public void HighPerformance()
        {
            var vehicleId = Guid.NewGuid();
            _logger.LogRetailedVehicle(vehicleId);
        }

        [HttpGet("scope")]
        public Guid Scope()
        {
            var orderId = Guid.NewGuid();
            using (_logger.BeginOrderScope(orderId))
            {
                for (int i = 0; i < 10; i++)
                {
                    var vehicleId = Guid.NewGuid();
                    _logger.LogRetailedVehicle(vehicleId);
                }
            }

            return orderId;
        }
    }
}
