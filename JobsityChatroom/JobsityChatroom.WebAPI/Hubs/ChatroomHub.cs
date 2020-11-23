using System;
using System.Threading.Tasks;
using JobsityChatroom.Common.Constants;
using JobsityChatroom.WebAPI.Data.Repository;
using JobsityChatroom.WebAPI.Models.Chatroom;
using JobsityChatroom.WebAPI.MQ;
using JobsityChatroom.WebAPI.Services;
using Microsoft.AspNetCore.SignalR;

namespace JobsityChatroom.WebAPI.Hubs
{
    public class ChatroomHub : Hub
    {
        private readonly IRepository<ChatMessage> _messagesRepository;
        private readonly IUserService _userService;
        private readonly IStockMessageSender _stockSender;

        public ChatroomHub(IRepository<ChatMessage> messagesRepository,
            IUserService userService, IStockMessageSender stockSender)
        {
            _messagesRepository = messagesRepository;
            _userService = userService;
            _stockSender = stockSender;
        }

        public async Task SendMessage(ChatMessageViewModel messageModel)
        {
            var isCommand = IsCommand(messageModel.Body);
            messageModel.CreatedOn = DateTime.Now;
            await SendMessageToAll(messageModel, isCommand);

            try
            {
                if (isCommand)
                {
                    // TODO create ChatCommandsHandler
                    var commandAndValue = ExtractCommandAndValue(messageModel.Body);
                    if (commandAndValue.Item1 == "stock")
                        _stockSender.Send(commandAndValue.Item2);
                    else
                        await SendMessageFromBot("Unknown command");
                }
                else
                {
                    await _messagesRepository.Insert(new ChatMessage
                    {
                        CreatedOn = messageModel.CreatedOn,
                        UserId = messageModel.UserId,
                        Body = messageModel.Body
                    });
                }
            }
            catch (Exception e)
            {
                await SendMessageFromBot(e.Message);
            }

        }

        private async Task SendMessageToAll(ChatMessageViewModel messageModel, bool isCommand)
        {
            var dbUser = await _userService.GetUserById(messageModel.UserId);
            var user = new UserViewModel { UserId = dbUser.Id, Username = dbUser.UserName, Email = dbUser.Email }; 
            await SendMessageToHub(messageModel.Body, user, isCommand, messageModel.CreatedOn);
        }

        private async Task SendMessageFromBot(string body)
        {
            var botUser = new UserViewModel
            {
                UserId = AppConstants.CHATBOT_USERID,
                Username = AppConstants.CHATBOT_USERNAME
            };
            await SendMessageToHub(body, botUser);
        }

        private async Task SendMessageToHub(string body, UserViewModel user,
            bool isCommand = false, DateTime? createdOn = null)
        {
            await Clients.All.SendAsync(AppConstants.MESSAGE_RECEIVED_HUB_EVENT, new ChatMessageResponse
            {
                Body = body,
                CreatedOn = createdOn ?? DateTime.Now,
                IsCommand = isCommand,
                User = user
            });
        }

        private bool IsCommand(string message)
        {
            return !string.IsNullOrWhiteSpace(message) && message.StartsWith("/");
        }

        private (string, string) ExtractCommandAndValue(string message)
        {
            var parts = message.Trim().ToLower().Substring(1).Split("=");
            if (parts.Length < 2)
            {
                throw new Exception("Invalid command syntax");
            }

            return (parts[0], parts[1]);
        }
    }
}
