using AsyncTutorialAspNetCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace AsyncTutorialAspNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotTheErrorController : ControllerBase
    {
        [HttpGet]
        [Route("async-void")]
        public async void Sleep()
        {
            await Task.Delay(1000);

            await Response.WriteAsync("Hello World");

            #region Answer For Async-Void

            // This will crash the process since we're writing after the response has completed on a background thread

            #endregion Answer
        }

        [HttpGet]
        [Route("stream")]
        public async Task StreamAsync()
        {
            var stream = System.IO.File.Create("SampleFile", 4096, FileOptions.Asynchronous);
            using (var streamWriter = new StreamWriter(stream))
            {
                await streamWriter.WriteAsync("Hello World");
            }

            #region Answer For Stream

            // If we didn't manually call FlushAsync before disposing then it will synchronously Flush

            var newStream = System.IO.File.Create("SampleFile", 4096, FileOptions.Asynchronous);
            using (var streamWriter = new StreamWriter(newStream))
            {
                await streamWriter.WriteAsync("Hello World");

                await streamWriter.FlushAsync();
            }

            #endregion Answer
        }
    }
}