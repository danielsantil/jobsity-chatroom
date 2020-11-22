using System;
using JobsityChatroom.WebAPI.Models.Authentication;
using JobsityChatroom.WebAPI.Models.Chatroom;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobsityChatroom.WebAPI.Data
{
    public class ChatroomDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ChatMessage> Messages { get; set; }

        public ChatroomDbContext(DbContextOptions<ChatroomDbContext> options) : base(options) { }
    }
}
