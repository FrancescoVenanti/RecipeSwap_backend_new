using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeSwapTest.Data;
using RecipeSwapTest.Models;

namespace RecipeSwapTest.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly RecipeSwapTestContext _context;

        public MessagesController(RecipeSwapTestContext context)
        {
            _context = context;
        }

        // GET: api/Messages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        {
            return await _context.Messages.ToListAsync();
        }

        [HttpGet("GetMessagesByUser/{userId}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessagesByUser(int userId)
        {
            // Return a list of messages where the sender or receiver is the user
            var messages = await _context.Messages
                .Where(m => m.SenderUserId == userId || m.ReceiverUserId == userId)
                .Select(m => new Message
                {
                    MessageId = m.MessageId,
                    SenderUserId = m.SenderUserId,
                    ReceiverUserId = m.ReceiverUserId,
                    MessageContent = m.MessageContent,
                    Timestamp = m.Timestamp,
                    SenderUser = new User
                    {
                        UserId = m.SenderUser.UserId,
                        Username = m.SenderUser.Username,
                        ProfilePicture = m.SenderUser.ProfilePicture
                    },
                    ReceiverUser = new User
                    {
                        UserId = m.ReceiverUser.UserId,
                        Username = m.ReceiverUser.Username,
                        ProfilePicture = m.ReceiverUser.ProfilePicture
                    }
                }).ToListAsync();

            return messages;
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.MessageId == id);
        }
    }
}
