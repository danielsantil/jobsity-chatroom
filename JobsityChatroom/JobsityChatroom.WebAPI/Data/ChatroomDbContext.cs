using System;
using JobsityChatroom.WebAPI.Models.Authentication;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobsityChatroom.WebAPI.Data
{
    public class ChatroomDbContext : IdentityDbContext<ApplicationUser>
    {
        
        public ChatroomDbContext(DbContextOptions<ChatroomDbContext> options) : base(options) { }
    }
}
