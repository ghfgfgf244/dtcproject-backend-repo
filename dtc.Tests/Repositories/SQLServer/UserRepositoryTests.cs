// using System.Threading.Tasks;
// using Xunit;
// using FluentAssertions;
// using Microsoft.EntityFrameworkCore;
// using dtc.Infrastructure.Pesistence.SQLServer;
// using dtc.Infrastructure.Repositories.Permissions;
// using dtc.Domain.Entities.Permissions;
// using dtc.Domain.ValueObjects;
// using System.Linq;
// using System;

// namespace dtc.Tests.Repositories.SQLServer
// {
//     public class UserRepositoryTests
//     {
//         private SQLDBContext GetInMemoryContext()
//         {
//             var options = new DbContextOptionsBuilder<SQLDBContext>()
//                 .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//                 .Options;

//             return new SQLDBContext(options);
//         }

//         [Fact]
//         public async Task AddAsync_ShouldAddUser_WhenValidEntity()
//         {
//             // Arrange
//             using var context = GetInMemoryContext();
//             var repository = new UserRepository(context);

//             var user = new User(
//                 Email.Create("john.doe@example.com"),
//                 "password123",
//                 "John Doe",
//                 "john_doe",
//                 "password123",
//                 "John",
//                 "Doe",
//                 Email.Create("john.doe@example.com"),
//                 PhoneNumber.Create("0123456789")
//             );

//             // Act
//             await repository.AddAsync(user);
//             await context.SaveChangesAsync();

//             // Assert
//             var users = await context.Users.ToListAsync();
//             users.Should().HaveCount(1);
//             users.First().Email.Value.Should().Be("john.doe@example.com");
//         }
//     }
// }
