using System;
using System.Threading.Tasks;

namespace AsyncTutorialAspNetCore.Services
{
    public interface IFfService
    {
        Task DoSomethingAsync();
    }

    public class FfService : IFfService
    {
        private string _context;

        public FfService(IFfServiceContext ffServiceConnection)
        {
            _context = ffServiceConnection.GetContext().Result;
        }

        public async Task DoSomethingAsync()
        {
            await Task.Delay(1000);
        }
    }

    public interface IFfServiceContext
    {
        Task<string> GetContext();
    }

    public class FfServiceContext : IFfServiceContext
    {
        public async Task<string> GetContext()
        {
            await Task.Delay(1000);
            return "Context";
        }
    }

    #region Answer

    public class AnswerFfService : IFfService
    {
        private string _context;

        private AnswerFfService(string context)
        {
            _context = context;
        }

        public static async Task<AnswerFfService> Create(IFfServiceContext ffServiceConnection)
        {
            var context = await ffServiceConnection.GetContext();
            return new AnswerFfService(context);
        }

        public async Task DoSomethingAsync()
        {
            await Task.Delay(1000);
        }
    }

    #endregion
}