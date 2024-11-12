using Reddit.Model;

namespace Reddit.Repositories
{
    public interface IUserRepository
    {
        public Task<PagedList<User>> GetUsers(int pageNumber, int pageSize, string? searchKey, string? sortKey = null, bool? isAscending = null);
    }
}
