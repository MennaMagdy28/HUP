using System;
using Xunit;
using HUP.Core.Entities.Academics;
using HUP.Core.Enums.AcademicEnums;

namespace HUP.Tests.Domain
{
    public class StaffTests
    {
        [Fact]
        public void StaffCategory_CanAssignMultipleFlags()
        {
            // Arrange
            var staff = new Staff
            {
                UserId = Guid.NewGuid(),
                Category = StaffCategory.Academic | StaffCategory.Administrative
            };

            // Act & Assert
            Assert.True(staff.Category.HasFlag(StaffCategory.Academic));
            Assert.True(staff.Category.HasFlag(StaffCategory.Administrative));
            Assert.True(staff.Category.HasFlag(StaffCategory.None)); // 0 flag is always true
        }

        [Fact]
        public void StaffTitle_CanAssignInstructorTitle()
        {
            // Arrange
            var staff = new Staff
            {
                UserId = Guid.NewGuid(),
                Title = StaffTitle.Instructor
            };

            // Act & Assert
            Assert.Equal(StaffTitle.Instructor, staff.Title);
        }
    }
}
