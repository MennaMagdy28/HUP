using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using HUP.Core.Entities.Academics;
using HUP.Core.Entities.Identity;
using HUP.Core.Enums;
using HUP.Data;

namespace HUP.Tests
{
    public class StaffEfQueryTests
    {
        private HupDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<HupDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new HupDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task QueryStaff_HasFlag_TranslatesAndFiltersCorrectly()
        {
            // Arrange
            using var context = GetDbContext();

            var user1 = new User { Id = Guid.NewGuid(), FullName = "Academic User", PasswordHash = "x", NationalId = "1" };
            var user2 = new User { Id = Guid.NewGuid(), FullName = "Admin User", PasswordHash = "x", NationalId = "2" };
            var user3 = new User { Id = Guid.NewGuid(), FullName = "Mixed User", PasswordHash = "x", NationalId = "3" };

            var staff1 = new Staff { UserId = user1.Id, User = user1, Category = StaffCategory.Academic };
            var staff2 = new Staff { UserId = user2.Id, User = user2, Category = StaffCategory.Administrative };
            var staff3 = new Staff { UserId = user3.Id, User = user3, Category = StaffCategory.Academic | StaffCategory.Administrative };

            context.Staff.AddRange(staff1, staff2, staff3);
            await context.SaveChangesAsync();

            // Act
            var academicStaff = await context.Staff
                .Where(s => s.Category.HasFlag(StaffCategory.Academic))
                .ToListAsync();

            var administrativeStaff = await context.Staff
                .Where(s => s.Category.HasFlag(StaffCategory.Administrative))
                .ToListAsync();

            // Assert
            Assert.Equal(2, academicStaff.Count);
            Assert.Contains(academicStaff, s => s.UserId == user1.Id);
            Assert.Contains(academicStaff, s => s.UserId == user3.Id);

            Assert.Equal(2, administrativeStaff.Count);
            Assert.Contains(administrativeStaff, s => s.UserId == user2.Id);
            Assert.Contains(administrativeStaff, s => s.UserId == user3.Id);
        }
    }
}
