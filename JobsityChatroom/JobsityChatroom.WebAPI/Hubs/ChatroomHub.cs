using System;
using System.Threading.Tasks;
using JobsityChatroom.WebAPI.Data.Repository;
using JobsityChatroom.WebAPI.Models.Chatroom;
using JobsityChatroom.WebAPI.MQ;
using JobsityChatroom.WebAPI.Services;
using Microsoft.AspNetCore.SignalR;

namespace JobsityChatroom.WebAPI.Hubs
{
    public class ChatroomHub : Hub
    {
        public const string MESSAGE_RECEIVED_EVENT = "MessageReceived";
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
            try
            {
                if (IsCommand(messageModel.Body))
                {
                    var commandAndValue = ExtractCommandAndValue(messageModel.Body);
                    _stockSender.Send(commandAndValue.Item2);
                }
                else
                {
                    messageModel.CreatedOn = DateTime.Now;
                    await _messagesRepository.Insert(new ChatMessage
                    {
                        CreatedOn = messageModel.CreatedOn,
                        UserId = messageModel.UserId,
                        Body = messageModel.Body
                    });
                }
            }
            catch(Exception e)
            {
            }
            finally
            {
                await SendMessageToAll(messageModel);
            }

        }

        private async Task SendMessageToAll(ChatMessageViewModel messageModel)
        {
            var user = await _userService.GetUserById(messageModel.UserId);

            await Clients.All.SendAsync(MESSAGE_RECEIVED_EVENT, new ChatMessageResponse
            {
                Body = messageModel.Body,
                CreatedOn = messageModel.CreatedOn,
                User = new UserViewModel
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    Email = user.Email
                }
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
