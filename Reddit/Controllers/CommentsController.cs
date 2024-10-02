using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Reddit.Data;
using Reddit.DTOs;
using Reddit.Model;

namespace Reddit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentsController(AppDbContext context)
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
            if (commentDto == null)
            {
                return NoContent();
            }

            var comment = new Comment()
            {
                Content = commentDto.Content,
                PostId = commentDto.PostId,
                AuthorId = commentDto.AuthorId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok(comment);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Comment>> UpdateComment(CommentDto commentDto, int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync<Comment>(c => c.Id == id);

            if (comment is null)
            {
                return NotFound();
            }

            comment.Content = commentDto.Content;
            comment.PostId = commentDto.PostId;
            comment.AuthorId = commentDto.AuthorId;

            _context.Entry(comment).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok($"Comment with the Id: {id} updated successfully!");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Comment>> DeleteComment(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync<Comment>(c => c.Id == id);

            if (comment is null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return Ok($"Comment with the Id: {id} Deleted Successfully!");
        }

    }
}
