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
            var post = await _context.Posts.FirstOrDefaultAsync<Post>(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }

        [HttpPost]
        public async Task<ActionResult<Post>> PostPost(PostDto postDto)
        {
            if(postDto == null)
            {
                return NoContent();
            }

            var post = new Post()
            {
                Title = postDto.Title,
                Content = postDto.Content,
                AuthorId = postDto.AuthorId,
                CommunityId = postDto.CommunityId,
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return Ok($"Created successfully");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Post>> UpdatePost(int id, PostDto post)
        {
            var postDto = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);

            if(postDto == null)
            {
                return NotFound();
            }

            postDto.Title = post.Title;
            postDto.Content = post.Content;
            postDto.AuthorId = post.AuthorId;
            postDto.CommunityId = post.CommunityId;

            _context.Entry(postDto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok($"Post with the following Id: {id} updated successfully!");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Post>> DeletePost(int id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);

            if( post == null )
            {
                return NotFound($"No Post found with the following Id: {id}");
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return Ok($"Post with the Id: {id} deleted successfully");
        } 
    }
}
