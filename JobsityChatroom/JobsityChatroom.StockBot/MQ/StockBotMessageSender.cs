using System;
using System.Text;
using RabbitMQ.Client;

namespace JobsityChatroom.StockBot.MQ
{
    public class StockBotMessageSender
    {
        private const string STOCK_MESSAGE_RESPONSE_Q = "StockMessageResponse";
        private readonly ConnectionFactory _factory;

        public StockBotMessageSender()
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

        public void Send(string response)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: STOCK_MESSAGE_RESPONSE_Q,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            var body = Encoding.UTF8.GetBytes(response);
            channel.BasicPublish(exchange: "",
                                    routingKey: STOCK_MESSAGE_RESPONSE_Q,
                                    basicProperties: null,
                                    body: body);
        }
    }
}
