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
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace RecipeSwapTest.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {

        private readonly RecipeSwapTestContext _context;
        private Cloudinary _cloudinary;

        public RecipesController(RecipeSwapTestContext context, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _context = context;

            var acc = new Account(
                cloudinaryConfig.Value.CloudName,
                cloudinaryConfig.Value.ApiKey,
                cloudinaryConfig.Value.ApiSecret);

            _cloudinary = new Cloudinary(acc);
        }



        // GET: api/Recipes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetRecipes()
        {
            var recipes = await _context.Recipes
                .OrderByDescending(r => r.CreationDate)
                .Take(500)
                 .Select(r => new
                 {
                     r.RecipeId,
                     r.Title,
                     r.Description,
                     r.Ingredients,
                     r.Instructions,
                     r.Image,
                     r.CreationDate,
                     User = new
                     {
                         r.User.UserId,
                         r.User.Username,
                         r.User.ProfilePicture,
                         follower = r.User.FollowerFollowerUsers.Select(f => new
                         {
                             f.FollowerUserId,
                             f.FollowedUserId
                         }),
                         following = r.User.FollowerFollowedUsers.Select(f => new
                         {
                             f.FollowerUserId,
                             f.FollowedUserId
                         })
                     },
                     Comments = r.Comments.Select(c => new
                     {
                         c.CommentId,
                         c.Comment1,
                         c.User.UserId,
                         c.User.Username,
                         c.User.ProfilePicture,
                     }),
                     Likes = r.Likes.Select(l => new
                     {
                         l.LikeId,
                         l.UserId,
                         l.RecipeId,
                     }).ToList(),
                     LikesCount = r.Likes.Count(),

                 })
                 .ToListAsync();

            return Ok(recipes);
        }

        // GET: api/Recipes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> GetRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);

            if (recipe == null)
            {
                return NotFound();
            }

            return recipe;
        }

        // PUT: api/Recipes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecipe(int id, Recipe recipe)
        {
            if (id != recipe.RecipeId)
            {
                return BadRequest();
            }

            _context.Entry(recipe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeExists(id))
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

        // POST: api/Recipes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Recipe>> PostRecipe([FromForm] RecipeDto recipeDto)
        {
            try
            {
                var imageUploadResult = new ImageUploadResult();
                if (recipeDto.Image != null)
                {
                    var uploadResult = await _cloudinary.UploadAsync(new ImageUploadParams
                    {
                        File = new FileDescription(recipeDto.Image.FileName, recipeDto.Image.OpenReadStream()),
                        // Optional transformations or folder settings here
                    });
                    imageUploadResult = uploadResult;
                }

                var recipe = new Recipe
                {
                    UserId = recipeDto.UserId,
                    Title = recipeDto.Title,
                    Instructions = recipeDto.Instructions,
                    Ingredients = recipeDto.Ingredients,
                    Description = recipeDto.Description,
                    CreationDate = DateTime.Now,
                    Image = imageUploadResult.SecureUrl?.ToString() ?? string.Empty,
                    IsVisible = true
                };

                _context.Recipes.Add(recipe);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetRecipe", new { id = recipe.RecipeId }, recipe);
            }
            catch (Exception ex)
            {
                // Log the exception here...
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        // DELETE: api/Recipes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            // Delete the related comments
            var comments = _context.Comments.Where(c => c.RecipeId == id);
            _context.Comments.RemoveRange(comments);

            // Delete the related likes
            var likes = _context.Likes.Where(l => l.RecipeId == id);
            _context.Likes.RemoveRange(likes);

            // Remove the recipe itself
            _context.Recipes.Remove(recipe);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.RecipeId == id);
        }

        ////////custom methods
        ///


        [HttpGet("GetRecipesByName/{name}")]
        public async Task<ActionResult<IEnumerable<object>>> GetRecipesByName(string name)
        {
            var recipes = await _context.Recipes
                .Where(r => r.Title.Contains(name) || r.User.Username.Contains(name))
                .Select(r => new
                {
                    r.RecipeId,
                    r.Title,
                    r.Description,
                    r.Ingredients,
                    r.Instructions,
                    r.Image,
                    r.CreationDate,
                    User = new
                    {
                        r.User.UserId,
                        r.User.Username,
                        r.User.ProfilePicture,
                        follower = r.User.FollowerFollowerUsers.Select(f => new
                        {
                            f.FollowerUserId,
                            f.FollowedUserId
                        }),
                        following = r.User.FollowerFollowedUsers.Select(f => new
                        {
                            f.FollowerUserId,
                            f.FollowedUserId
                        })
                    },
                    Comments = r.Comments.Select(c => new
                    {
                        c.CommentId,
                        c.Comment1,
                        c.User.UserId,
                        c.User.Username,
                        c.User.ProfilePicture,
                    }),
                    Likes = r.Likes.Select(l => new
                    {
                        l.LikeId,
                        l.UserId,
                        l.RecipeId,
                    }).ToList(),
                    LikesCount = r.Likes.Count(),

                })
                 .ToListAsync();

            return Ok(recipes);
        }

        //get the 3 most liked recipes
        [HttpGet]
        [Route("GetMostLikedRecipes")]
        public async Task<ActionResult<IEnumerable<object>>> GetMostLikedRecipes()
        {
            var recipes = await _context.Recipes
                .Select(r => new
                {
                    r.RecipeId,
                    r.Title,
                    r.Description,
                    r.Ingredients,
                    r.Instructions,
                    r.Image,
                    User = new
                    {
                        r.User.UserId,
                        r.User.Username,
                        r.User.ProfilePicture,
                    },
                    Comments = r.Comments.Select(c => new
                    {
                        c.CommentId,
                        c.Comment1,
                        User = new
                        {
                            c.User.UserId,
                            c.User.Username,
                            c.User.ProfilePicture,
                        }
                    }),
                    Likes = r.Likes.Count(),
                })
                .OrderByDescending(r => r.Likes)
                .Take(3)
                .ToListAsync();

            return Ok(recipes);
        }
    }
}

