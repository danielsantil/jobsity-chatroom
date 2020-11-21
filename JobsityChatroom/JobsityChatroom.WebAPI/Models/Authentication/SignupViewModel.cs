using System;
using System.ComponentModel.DataAnnotations;

namespace JobsityChatroom.WebAPI.Models.Authentication
{
    public class SignupViewModel : AuthViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
