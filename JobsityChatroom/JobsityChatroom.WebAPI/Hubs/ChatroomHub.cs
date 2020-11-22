using System;
using System.Threading.Tasks;
using JobsityChatroom.WebAPI.Data.Repository;
using JobsityChatroom.WebAPI.Models.Chatroom;
using JobsityChatroom.WebAPI.Services;
using Microsoft.AspNetCore.SignalR;

namespace JobsityChatroom.WebAPI.Hubs
{
    public class ChatroomHub : Hub
    {
        private readonly IRepository<ChatMessage> _messagesRepository;
        private readonly IUserService _userService;

        public ChatroomHub(IRepository<ChatMessage> messagesRepository,
            IUserService userService)
        {
            _messagesRepository = messagesRepository;
            _userService = userService;
        }

        public async Task SendMessage(ChatMessageViewModel messageModel)
        {
            // check if message is a command (starting with /)

            // insert message
            var date = DateTime.Now;
            await _messagesRepository.Insert(new ChatMessage
            {
                CreatedOn = date,
                UserId = messageModel.UserId,
                Body = messageModel.Body
            });

            var user = await _userService.GetUserById(messageModel.UserId);

            await Clients.All.SendAsync("MessageReceived", new ChatMessageResponse
            {
                Body = messageModel.Body,
                CreatedOn = date,
                User = new UserViewModel
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    Email = user.Email
                }
            });
        }
    }
}
