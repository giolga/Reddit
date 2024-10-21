using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using Reddit.Data;
using Reddit.DTOs;
using Reddit.Model;
using Reddit.Repositories;

namespace Reddit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ICommentRepository _commentRepository;
        public CommentsController(AppDbContext context, ICommentRepository commentRepository)
        {
            this._context = context;
            this._commentRepository = commentRepository;
        }

        [HttpGet]
        public async Task<PagedList<Comment>> GetComments(int pageNumber = 1, int pageSize = 3, string? searchTerm = null, string? sortTerm = null, bool isAscending = true)
        {
            //return await _context.Comments.ToListAsync();

            return await _commentRepository.GetComments(pageNumber, pageSize, searchTerm, sortTerm, isAscending);
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

        [HttpPost("Upvote")]
        public async Task<ActionResult<Comment>> UpvoteAsync(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null)
            {
                return NotFound(new { message = "Comment not found" });
            }

            comment.Upvote += 1;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Upvoted successfully", upvotes = comment.Upvote });
        }

        [HttpPost("Downvote")]
        public async Task<ActionResult<Comment>> DownvoteAsync(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null)
            {
                return NotFound(new { message = "Comment not found" });
            }

            comment.Downvote += 1;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Downvoted successfully", upvotes = comment.Downvote });
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
