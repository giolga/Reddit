using Microsoft.EntityFrameworkCore;
using Reddit.Data;
using Reddit.DTOs;
using Reddit.Model;
using Reddit.Repositories;

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

            List<User> users = new List<User>
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

            List<Community> communities = new List<Community>
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
                }
            };

            dbContext.Communities.AddRange(communities);
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
            var giorgi = dbContext.Users.FirstOrDefault(u => u.Id == pagedList.Items[0].OwnerId);

            Assert.Equal("Giorgi", giorgi.Name);

        }
    }
}