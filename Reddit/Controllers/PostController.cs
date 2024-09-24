using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reddit.Data;
using Reddit.DTOs;
using Reddit.Model;

namespace Reddit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PostController(AppDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            return await _context.Posts.ToListAsync<Post>();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == id);

            if (post is null)
            {
                return NotFound($"Post with the id {id} not found! Try Again!");
            }

            return Ok(post);
        }

        [HttpPost]
        public async Task<ActionResult<Post>> Post(PostDto postDto)
        {
            if (postDto is null)
            {
                return NoContent();
            }

            var post = new Post()
            {
                Title = postDto.Title,
                Content = postDto.Content,
                AuthorName = postDto.AuthorName,
                CommunityName = postDto.CommunityName
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return Ok("Created Successfullt!");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Post>> UpdatePost(int id, Post post)
        {
            if (id != post.Id)
            {
                return BadRequest($"id {id} doesn't match with the post id {post.Id}");
            }

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (!PostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Updated Successfully!");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Post>> DeletePost(int id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);

            if (post is null)
            {
                return NoContent();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return Ok($"Post with the id {id} deleted Successfully!");
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

    }
}
