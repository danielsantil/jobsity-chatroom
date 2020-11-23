using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JobsityChatroom.Common.Constants;
using JobsityChatroom.Common.Models;
using JobsityChatroom.WebAPI.Hubs;
using JobsityChatroom.WebAPI.Models.Chatroom;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace JobsityChatroom.WebAPI.MQ
{
    public class StockMessageConsumer : BackgroundService, IStockMessageConsumer
    {
        private readonly IHubContext<ChatroomHub> _chatHubContext;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IConfiguration _configuration;

        public StockMessageConsumer(IHubContext<ChatroomHub> chatHubContext,
            IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["MQ:HostName"],
                Port = int.Parse(_configuration["MQ:Port"]),
                VirtualHost = _configuration["MQ:VirtualHost"],
                UserName = _configuration["MQ:User"],
                Password = _configuration["MQ:Password"]
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: AppConstants.STOCK_MESSAGE_RESPONSE_Q,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            _chatHubContext = chatHubContext;
        }

        public void Consume()
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var response = GetStockResponse(message);

                SendMessageToHub(response);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: AppConstants.STOCK_MESSAGE_RESPONSE_Q,
                                    autoAck: false,
                                    consumer: consumer);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            Consume();

            return Task.CompletedTask;
        }

        private string GetStockResponse(string mqResponse)
        {
            var stockResponse = JsonConvert.DeserializeObject<StockInfoResponse>(mqResponse);

            var response = $"Stock code not found.";
            if (stockResponse.Success)
            {
                var closeValue = stockResponse.StockInfo.Close.ToString("F");
                response = $"{stockResponse.StockInfo.Symbol} quote is ${closeValue} per share";
            }

            return response;
        }

        private void SendMessageToHub(string response)
        {
            _chatHubContext.Clients.All.SendAsync(AppConstants.MESSAGE_RECEIVED_HUB_EVENT,
                new ChatMessageResponse
                {
                    Body = response,
                    CreatedOn = DateTime.Now,
                    User = new UserViewModel
                    {
                        UserId = AppConstants.CHATBOT_USERID,
                        Username = AppConstants.CHATBOT_USERNAME
                    }
                });
        }

        public override void Dispose()
        {
            _connection.Close();
            _channel.Close();
            base.Dispose();
        }
    }
}
