using System;
using System.Text;
using JobsityChatroom.Common.Constants;
using JobsityChatroom.Common.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace JobsityChatroom.StockBot.MQ
{
    public class StockBotMessageSender
    {
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

        public void Send(StockInfoResponse response)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: AppConstants.STOCK_MESSAGE_RESPONSE_Q,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            var responseJson = JsonConvert.SerializeObject(response);
            var body = Encoding.UTF8.GetBytes(responseJson);
            channel.BasicPublish(exchange: "",
                                    routingKey: AppConstants.STOCK_MESSAGE_RESPONSE_Q,
                                    basicProperties: null,
                                    body: body);
        }
    }
}
