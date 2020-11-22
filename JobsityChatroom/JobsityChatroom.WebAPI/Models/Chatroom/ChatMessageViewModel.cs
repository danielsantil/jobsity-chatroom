using System;

namespace JobsityChatroom.WebAPI.Models.Chatroom
{
    public class ChatMessageViewModel
    {
        public string UserId { get; set; }
        public string Body { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
