using HUP.Core.Enums.IdentityEnums;
using Gender = HUP.Core.Enums.AcademicEnums.Gender;

namespace HUP.Core.Entities.Identity
{
    public class UserPersonalInfo
    {
        public Guid UserId { get; set; }
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public Religion Religion { get; set; }
        public Nationality Nationality { get; set; }
        public BirthPlace BirthPlace { get; set; }

    }
}