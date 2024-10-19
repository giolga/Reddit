using Reddit.Model;

namespace Reddit.Repositories
{
    public interface IPostRepository
    {
        public Task<PagedList<Post>> GetPosts(int pageNumber, int pageSize, string? searchTerm = null, string? sortTerm = null, bool isAscending = true);
    }
}
