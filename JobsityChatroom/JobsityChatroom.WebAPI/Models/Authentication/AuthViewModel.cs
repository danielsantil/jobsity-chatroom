using System;
using System.ComponentModel.DataAnnotations;

namespace JobsityChatroom.WebAPI.Models.Authentication
{
    public class AuthViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
