using Reddit.Data;
using Reddit.Model;
using System.Linq.Expressions;

namespace Reddit.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            this._context = context;
        }

        public async Task<PagedList<User>> GetUsers(int pageNumber, int pageSize, string? searchKey, string? sortKey = null, bool? isAscending = null)
        {
            var users = _context.Users.AsQueryable();
            // Validate 
            // Filtration
            if (searchKey != null)
            {
                users = users.Where(u => u.Email.Contains(searchKey) || u.Name.Contains(searchKey));
            }

            // Sort
            if (isAscending != null)
            {
                if (isAscending == true)
                {
                    users = users.OrderBy(GetSortExpression(sortKey));
                }
                else
                {
                    users = users.OrderByDescending(GetSortExpression(sortKey));
                }
            }

            return await PagedList<User>.CreateAsync(users, pageNumber, pageSize);
        }

        private Expression<Func<User, object>> GetSortExpression(string? sortKey)
        {
            sortKey = sortKey?.ToLower();
            return sortKey switch
            {
                "numberofpages" => user => user.Posts.Count,
                _ => post => post.Id
            };
        }
    }
}
