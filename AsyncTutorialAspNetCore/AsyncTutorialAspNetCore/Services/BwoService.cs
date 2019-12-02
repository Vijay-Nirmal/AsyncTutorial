using System;
using System.Threading.Tasks;

namespace AsyncTutorialAspNetCore.Services
{
    public interface IBwoService
    {
        Task DoSomethingAsync();
    }

    public class BwoService : IBwoService
    {
        public async Task DoSomethingAsync()
        {
            await Task.Delay(1000);
        }
    }

    public interface IBwoServiceConnection
    {
        Task<IBwoService> ConnectAsync();
    }

    public class BwoServiceConnection : IBwoServiceConnection
    {
        public async Task<IBwoService> ConnectAsync()
        {
            // Create BWOService object in async
            await Task.Delay(1000);
            return new BwoService();
        }
    }

    #region Answer For Service Creation Async

    public class LazyBwoService : IBwoService
    {
        private readonly AsyncLazy<IBwoService> _connectionTask;

        public LazyBwoService(IBwoServiceConnection bwoServiceConnection)
        {
            _connectionTask = new AsyncLazy<IBwoService>(() => bwoServiceConnection.ConnectAsync());
        }

        public async Task DoSomethingAsync()
        {
            var connection = await _connectionTask.Value;

            await connection.DoSomethingAsync();
        }

        private class AsyncLazy<T> : Lazy<Task<T>>
        {
            public AsyncLazy(Func<Task<T>> valueFactory) : base(valueFactory)
            {
            }
        }
    }

    #endregion
}