using System;
using System.Text;
using JobsityChatroom.Common.Constants;
using JobsityChatroom.StockBot.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace JobsityChatroom.StockBot.MQ
{
    public class StockBotMessageConsumer
    {
        private readonly StocksService _stocksService;
        private readonly StockBotMessageSender _stockMessageSender;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public StockBotMessageConsumer()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                VirtualHost = "/",
                UserName = "admin",
                Password = "admin"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: AppConstants.STOCK_MESSAGE_REQUEST_Q,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            _stocksService = new StocksService();
            _stockMessageSender = new StockBotMessageSender();
        }

        public void Consume()
        {
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
    }
}
