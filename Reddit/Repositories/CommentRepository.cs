using Microsoft.Extensions.Hosting;
using Reddit.Data;
using Reddit.Model;
using System.Linq;
using System.Linq.Expressions;

namespace Reddit.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _context;

        public CommentRepository(AppDbContext context)
        {
            this._context = context;
        }

        public async Task<PagedList<Comment>> GetComments(int pageNumber, int pageSize, string? searchTerm = null, string? sortTerm = null, bool isAscending = true)
        {
            var comments = _context.Comments.AsQueryable();

            // Filtration
            if (searchTerm != null)
            {
                comments = comments.Where(c => c.Content.Contains(searchTerm));
            }

            // Sort
            if (isAscending)
            {
                comments = comments.OrderBy(GetSortExpression(sortTerm));
            }
            else
            {
                comments = comments.OrderByDescending(GetSortExpression(sortTerm));
            }

            return await PagedList<Comment>.CreateAsync(comments, pageNumber, pageSize);
        }

        private Expression<Func<Comment, object>> GetSortExpression(string? sortTerm)
        {
            sortTerm = sortTerm?.ToLower();

            return sortTerm switch
            {
                "positivity" => comment => (comment.Upvote) / (comment.Upvote + comment.Downvote),
                "popular" => comment => comment.Upvote + comment.Downvote,
                _ => comment => comment.Id //_ default case
            };
        }
    }
}
