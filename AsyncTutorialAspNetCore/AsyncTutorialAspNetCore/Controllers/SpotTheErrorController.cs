using AsyncTutorialAspNetCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncTutorialAspNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotTheErrorController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SpotTheErrorController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [Route("result-try-catch")]
        public string ResultTryCatch()
        {
            var httpClient = _httpClientFactory.CreateClient();

            try
            {
                return httpClient.GetStringAsync("https://www.invalidsite.com/").Result;
            }
            catch (HttpRequestException ex)
            {
                return ex.Message;
            }

            #region Answer For Result Try Catch

            // Result will wrap all innerexception and throw as AggregateException so we can catch it

            /*try
            {
                return httpClient.GetStringAsync("https://www.invalidsite.com/").GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                return ex.Message;
            }*/

            #endregion Answer
        }

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
            using (var streamWriter = new StreamWriter(Response.Body))
            {
                await streamWriter.WriteAsync("Hello World");
            }

            #region Answer For Stream

            // If we didn't manually call FlushAsync before disposing then it will synchronously Flush

            /*using (var streamWriter = new StreamWriter(Response.Body))
            {
                await streamWriter.WriteAsync("Hello World");

                await streamWriter.FlushAsync();
            }*/

            #endregion Answer
        }

        [HttpGet]
        [Route("list-error")]
        public async Task ListError()
        {
            var users = new List<string>();
            var tasks = new List<Task>();

            tasks.Add(AddUser(users, 1));
            tasks.Add(AddUser(users, 2));

            await Task.WhenAll(tasks);

            #region Answer For List Error

            // Since AspNetCore doesn't have SynchronizationContext, after the await remaining code will run on different thread
            // So in this case list is modified from two different thread at the same time.

            #endregion Answer
        }

        public async Task AddUser(List<string> users, int id)
        {
            // Emulating get user data from server
            await Task.Delay(1000);
            var user = id.ToString();

            users.Add(user);
        }
    }
}