using System;
using Microsoft.AspNetCore.Identity;

namespace JobsityChatbot.WebAPI.Services
{
    public interface ITokenService
    {
        string GetAccessToken(IdentityUser user);
    }
}
