using Reddit.Model;

namespace Reddit.Repositories
{
    public interface ICommentRepository
    {
        public Task<PagedList<Comment>> GetComments(int pageNumber, int pageSize, string? searchTerm = null, string? sortTerm = null, bool isAscending = true);
    }
}
