using System;
using System.IO;
using System.Threading;
using JobsityChatroom.StockBot.MQ;
using JobsityChatroom.StockBot.Services;
using Microsoft.Extensions.Configuration;

namespace JobsityChatroom.StockBot
{
    class Program
    {
        private static readonly AutoResetEvent closingEvent = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            Console.WriteLine("StockBot environment: {0}", env ?? "Development");

            var settingsFile = env == null ? "appsettings.Development.json"
                : "appsettings.json";

            // reads configs
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(settingsFile, optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            Console.WriteLine("StockBot started");

            var consumer = new StockBotMessageConsumer(configuration);
            Console.WriteLine("Starting consumer...");
            consumer.Consume();

            Console.CancelKeyPress += (sender, ea) =>
            {
                Console.WriteLine("Exiting StockBot");
                closingEvent.Set();
            };

            closingEvent.WaitOne();
        }
    }
}
