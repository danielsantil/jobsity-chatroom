using System;
using System.Text;
using System.Threading;
using JobsityChatroom.Common.Constants;
using JobsityChatroom.StockBot.Services;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace JobsityChatroom.StockBot.MQ
{
    public class StockBotMessageConsumer
    {
        private readonly StocksService _stocksService;
        private readonly StockBotMessageSender _stockMessageSender;
        private readonly IConfiguration _configuration;
        private readonly ConnectionFactory factory;
        private IConnection _connection;
        private IModel _channel;

        public StockBotMessageConsumer(IConfiguration configuration)
        {
            _configuration = configuration;
            _stocksService = new StocksService();
            _stockMessageSender = new StockBotMessageSender(_configuration);

            factory = new ConnectionFactory()
            {
                HostName = _configuration["MQ:HostName"],
                Port = int.Parse(_configuration["MQ:Port"]),
                VirtualHost = _configuration["MQ:VirtualHost"],
                UserName = _configuration["MQ:User"],
                Password = _configuration["MQ:Password"]
            };

        }

        public void Consume()
        {
            EnsureConnection();
            Console.WriteLine("Listening to messages...");

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var stockCode = Encoding.UTF8.GetString(body);
                Console.WriteLine("Received request for stock {0}", stockCode);

                var stockInfo = await _stocksService.GetStockInfo(stockCode);
                _stockMessageSender.Send(stockInfo);
            };

            _channel.BasicConsume(queue: AppConstants.STOCK_MESSAGE_REQUEST_Q,
                                    autoAck: true,
                                    consumer: consumer);
        }

        private void EnsureConnection(int retries = 0)
        {
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.QueueDeclare(queue: AppConstants.STOCK_MESSAGE_REQUEST_Q,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
            }
            catch (BrokerUnreachableException be)
            {
                if (retries == 10)
                {
                    Console.WriteLine("Max retries reached. Closing consumer.");
                    throw be;
                }
                retries += 1;
                Console.WriteLine("MQ Server not running. Attempting connection after {0} ms. Retry count: {1}", 3000, retries);
                Thread.Sleep(3000);
                EnsureConnection(retries);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
