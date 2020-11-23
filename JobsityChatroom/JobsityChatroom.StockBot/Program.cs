using System;
using System.Threading;
using JobsityChatroom.StockBot.MQ;
using JobsityChatroom.StockBot.Services;

namespace JobsityChatroom.StockBot
{
    class Program
    {
        private static readonly AutoResetEvent closingEvent = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            Console.WriteLine("StockBot started");
            Console.WriteLine("Listening to messages...");

            var consumer = new StockBotMessageConsumer();
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
