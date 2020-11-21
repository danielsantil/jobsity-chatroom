using System;
using JobsityChatbot.WebAPI.Models.Authentication;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobsityChatbot.WebAPI.Data
{
    public class ChatbotDbContext : IdentityDbContext<ApplicationUser>
    {
        
        public ChatbotDbContext(DbContextOptions<ChatbotDbContext> options) : base(options) { }
    }
}
