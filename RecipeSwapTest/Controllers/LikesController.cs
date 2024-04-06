using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeSwapTest.Data;
using RecipeSwapTest.Models;

namespace RecipeSwapTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly RecipeSwapTestContext _context;

        public LikesController(RecipeSwapTestContext context)
        {
            _context = context;
        }

        // GET: api/Likes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Like>>> GetLikes()
        {
            return await _context.Likes.ToListAsync();
        }

        // GET: api/Likes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Like>> GetLike(int id)
        {
            var like = await _context.Likes.FindAsync(id);

            if (like == null)
            {
                return NotFound();
            }

            return like;
        }

        // PUT: api/Likes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLike(int id, Like like)
        {
            if (id != like.LikeId)
            {
                return BadRequest();
            }

            _context.Entry(like).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LikeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Likes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Like>> PostLike(int ProductId, int userId)
        {
            var like = new Like
            {
                RecipeId = ProductId,
                UserId = userId
            };
            if (await _context.Likes.AnyAsync(f => f.RecipeId == ProductId && f.UserId == userId))
            {
                await RemoveLike(ProductId, userId);
                return Ok(new { message = "Like rimosso." });
            }
            _context.Likes.Add(like);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLike", new { id = like.LikeId }, like);
        }
        
        [HttpDelete]
        [Route("remove")]
        public async Task<ActionResult<Like>> RemoveLike(int RecipeId, int userId)
        {
            var like = await _context.Likes.FirstOrDefaultAsync(x => x.RecipeId == RecipeId && x.UserId == userId);

            if (like == null)
            {
                return NotFound(new { message = "Like non trovato." });
            }

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Like rimosso." });
        }

        // DELETE: api/Likes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLike(int id)
        {
            var like = await _context.Likes.FindAsync(id);
            if (like == null)
            {
                return NotFound();
            }

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LikeExists(int id)
        {
            return _context.Likes.Any(e => e.LikeId == id);
        }
    }
}
