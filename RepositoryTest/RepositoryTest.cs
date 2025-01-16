using Microsoft.EntityFrameworkCore;
using Reddit.Data;
using Reddit.Model;
using Reddit.Repositories;

namespace RepositoryTests
{
    public class RepositoryTest
    {
        private IPostRepository GetPostRepository()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var dbContext = new AppDbContext(options);

            dbContext.Posts.Add(new Post { Title = "Title 1", Content = "Content 1", Upvote = 5, Downvote = 1 });
            dbContext.Posts.Add(new Post { Title = "Title 2", Content = "Content 1", Upvote = 12, Downvote = 1 });
            dbContext.Posts.Add(new Post { Title = "Title 3", Content = "Content 1", Upvote = 3, Downvote = 1 });
            dbContext.Posts.Add(new Post { Title = "Title 4", Content = "Content 1", Upvote = 221, Downvote = 1 }); // 221 / 222 = 0.9954954954954955
            dbContext.Posts.Add(new Post { Title = "Title 5", Content = "Content 1", Upvote = 5, Downvote = 2123 }); // 5 / 2123 = 0.002356344
            dbContext.SaveChanges();

            return new PostRepository(dbContext);
        }

        [Fact]
        public async void GetPosts_ReturnsCorrectPagination()
        {
            var postRepository = GetPostRepository();

            var posts = await postRepository.GetPosts(pageNumber: 1, pageSize: 2, searchTerm: null, sortTerm: null, isAscending: false);
            Assert.Equal(2, posts.Items.Count);
            Assert.Equal(5, posts.TotalCount);
            Assert.True(posts.HasNextPage);
            Assert.False(posts.HasPreviousPage);
        }

        [Fact]
        public async Task GetPosts_ReturnsCorrect()
        {
            var postsRepository = GetPostRepository();
            var posts = await postsRepository.GetPosts(pageNumber: 1, pageSize: 2, searchTerm: null, sortTerm: "popular", isAscending: false);
            Assert.Equal(2, posts.Items.Count);
            Assert.Equal(5, posts.TotalCount);
            Assert.True(posts.HasNextPage);
            Assert.False(posts.HasPreviousPage);
            //Positivity->Assert.Equal("Title 4", posts.Items.First().Title);
            Assert.Equal("Title 5", posts.Items.First().Title);
        }

        [Fact]
        public async Task GetPosts_InvalidPage_ThrowsArgumentException()
        {
            var repository = GetPostRepository();
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => repository.GetPosts(pageNumber: 0, pageSize: 10, searchTerm: null, sortTerm: null));
            Assert.Equal("pageNumber", exception.ParamName);
        }

        [Fact]
        public async Task GetPosts_InvalidPageSize_ThrowsArgumentOutOfRangeException()
        {
            var repository = GetPostRepository();
            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repository.GetPosts(pageNumber: 1, pageSize: 0, searchTerm: null, sortTerm: null));
            Assert.Equal("pageSize", exception.ParamName);
        }
    }
}