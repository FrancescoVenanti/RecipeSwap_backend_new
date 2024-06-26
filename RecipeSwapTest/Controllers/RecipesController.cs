﻿using System;
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
    public class RecipesController : ControllerBase
    {
        private readonly RecipeSwapTestContext _context;

        public RecipesController(RecipeSwapTestContext context)
        {
            _context = context;
        }
        
        // GET: api/Recipes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetRecipes()
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
                        c.User.UserId,
                        c.User.Username,
                        c.User.ProfilePicture,
                    }),
                    Likes = r.Likes.Count(),

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
        public async Task<ActionResult<Recipe>> PostRecipe(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRecipe", new { id = recipe.RecipeId }, recipe);
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
                    .Where(r => r.Title.Contains(name))
                    .Select(r => new
                    {
                        Recipe = new
                        {
                            r.RecipeId,
                            r.Title,
                            r.Ingredients,
                            r.Description,
                            r.Instructions,
                            r.Image,
                            Users = new
                            {
                                r.User.UserId, // Assuming each recipe is associated with one user
                                r.User.Username,
                                r.User.ProfilePicture
                            },
                            Comments = r.Comments.Select(c => new
                            {
                                c.CommentId,
                                Comment = c.Comment1, // Assuming 'Comment' is the actual name of the property holding the comment text
                                User = new // Changed from Users to User, assuming each comment is made by a single user
                                {
                                    c.User.UserId,
                                    c.User.Username,
                                    c.User.ProfilePicture
                                }
                            }),
                        },
                        LikesCount = r.Likes.Count
                    })
                    .OrderByDescending(r => r.LikesCount)
                    .ToListAsync();

                return Ok(recipes);
            }
        }
    }

