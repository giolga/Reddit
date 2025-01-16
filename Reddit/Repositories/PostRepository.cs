using Microsoft.Identity.Client;
using Reddit.Data;
using Reddit.Model;
using System.Linq;
using System.Linq.Expressions;

namespace Reddit.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext _context;

        public PostRepository(AppDbContext context)
        {
            this._context = context;
        }


        public async Task<PagedList<Post>> GetPosts(int pageNumber, int pageSize, string? searchTerm = null, string? sortTerm = null, bool isAscending = true)
        {
            if (pageNumber <= 0)
            {
                throw new ArgumentException("Page number must be greater than zero.", nameof(pageNumber));
            }

            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
            }

            var posts = _context.Posts.AsQueryable();

            // Filtration
            if (searchTerm != null)
            {
                posts = posts.Where(p => p.Title.Contains(searchTerm) || p.Content.Contains(searchTerm));
            }

            // Sort
            if (isAscending)
            {
                posts = posts.OrderBy(GetSortExpression(sortTerm));
            }
            else
            {
                posts = posts.OrderByDescending(GetSortExpression(sortTerm));
            }

            return await PagedList<Post>.CreateAsync(posts, pageNumber, pageSize);
        }

        private Expression<Func<Post, object>> GetSortExpression(string? sortTerm)
        {
            sortTerm = sortTerm?.ToLower(); //if not nullthen lowecase

            return sortTerm switch
            {
                "positivity" => post => (post.Upvote) / (post.Upvote + post.Downvote),
                "popular" => post => post.Upvote + post.Downvote,
                _ => post => post.Id //_ default case
            };
        }
    }
}
