using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncTutorialAspNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsyncAdvantageController : ControllerBase
    {
        private readonly ILogger<AsyncAdvantageController> _logger;

        public AsyncAdvantageController(ILogger<AsyncAdvantageController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("sleep")]
        public ActionResult<IEnumerable<string>> Sleep()
        {
            Thread.Sleep(5000);
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        [Route("delay-wait")]
        public ActionResult<IEnumerable<string>> DelayWait()
        {
            // Sync over Async
            Task.Delay(5000).Wait();
            return new string[] { "value1", "value2" };
        }

        [HttpGet("run-sleep")]
        public async Task<string> RunSleep()
        {
            // Async over Sync
            await Task.Run(() => Thread.Sleep(5000));
            return "Hello World";
        }

        [HttpGet]
        [Route("async")]
        public async Task<ActionResult<IEnumerable<string>>> GetAsync()
        {
            // Sync over Async
            await Task.Delay(5000);
            return new string[] { "value1", "value2" };
        }
    }
}