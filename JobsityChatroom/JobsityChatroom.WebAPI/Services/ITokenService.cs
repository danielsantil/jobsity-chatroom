using System;
using Microsoft.AspNetCore.Identity;

namespace JobsityChatroom.WebAPI.Services
{
    public interface ITokenService
    {
        string GetAccessToken(IdentityUser user);
    }
}
