using System;
using JobsityChatroom.WebAPI.Models.Chatroom;

namespace JobsityChatroom.WebAPI.Models.Authentication
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public UserViewModel LoggedUser { get; set; }
    }
}
