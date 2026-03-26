using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using HUP.Core.Entities.Academics;
using HUP.Core.Enums.AcademicEnums;

namespace HUP.Tests.Application
{
    public class StaffServiceTests
    {
        [Fact]
        public void FilterAcademicStaff_ReturnsOnlyAcademic()
        {
            // Arrange
            var staffList = new List<Staff>
            {
                new Staff { UserId = Guid.NewGuid(), Category = StaffCategory.Academic },
                new Staff { UserId = Guid.NewGuid(), Category = StaffCategory.Administrative },
                new Staff { UserId = Guid.NewGuid(), Category = StaffCategory.Academic | StaffCategory.Administrative },
                new Staff { UserId = Guid.NewGuid(), Category = StaffCategory.None }
            };

            // Act
            var academicStaff = staffList.Where(s => s.Category.HasFlag(StaffCategory.Academic)).ToList();

            // Assert
            Assert.Equal(2, academicStaff.Count);
            Assert.All(academicStaff, s => Assert.True(s.Category.HasFlag(StaffCategory.Academic)));
        }

        [Fact]
        public void FilterInstructors_ReturnsOnlyInstructors()
        {
            // Arrange
            var staffList = new List<Staff>
            {
                new Staff { UserId = Guid.NewGuid(), Title = StaffTitle.Instructor },
                new Staff { UserId = Guid.NewGuid(), Title = StaffTitle.Professor },
                new Staff { UserId = Guid.NewGuid(), Title = StaffTitle.AdminStaff },
                new Staff { UserId = Guid.NewGuid(), Title = StaffTitle.Instructor }
            };

            // Act
            var instructors = staffList.Where(s => s.Title == StaffTitle.Instructor).ToList();

            // Assert
            Assert.Equal(2, instructors.Count);
            Assert.All(instructors, s => Assert.Equal(StaffTitle.Instructor, s.Title));
        }
    }
}
