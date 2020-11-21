using System;
using System.Threading.Tasks;
using JobsityChatroom.WebAPI.Models.Authentication;
using Microsoft.AspNetCore.Identity;

namespace JobsityChatroom.WebAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityUser> Login(AuthViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (!await _userManager.CheckPasswordAsync(user, model.Password))
                return null;

            return user;
        }

        public async Task<IdentityUser> Register(SignupViewModel model)
        {
            var newUser = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);
            if (!result.Succeeded)
                return null;

            return newUser;
        }

        public async Task<bool> UserExists(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            return user != null;
        }
    }
}
