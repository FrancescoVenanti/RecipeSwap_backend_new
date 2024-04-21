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
    public class FollowersController : ControllerBase
    {
        private readonly RecipeSwapTestContext _context;

        public FollowersController(RecipeSwapTestContext context)
        {
            _context = context;
        }


        //todo do i need it?
        [HttpGet]
        [Route("GetFollowers/{id}")]
        public async Task<ActionResult<IEnumerable<Follower>>> GetFollowers(int id)
        {
            return await _context.Followers.Where(f => f.FollowerUserId == id).ToListAsync();
        }

        //todo do i need it?
        [HttpGet]
        [Route("GetFollowing/{id}")]
        public async Task<ActionResult<IEnumerable<Follower>>> GetFollowing(int id)
        {
            return await _context.Followers.Where(f => f.FollowedUserId == id).ToListAsync();
        }

        [HttpGet]
        [Route("GetUserFollowing/{id}")]
        public async Task<ActionResult<IEnumerable<Follower>>> GetUserFollowing(int id)
        {
            var follow = await _context.Followers
                .Where(f => f.FollowerUserId == id)
                .Select(f => new Follower
                {
                    FollowerId = f.FollowerId,
                    FollowerUserId = f.FollowerUserId,
                    FollowedUserId = f.FollowedUserId,
                    FollowDate = f.FollowDate,
                    FollowedUser = new User
                    {
                        UserId = f.FollowedUser.UserId,
                        Username = f.FollowedUser.Username,
                        ProfilePicture = f.FollowedUser.ProfilePicture
                    }
                }).ToListAsync();

            return follow;
        }

        [HttpPost]
        [Route("followUnfollow")]
        public async Task<ActionResult<Follower>> followUnfollow( int followerId, int followedId)
        {
            // Check if both users exist
            var followerExists = await _context.Users.AnyAsync(u => u.UserId == followerId);
            var followedExists = await _context.Users.AnyAsync(u => u.UserId == followedId);

            if (!followerExists || !followedExists)
            {
                return BadRequest("One or both users do not exist.");
            }

            // Check if the follower already exists
            var existingFollower = await _context.Followers.FirstOrDefaultAsync(f => f.FollowerUserId == followerId && f.FollowedUserId == followedId);

            if (existingFollower != null)
            {
                // Unfollow the user
                _context.Followers.Remove(existingFollower);
                await _context.SaveChangesAsync();

                return Ok("Unfollowed");
            }
            else
            {
                // Create a new follower
                Follower follower = new Follower
                {
                    FollowerUserId = followerId,
                    FollowedUserId = followedId,
                    FollowDate = DateTime.Now
                };

                _context.Followers.Add(follower);
                await _context.SaveChangesAsync();

                return Ok("followed");
            }
        }

        private bool FollowerExists(int id)
        {
            return _context.Followers.Any(e => e.FollowerId == id);
        }
    }
}
