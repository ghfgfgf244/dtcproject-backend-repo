using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using MongoDB.Driver;
using dtc.Infrastructure.Repositories.Blogs;
using dtc.Infrastructure.Persistence.MongoDB;
using dtc.Domain.Entities.Blogs;
using System;
using System.Threading;
using System.Collections.Generic;

namespace dtc.Tests.Repositories.MongoDB
{
    public class BlogRepositoryTests
    {
        [Fact]
        public async Task AddAsync_ShouldCallInsertOneAsync_OnMongoCollection()
        {
            // Arrange
            var mockCollection = new Mock<IMongoCollection<Blog>>();
            var mockContext = new Mock<MongoDBContext>(new Mock<Microsoft.Extensions.Configuration.IConfiguration>().Object);
            
            // Setup the property to return the mock collection
            mockContext.Setup(c => c.Blogs).Returns(mockCollection.Object);

            var repository = new BlogRepository(mockContext.Object);
            
            var blog = new Blog(
                title: "Test Blog",
                categoryId: 1,
                content: "Content",
                createdBy: Guid.NewGuid()
            );

            // Act
            await repository.AddAsync(blog);

            // Assert
            mockCollection.Verify(c => c.InsertOneAsync(blog, null, default(CancellationToken)), Times.Once);
        }
    }
}
