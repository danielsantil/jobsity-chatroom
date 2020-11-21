using System;
using System.Threading.Tasks;
using JobsityChatbot.WebAPI.Models.Authentication;
using Microsoft.AspNetCore.Identity;

namespace JobsityChatbot.WebAPI.Services
{
    public interface IUserService
    {
        Task<IdentityUser> Login(AuthViewModel authModel);
        Task<IdentityUser> Register(SignupViewModel authModel);
        Task<bool> UserExists(string username);
    }
}
