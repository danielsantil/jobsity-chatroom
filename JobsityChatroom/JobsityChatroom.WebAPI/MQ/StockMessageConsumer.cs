using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JobsityChatroom.WebAPI.Hubs;
using JobsityChatroom.WebAPI.Models.Chatroom;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace JobsityChatroom.WebAPI.MQ
{
    public class StockMessageConsumer : BackgroundService, IStockMessageConsumer
    {
        private const string STOCK_MESSAGE_RESPONSE_Q = "StockMessageResponse";
        private readonly IHubContext<ChatroomHub> _chatHubContext;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public StockMessageConsumer(IHubContext<ChatroomHub> chatHubContext)
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
            _channel.QueueDeclare(queue: STOCK_MESSAGE_RESPONSE_Q,
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
                var response = "Consuming: " + message;
                SendMessageToHub(response);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(queue: STOCK_MESSAGE_RESPONSE_Q,
                                    autoAck: false,
                                    consumer: consumer);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            Consume();

            return Task.CompletedTask;
        }

        private void SendMessageToHub(string response)
        {
            _chatHubContext.Clients.All.SendAsync(ChatroomHub.MESSAGE_RECEIVED_EVENT,
                new ChatMessageResponse
                {
                    Body = response,
                    CreatedOn = DateTime.Now,
                    User = new UserViewModel
                    {
                        UserId = "0",
                        Username = "ChatBot",
                        Email = null
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
