using System;
namespace JobsityChatroom.WebAPI.Models.Chatroom
{
    public class ChatMessageResponse
    {
        public string Body { get; set; }
        public UserViewModel User { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsCommand { get; set; }
    }

    public class UserViewModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
