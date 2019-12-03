using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncTutorialAspNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HowToAsyncAdvantageController : ControllerBase
    {
        [HttpGet]
        [Route("await")]
        public async Task<string> AwaitAsync()
        {
            await Task.Delay(1000);
            return "Hello World";
        }

        [HttpGet]
        [Route("async-chain")]
        public async Task<string> AsyncChain()
        {
            return await InnerMethod();
        }

        public async Task<string> InnerMethod()
        {
            await Task.Delay(1000);
            return "Hello World";
        }

        [HttpGet]
        [Route("multi-async")]
        public async Task<string> MultiAsync()
        {
            var resultTask = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                resultTask.Add(DoSomethingAsync());
            }

            await Task.WhenAll(resultTask);

            return "Finished";
        }

        public async Task DoSomethingAsync()
        {
            await Task.Delay(1000);
        }

        [HttpGet]
        [Route("multi-return-async")]
        public async Task<string[]> MultiReturnAsync()
        {
            var resultTask = new List<Task<string>>();

            for (int i = 0; i < 10; i++)
            {
                resultTask.Add(ReturnSomethingAsync());
            }

            var result = await Task.WhenAll(resultTask);

            return result;
        }

        public async Task<string> ReturnSomethingAsync()
        {
            await Task.Delay(1000);
            return "Hi";
        }

        [HttpGet]
        [Route("async-runs-sync")]
        public async Task<string> AsyncRunsSync()
        {
            // Async method runs synchronously if task is completed
            await GetValues(false);

            return "Finished";
        }

        public async Task<string> GetValues(bool isAvailable)
        {
            if (isAvailable)
            {
                await Task.Delay(100);
                return "Sample Value";
            }
            else
            {
                return string.Empty;
            }
        }

        [HttpGet]
        [Route("sync-context")]
        public async Task<string> SyncContext()
        {
            return await InnerMethod().ConfigureAwait(false);
        }
    }
}