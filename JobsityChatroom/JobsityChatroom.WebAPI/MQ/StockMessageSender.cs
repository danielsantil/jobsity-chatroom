using System;
using System.Text;
using JobsityChatroom.Common.Constants;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace JobsityChatroom.WebAPI.MQ
{
    public class StockMessageSender : IStockMessageSender
    {
        private readonly ConnectionFactory _factory;
        private readonly IConfiguration _configuration;

        public StockMessageSender(IConfiguration configuration)
        {
            _configuration = configuration;
            _factory = new ConnectionFactory()
            {
                HostName = _configuration["MQ:HostName"],
                Port = int.Parse(_configuration["MQ:Port"]),
                VirtualHost = _configuration["MQ:VirtualHost"],
                UserName = _configuration["MQ:User"],
                Password = _configuration["MQ:Password"]
            };
        }

        public void Send(string stockCode)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: AppConstants.STOCK_MESSAGE_REQUEST_Q,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            var body = Encoding.UTF8.GetBytes(stockCode);
            channel.BasicPublish(exchange: "",
                                    routingKey: AppConstants.STOCK_MESSAGE_REQUEST_Q,
                                    basicProperties: null,
                                    body: body);
        }
    }
}
