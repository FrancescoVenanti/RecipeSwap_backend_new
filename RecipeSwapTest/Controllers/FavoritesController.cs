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
using RecipeSwapTest.ViewModels;

namespace RecipeSwapTest.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly RecipeSwapTestContext _context;

        public FavoritesController(RecipeSwapTestContext context)
        {
            _context = context;
        }

        // GET: api/Favorites
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Favorites>>> GetFavorites()
        {
            return await _context.Favorites.ToListAsync();
        }
        //get logged user favorites
        [HttpGet]
        [Route("getUserFavorites")]
        public async Task<ActionResult<IEnumerable<object>>> GetUserFavorites(int userId)
        {
            return await _context.Favorites.Where(f => f.UserId == userId)
                .Include(f => f.Recipe)
                    .ThenInclude(r => r.User)
                .Include(f => f.Recipe)
                    .ThenInclude(r => r.Likes)
                .Select(f => new
                {
                    f.FavoriteId,
                    f.UserId,
                    f.RecipeId,
                    Recipe = new
                    {
                        f.Recipe.RecipeId,
                        f.Recipe.UserId,
                        f.Recipe.Title,
                        f.Recipe.Description,
                        f.Recipe.Ingredients,
                        f.Recipe.Instructions,
                        f.Recipe.CreationDate,
                        f.Recipe.Image,
                        f.Recipe.IsVisible,
                        User = new
                        {
                            f.Recipe.User.UserId,
                            f.Recipe.User.Username,
                            f.Recipe.User.ProfilePicture,
                        },
                        Likes = f.Recipe.Likes.Select(l => new
                        {
                            l.LikeId,
                            l.UserId,
                            l.RecipeId,
                        }).ToList(),
                        LikesCount = f.Recipe.Likes.Count(),
                    }
                })
                .ToListAsync();
        }




        //add to favorites
        [HttpPost]
        [Route("addFavorite")]
        public async Task<ActionResult<Favorites>> PostFavorites(int ProductId, int userId)
        {
            
            var favorite = new Favorites
            {
                RecipeId = ProductId,
                UserId = userId
            };
            if (await _context.Favorites.AnyAsync(f => f.RecipeId == ProductId && f.UserId == userId))
            {
                await RemoveFromFavorite(ProductId, userId);
                return Ok(new { message = "Articolo rimosso dai preferiti." });
            }
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFavorites", new { id = favorite.FavoriteId }, favorite);
        }

        [HttpPost]
        [Route("remove")]
        public async Task<ActionResult<Favorites>> RemoveFromFavorite(int RecipeId, int userId)
        {
            var Favorite = await _context.Favorites.FirstOrDefaultAsync(x => x.RecipeId == RecipeId && x.UserId == userId);

            if (Favorite == null)
            {
                return NotFound(new { message = "Articolo non trovato nei preferiti." });
            }

            _context.Favorites.Remove(Favorite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Articolo rimosso dai preferiti." });
        }

    }
}
