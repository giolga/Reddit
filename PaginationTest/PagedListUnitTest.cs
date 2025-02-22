using Microsoft.EntityFrameworkCore;
using Reddit.Data;
using Reddit.DTOs;
using Reddit.Model;
using Reddit.Repositories;
using System.Linq;

namespace PaginationTest
{
    public class PagedListUnitTest
    {
        public async Task<PagedList<T>> Pagination<T>(IQueryable<T> items, int pageNumber, int pageSize)
        {
            return await PagedList<T>.CreateAsync(items, pageNumber, pageSize);
        }

        [Fact]
        public async Task User_test()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var dbContext = new AppDbContext(options);
            List<User> users = Users();

            dbContext.Users.AddRange(users);
            await dbContext.SaveChangesAsync(); // Await the SaveChangesAsync call

            var queryableUsers = dbContext.Users.AsQueryable();

            int pageNumber = 1;
            int pageSize = 5;

            // Act: Call the Pagination method
            var pagedList = await Pagination(queryableUsers, pageNumber, pageSize);

            // Assert: Verify the results
            Assert.Equal(pageNumber, pagedList.PageNumber);
            Assert.Equal(pageSize, pagedList.PageSize);
            Assert.Equal(users.Count, pagedList.TotalCount);
            Assert.Equal(pageSize, pagedList.Items.Count);
            Assert.True(pagedList.HasNextPage);
            Assert.False(pagedList.HasPreviousPage);

            // Optionally, verify the content of the first page
            Assert.Contains(pagedList.Items, u => u.Name == "Giorgi");
            Assert.Contains(pagedList.Items, u => u.Name == "Kamaru");
            Assert.Equal("Giorgi", pagedList.Items[0].Name);
        }

        [Fact]
        public async Task Community_test()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var dbContext = new AppDbContext(options);

            List<Community> communities = Communities();

            dbContext.Communities.AddRange(communities);
            dbContext.Users.AddRange(Users());
            await dbContext.SaveChangesAsync();

            var queryableCommunities = dbContext.Communities.AsQueryable();

            int pageNumber = 1;
            int pageSize = 3;

            // Act: Call the Pagination method
            var pagedList = await Pagination(queryableCommunities, pageNumber, pageSize);

            Assert.Equal(pageNumber, pagedList.PageNumber);
            Assert.Equal(pageSize, pagedList.PageSize);
            Assert.Equal(communities.Count, pagedList.TotalCount);
            Assert.Equal(pageSize, pagedList.Items.Count);
            Assert.True(pagedList.HasNextPage);
            Assert.False(pagedList.HasPreviousPage);

            Assert.Equal(5, pagedList.Items[2].OwnerId); //owner id of the owner in the first page should be 5 (el cucuy)

            var person = await dbContext.Users.FirstAsync(u => u.Id == pagedList.Items[0].OwnerId);
            Assert.Equal(1, person.Id);

            int pageNumber2 = 5;
            int pageSize2 = 2;

            var pagedList2 = await Pagination(queryableCommunities, pageNumber2, pageSize2); //Page should have 1 article
            Assert.Equal(1, pagedList2.Items.Count);
            Assert.False(pagedList2.HasNextPage);
            Assert.True(pagedList2.HasPreviousPage);
            Assert.Equal("Networking Community", pagedList2.Items[0].Name);
            Assert.NotEqual(communities.Count, pagedList2.Items.Count);
        }

        [Fact]
        public async Task Post_test()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var dbContext = new AppDbContext(options);

            List<User> users = Users();
            List<Community> communities = Communities();
            List<Post> posts = Posts();

            dbContext.Users.AddRange(users);
            dbContext.Communities.AddRange(communities);
            dbContext.Posts.AddRange(posts);
            await dbContext.SaveChangesAsync();

            Assert.Equal(12, posts.Count);

            var queryableCommunities = dbContext.Posts.AsQueryable();

            int pageNumber = 1;
            int pageSize = 6;

            var pagedList = await Pagination(queryableCommunities, pageNumber, pageSize);
            Assert.Equal(6, pagedList.Items.Count);
            Assert.True(pagedList.HasNextPage);
            Assert.False(pagedList.HasPreviousPage);
            Assert.Equal("Networking Community", (await dbContext.Communities.FirstAsync(c => c.Id == pagedList.Items[5].CommunityId)).Name);
        }

        [Fact]
        public async Task Comment_test()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var dbContext = new AppDbContext(options);

            List<User> users = Users();
            List<Community> communities = Communities();
            List<Post> posts = Posts();
            List<Comment> comments = Comments();

            dbContext.Users.AddRange(users);
            dbContext.Communities.AddRange(communities);
            dbContext.Posts.AddRange(posts);
            dbContext.Comments.AddRange(comments);
            await dbContext.SaveChangesAsync();

            Assert.Equal(12, dbContext.Comments.ToList().Count);

            var queryableComments = dbContext.Comments.AsQueryable();

            int pageNumber = 3;
            int pageSize = 2;

            var pagedList = await Pagination(queryableComments, pageNumber, pageSize);
            Assert.Equal(2, pagedList.Items[0].AuthorId);
            Assert.Equal("Merab deserves more respect! #MMA", dbContext.Posts.FirstOrDefault(p => p.Id == pagedList.Items[0].PostId)!.Title);
            Assert.True(pagedList.HasPreviousPage);
            Assert.True(pagedList.HasPreviousPage);
            Assert.Equal("Kamaru", dbContext.Users.FirstOrDefault(u => u.Id == pagedList.Items[0].AuthorId)!.Name);
        }

        private List<User> Users()
        {
            return new List<User>
                {
                    new User { Name = "Giorgi", Email = "kumi@gmail.com" },
                    new User { Name = "Kamaru", Email = "nightmare@gmail.com" },
                    new User { Name = "Israel", Email = "the_last_stylebender@gmail.com" },
                    new User { Name = "Alex", Email = "poatan@gmail.com" },
                    new User { Name = "Tony", Email = "el_cucuy@gmail.com" },
                    new User { Name = "Charles", Email = "do_bronx@gmail.com" },
                    new User { Name = "Alex", Email = "the_great@gmail.com" },
                    new User { Name = "Anderson", Email = "spider@gmail.com" },
                    new User { Name = "Jon", Email = "goat@gmail.com" }
                };
        } // 9 users

        private List<Community> Communities()
        {
            return new List<Community>
            {
                new Community
                {
                    Name = "MMA Community",
                    Description = "Merab THE GOAT",
                    OwnerId = 1 //Giorgi
                },
                new Community
                {
                    Name = "Alex Pereira Training Sessions",
                    Description = "CHAMA",
                    OwnerId = 4 //Poatan owner
                },
                new Community
                {
                    Name = "C# Masterclass",
                    Description = "This is the best C# community to learn this prog language",
                    OwnerId = 5 // Tony                   
                },
                new Community
                {
                    Name = "Culinary community",
                    Description = "Interested in cooking? Let's cook with Volk!",
                    OwnerId = 7 //the great
                },
                new Community
                {
                    Name = "Striking masterclass",
                    Description = "Learn Israel's question mark kicks within 5 minutes!",
                    OwnerId = 3 //izzy
                },
                new Community
                {
                    Name = "GOAT community",
                    Description = "Jon Jones teaches us how to become the best!",
                    OwnerId = 9
                },
                new Community
                {
                    Name = "Head movement community",
                    Description = "Learn how to become Neo with Spider",
                    OwnerId = 8
                },
                new Community
                {
                    Name = "Muay Thai",
                    Description = "OOOooooooOoooooooowwweeeeeeeeeeeee",
                    OwnerId = 1
                },
                new Community
                {
                    Name = "Networking Community",
                    Description = "Cisco",
                    OwnerId = 1
                }
            };
        }

        private List<Post> Posts()
        {
            return new List<Post>
            {
                new Post
                {
                    Title = "Poatan has a vicious left hook!",
                    Content = "MMA/Kickboxing",
                    AuthorId = 1, // Giorgi
                    CommunityId = 2 // Poatan's community
                },
                new Post
                {
                    Title = "Let's enjoy eating delicious burgers with Volk!",
                    Content = "Cook, chef, Culinary",
                    AuthorId = 9, // Jon Jones
                    CommunityId = 4 // Volk's community
                },
                new Post
                {
                    Title = "Today was a good day. #question_mark_kicks #izzy #goat",
                    Content = "Adesanya masterclass",
                    AuthorId = 5, // Tony
                    CommunityId = 5 // Israel's community
                },
                new Post
                {
                    Title = "I've just learned head movement from the legend himself",
                    Content = "Training with the Spider",
                    AuthorId = 6, // Charles
                    CommunityId = 7 // Anderson's community
                },
                new Post
                {
                    Title = "Merab deserves more respect! #MMA",
                    Content = "The bantamweight king deserves a shot!",
                    AuthorId = 3, // Israel
                    CommunityId = 1 // MMA community
                },
                new Post
                {
                    Title = "Networking is the future! Let's dive into Cisco essentials.",
                    Content = "Exploring CCNA certifications and beyond.",
                    AuthorId = 1, // Giorgi
                    CommunityId = 9 // Networking community
                },
                new Post
                {
                    Title = "Muay Thai techniques to elevate your game!",
                    Content = "Focus on clinching and sweeping techniques.",
                    AuthorId = 4, // Poatan
                    CommunityId = 8 // Muay Thai community
                },
                new Post
                {
                    Title = "Mastering C# LINQ Queries",
                    Content = "Learn to manipulate data like a pro with LINQ.",
                    AuthorId = 5, // Tony
                    CommunityId = 3 // C# masterclass
                },
                new Post
                {
                    Title = "Striking Techniques for MMA",
                    Content = "The fundamentals of jabs and leg kicks.",
                    AuthorId = 7, // Alex (the great)
                    CommunityId = 5 // Striking masterclass
                },
                new Post
                {
                    Title = "How to defend against the left hook",
                    Content = "Analyzing the most dangerous punches in combat sports.",
                    AuthorId = 2, // Kamaru
                    CommunityId = 2 // Poatan's community
                },
                new Post
                {
                    Title = "Spider's Tips on Timing and Reflexes",
                    Content = "Improve your reaction time with Anderson Silva's methods.",
                    AuthorId = 8, // Anderson
                    CommunityId = 7 // Head movement community
                },
                new Post
                {
                    Title = "Best practices for C# async programming",
                    Content = "Simplify your code and improve performance with async/await.",
                    AuthorId = 1, // Giorgi
                    CommunityId = 3 // C# masterclass
                }
            };
        }


        private List<Comment> Comments()
        {
            return new List<Comment>
            {
                    new Comment
                    {
                        Content = "Poatan is a monster in the octagon! That left hook is terrifying!",
                        AuthorId = 3, // Israel
                        PostId = 1 // Post about Poatan's left hook
                    },
                    new Comment
                    {
                        Content = "Burgers with Volk sounds amazing! What's your favorite recipe?",
                        AuthorId = 5, // Tony
                        PostId = 2 // Post about burgers with Volk
                    },
                    new Comment
                    {
                        Content = "Question mark kicks are no joke, great session today!",
                        AuthorId = 7, // Alex (the great)
                        PostId = 3 // Post about Israel's kicks
                    },
                    new Comment
                    {
                        Content = "Spider's head movement is unreal, he makes it look so easy!",
                        AuthorId = 4, // Poatan
                        PostId = 4 // Post about head movement with Spider
                    },
                    new Comment
                    {
                        Content = "Merab has earned his place at the top. Bantamweight division needs to take notice!",
                        AuthorId = 2, // Kamaru
                        PostId = 5 // Post about Merab
                    },
                    new Comment
                    {
                        Content = "Networking is indeed the future, let's master it together!",
                        AuthorId = 9, // Jon Jones
                        PostId = 6 // Post about networking and Cisco
                    },
                    new Comment
                    {
                        Content = "Clinching and sweeping are my favorite techniques in Muay Thai!",
                        AuthorId = 6, // Charles
                        PostId = 7 // Post about Muay Thai techniques
                    },
                    new Comment
                    {
                        Content = "This LINQ guide is amazing! Helped me optimize my code.",
                        AuthorId = 8, // Anderson
                        PostId = 8 // Post about C# LINQ
                    },
                    new Comment
                    {
                        Content = "Jabs and leg kicks are underrated tools in MMA. Great post!",
                        AuthorId = 4, // Poatan
                        PostId = 9 // Post about striking techniques
                    },
                    new Comment
                    {
                        Content = "Defending the left hook is tough, but a high guard helps a lot!",
                        AuthorId = 5, // Tony
                        PostId = 10 // Post about defending against the left hook
                    },
                    new Comment
                    {
                        Content = "Timing is everything in fighting. Spider's tips are gold!",
                        AuthorId = 2, // Kamaru
                        PostId = 11 // Post about timing and reflexes
                    },
                    new Comment
                    {
                        Content = "Async programming has made my life so much easier. Thanks for sharing!",
                        AuthorId = 7, // Alex (the great)
                        PostId = 12 // Post about C# async programming
                    }
            };
        }
    }
}