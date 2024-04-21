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

        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            return message;
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

        [HttpPost("SendMessage")]
        public async Task<ActionResult<Message>> SendMessage(messageDto m)
        {
            Message message = new Message
            {
                SenderUserId = m.SenderUserId,
                ReceiverUserId = m.ReceiverUserId,
                MessageContent = m.MessageContent,
                Timestamp = DateTime.Now,
                IsRead = false
            };
            message.Timestamp = DateTime.Now;
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMessage", new { id = message.MessageId }, message);
        }

        private bool MessageExists(int id)
        {
            return _context.Messages.Any(e => e.MessageId == id);
        }
    }
}
