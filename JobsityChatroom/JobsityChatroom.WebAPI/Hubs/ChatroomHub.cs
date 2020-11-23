using System;
using System.Threading.Tasks;
using JobsityChatroom.Common.Constants;
using JobsityChatroom.WebAPI.Data.Repository;
using JobsityChatroom.WebAPI.Models.Chatroom;
using JobsityChatroom.WebAPI.Models.Command;
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
        private readonly IChatCommandService _commandService;

        public ChatroomHub(IRepository<ChatMessage> messagesRepository,
            IUserService userService, IStockMessageSender stockSender,
            IChatCommandService commandService)
        {
            _messagesRepository = messagesRepository;
            _userService = userService;
            _stockSender = stockSender;
            _commandService = commandService;
        }

        public async Task SendMessage(ChatMessageViewModel messageModel)
        {
            var isCommand = _commandService.IsCommand(messageModel.Body);
            messageModel.CreatedOn = DateTime.Now;
            messageModel.Body = messageModel.Body.Trim();
            await SendMessageToAll(messageModel, isCommand);

            try
            {
                if (isCommand)
                {
                    _commandService.Handle(messageModel.Body, command =>
                    {
                        _stockSender.Send(command.Value);
                    });
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
            catch (CommandException e)
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
    }
}
