using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncTutorialAspNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HowToAsyncController : ControllerBase
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
        [Route("task-run")]
        public async Task<string> TaskRun()
        {
            // Adding async will not run the content of the method in a different thread
            await Task.Run(() => { for (int i = 0; i < 100000; i++) {  } });

            return "Finished";
        }

        [HttpGet]
        [Route("new-thread")]
        public async Task<string> NewThread()
        {
            // Create new thread for long running background thread
            var newThread = new Thread(() => { for (int i = 0; i < 100000; i++) { } })
            {
                IsBackground = true
            };

            newThread.Start();

            return "Started Background Thread";
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
        [Route("cache-task")]
        public async Task<IList<string>> CacheTask()
        {
            // Reuse the same Task for same result
		    // If caching is used, cache Task<Result> rather than Result
		    // Common return values like true, false, 1, 0, null are precached by.net so there is no new task has created
		    // Less unnecessary objects will be created
            // GC will run less

            var result = new List<string>();

            result.Add(await GetAsync("1"));
            result.Add(await GetAsync("2"));
            result.Add(await GetAsync("3"));

            result.Add(await GetAsync("1"));
            result.Add(await GetAsync("2"));
            result.Add(await GetAsync("3"));

            return result;
        }

        static ConcurrentDictionary<string, Task<string>> _cache = new ConcurrentDictionary<string, Task<string>>(StringComparer.Ordinal);

        public Task<string> GetAsync(string key)
        {
            return _cache.GetOrAdd(key, GetValuesFromServerAsync);
        }

        private async Task<string> GetValuesFromServerAsync(string key)
        {
            await Task.Delay(1000);
            return Guid.NewGuid().ToString();
        }

        [HttpGet]
        [Route("sync-context")]
        public async Task<string> SyncContext()
        {
            // Task.ConfigureAwait(true)    // Default
            // Send the task to Current SynchronizationContext
            // If null, to current TaskScheduler
            // If null, to Thread Pool
            // Mainly used for application level code which is thread sensitive

            // Task.ConfigureAwait(false)
            // Continues on the same context after completing the task
            // Improves performs by avoiding unnecessary thread switching
            // Mainly used for Libraries

            // AspNetCore doesn't have SynchronizationContext so it will act as ConfigureAwait(false)

            return await InnerMethod().ConfigureAwait(false);
        }

        // Empty Body Overhead -> Just by adding async without any await, it will create overhead
        // Its roughly equivalent to do 40 empty `for` loop
        // It is negligible but in method hierarchy it will add up (which is also negligible)
        // It will have non-negligible overhead when we await in big loop.
        // If method skips await then it wont allocate memory for state machine and delegate because It won't move state machine to heap
        // Reduce unnecessary fields in async method -> Reduces size of the state machine object
        // Consider writing async big method

        [HttpGet]
        [Route("custom-state-machine")]
        public int CustomStateMachine()
        {
            return FooAsync().Result;
        }

        public Task<int> FooAsync()
        {
            var stateMachine = new MyStateMachine();
            stateMachine.methodBuilder = AsyncTaskMethodBuilder<int>.Create();
            stateMachine.methodBuilder.Start(ref stateMachine);
            return stateMachine.methodBuilder.Task;
        }

        public async Task<int> ActualFooAsync()
        {
            await Task.Delay(10000);
            return 10;
        }
    }

    public struct MyStateMachine : IAsyncStateMachine
    {
        public AsyncTaskMethodBuilder<int> methodBuilder;
        private int state;
        private TaskAwaiter awaiter;

        public void MoveNext()
        {
            if (state == 0)
            {
                awaiter = Task.Delay(10000).GetAwaiter();
                if (awaiter.IsCompleted)
                {
                    state = 1;
                    goto state1;
                }
                else
                {
                    state = 1;

                    // ref moves state machine to heap
                    methodBuilder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
                }
                return;
            }
        state1:
            if (state == 1)
            {
                // Task is done so no blocking call
                awaiter.GetResult();
                methodBuilder.SetResult(10);
            }
        }

        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            methodBuilder.SetStateMachine(stateMachine);
        }
    }
}