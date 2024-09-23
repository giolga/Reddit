using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reddit.Data;
using Reddit.DTOs;
using Reddit.Model;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Reddit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentController(AppDbContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments()
        {
            return await _context.Comments.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);

            if (comment is null)
            {
                return NotFound();
            }

            return Ok(comment);
        }

        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(CommentDto commentDto)
        {
            if (commentDto is null)
            {
                return BadRequest();
            }

            var comment = new Comment()
            {
                AuthorName = commentDto.AuthorName,
                Content = commentDto.Content,
                Upvote = commentDto.Upvote,
                Downvote = commentDto.Downvote,
                PostId = commentDto.PostId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return Ok(comment);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Comment>> UpdateComment(int id, CommentUpdateDto commentDto)
        {
            if (id != commentDto.Id)
            {
                return BadRequest();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(commentDto.PostId);
            if (post == null)
            {
                return NotFound("Post not found!");
            }

            // Map the updated fields
            comment.AuthorName = commentDto.AuthorName;
            comment.Content = commentDto.Content;
            comment.Upvote = commentDto.Upvote;
            comment.Downvote = commentDto.Downvote;
            comment.PostId = commentDto.PostId;
            comment.Post = post;

            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (!CommentExists(id))
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

        [HttpDelete]
        public async Task<ActionResult<Comment>> DeleteComment(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);

            if(comment is null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok($"Comment with the id {id} deleted successfully!");
        }
        private bool CommentExists(int id)
        {
            return _context.Comments.Any(c => c.Id == id);
        }
    }
}
