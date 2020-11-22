using System;
using System.Threading.Tasks;
using JobsityChatroom.WebAPI.Data.Repository;
using JobsityChatroom.WebAPI.Models.Chatroom;
using Microsoft.AspNetCore.SignalR;

namespace JobsityChatroom.WebAPI.Hubs
{
    public class ChatroomHub : Hub
    {
        private readonly IRepository<ChatMessage> _messagesRepository;

        public ChatroomHub(IRepository<ChatMessage> messagesRepository)
        {
            _messagesRepository = messagesRepository;
        }

        public async Task SendMessage(ChatMessageViewModel messageModel)
        {
            // check if message is a command (starting with /)

            // insert message
            

            await Clients.All.SendAsync("MessageReceived", messageModel);
        }
    }
}
