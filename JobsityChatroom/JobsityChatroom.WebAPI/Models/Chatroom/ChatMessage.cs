using System;
using System.ComponentModel.DataAnnotations;
using JobsityChatroom.WebAPI.Data.Repository;
using JobsityChatroom.WebAPI.Models.Authentication;

namespace JobsityChatroom.WebAPI.Models.Chatroom
{
    public class ChatMessage : EntityBase
    {
        public DateTime CreatedOn { get; set; }
        public string Body { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
