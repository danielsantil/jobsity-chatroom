using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobsityChatroom.WebAPI.Data.Repository;
using JobsityChatroom.WebAPI.Models.Chatroom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobsityChatroom.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IRepository<ChatMessage> _messagesRepository;

        public MessagesController(IRepository<ChatMessage> messagesRepository)
        {
            _messagesRepository = messagesRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<ChatMessageResponse>> Get()
        {
            var messages = (await _messagesRepository.GetAll(x => x.CreatedOn))
                .TakeLast(50)
                .Select(x => new ChatMessageResponse
                {
                    Body = x.Body,
                    CreatedOn = x.CreatedOn,
                    User = new UserViewModel {
                        Email = x.User.Email,
                        Username = x.User.UserName
                    }
                });

            return messages;
        }

        // Testing purposes. TODO: Remove action.
        [HttpPost]
        public async Task Insert([FromBody] ChatMessageViewModel message)
        {
            await _messagesRepository.Insert(new ChatMessage
            {
                CreatedOn = DateTime.Now,
                UserId = message.UserId,
                Body = message.Body
            });
        }
    }
}
