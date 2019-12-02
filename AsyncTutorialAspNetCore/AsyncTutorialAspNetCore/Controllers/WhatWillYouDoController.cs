using AsyncTutorialAspNetCore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AsyncTutorialAspNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhatWillYouDoController : ControllerBase
    {
        [HttpGet]
        [Route("constructor-async")]
        public async Task ConstructorAsync()
        {
            var ffServiceContext = new FfServiceContext();
            var ffService = new FfService(ffServiceContext);

            await ffService.DoSomethingAsync();

            #region Answer For Constructor Async
            var answerFfService = await AnswerFfService.Create(ffServiceContext);
            #endregion
        }

        [HttpGet]
        [Route("service-creation-async")]
        public async Task ServiceCreationAsync([FromServices] IBwoService bwoService)
        {
            await bwoService.DoSomethingAsync();
        }
    }
}