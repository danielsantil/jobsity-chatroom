using System;
using System.Threading.Tasks;
using JobsityChatroom.WebAPI.Models.Authentication;
using Microsoft.AspNetCore.Identity;

namespace JobsityChatroom.WebAPI.Services
{
    public interface IUserService
    {
        Task<IdentityUser> Login(AuthViewModel authModel);
        Task<IdentityUser> Register(SignupViewModel authModel);
        Task<bool> UserExists(string username);
        Task<ApplicationUser> GetUser(string username);
        Task<ApplicationUser> GetUserById(string userId);
    }
}
