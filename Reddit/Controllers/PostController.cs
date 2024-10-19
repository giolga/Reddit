using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reddit.Data;
using Reddit.DTOs;
using Reddit.Model;
using Reddit.Repositories;

namespace Reddit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPostRepository _postRepository;

        public PostController(AppDbContext context, IPostRepository postRepository)
        {
            this._context = context;
            this._postRepository = postRepository;
        }

        [HttpGet]
        public async Task<PagedList<Post>> GetPosts(int pageNumber = 1, int pageSize = 1, string? searchTerm = null, string? sortTerm = null, bool isAscending = true)
        {
            //var posts = _context.Posts.AsQueryable();
            //if (searchTerm != null)
            //{
            //    posts = posts.Where(p => p.Title.Contains(searchTerm) || p.Content.Contains(searchTerm));
            //}

            ////pagination
            //if (pageNumber < 1 || pageSize < 1)
            //{
            //    return BadRequest("Page number and page size must be greater than 0.");
            //}
            //var pages = await posts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            //return pages;

            return await _postRepository.GetPosts(pageNumber, pageSize, searchTerm, sortTerm, isAscending);
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

        [HttpPost("Upvote")]
        public async Task<IActionResult> UpvoteAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);

            if (post == null)
            {
                return NotFound(new { message = "Post not found" });
            }
            post.Upvote += 1;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Upvoted successfully", upvotes = post.Upvote });
        }

        [HttpPost("Downvote")]
        public async Task<IActionResult> DownvoteAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);

            if (post == null)
            {
                return NotFound(new { message = "Post not found" });
            }
            post.Downvote += 1;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Downvoted successfully", downvotes = post.Downvote });
        }

        [HttpPost]
        public async Task<ActionResult<Post>> PostPost(PostDto postDto)
        {
            if (postDto == null)
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

            if (postDto == null)
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

            if (post == null)
            {
                return NotFound($"No Post found with the following Id: {id}");
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return Ok($"Post with the Id: {id} deleted successfully");
        }
    }
}
