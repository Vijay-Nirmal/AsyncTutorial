using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AsyncTutorialAspNetCore
{
    public class Program
    {
        public static int Requests;

        public static void Main(string[] args)
        {
            /*new Thread(ShowThreadStats)
            {
                IsBackground = true
            }.Start();*/

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConsole();
            })
            .UseStartup<Startup>();

        private static void ShowThreadStats(object _)
        {
            ThreadPool.SetMinThreads(2, 50);
            ThreadPool.SetMaxThreads(10, 100);
            while (true)
            {
                ThreadPool.GetAvailableThreads(out var workerThread, out var completionPortThreads);
                // ThreadPool.GetMinThreads(out var minWorkerThread, out var minCompletionPortThreads);
                ThreadPool.GetMaxThreads(out var maxWorkerThread, out var maxCompletionPortThreads);

                Console.WriteLine($"Request -> {Requests} || WorkerThread -> Available: {workerThread} --- Active: {maxWorkerThread - workerThread} || CompletionPortThreads -> Available: {completionPortThreads} --- Active: {maxCompletionPortThreads - completionPortThreads}");
                Thread.Sleep(1000);
            }
        }
    }
}
