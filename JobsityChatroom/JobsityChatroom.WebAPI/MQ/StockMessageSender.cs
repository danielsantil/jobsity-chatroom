using System;
using System.Text;
using RabbitMQ.Client;

namespace JobsityChatroom.WebAPI.MQ
{
    public class StockMessageSender : IStockMessageSender
    {
        private const string STOCK_MESSAGE_REQUEST_Q = "StockMessageRequest";
        private readonly ConnectionFactory _factory;

        public StockMessageSender()
        {
            _factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                VirtualHost = "/",
                UserName = "admin",
                Password = "admin"
            };
        }

        public void Send(string stockCode)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: STOCK_MESSAGE_REQUEST_Q,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            var body = Encoding.UTF8.GetBytes(stockCode);
            channel.BasicPublish(exchange: "",
                                    routingKey: STOCK_MESSAGE_REQUEST_Q,
                                    basicProperties: null,
                                    body: body);
        }
    }
}
