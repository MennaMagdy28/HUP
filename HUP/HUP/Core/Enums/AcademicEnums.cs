namespace HUP.Core.Enums.AcademicEnums
{
    public enum AcademicStatus
    {
        Active,
        Graduated
    }

    public enum Gender
    {
        [Localized("Male", "ذكر")]
        Male,
        [Localized("Female", "أنثى")]
        Female
    }
    

    public enum ExamType
    {
        Midterm,
        Final
    }

    public enum EnrollmentStatus
    {
        Registered,
        InProgress,
        Completed,
        Incomplete,
        Failed,
        Dropped
    }

    public enum RequirementType
    {
       University,
       Faculty,
       Department
    }

    public enum UserRoles
    {
        Admin,
        Student,
        Instructor,
        DepartmentHead,
        FacultyDean
    }
    

    public enum AcademicTitle
    {
        Professor,
        AssistantProfessor
    }

    public enum FacultyTitle
    {
        [Localized("Faculty of Arts", "كلية الآداب")]
        FacultyOfArts,
        [Localized("Faculty of Home Economics", "كلية الآداب")]
        FacultyOfHomeEconomics,
        [Localized("Faculty of Education", "كلية الاقتصاد التربية")]
        FacultyOfEducation,
        [Localized("Faculty of Nursing", "كلية  التمريض")]
        FacultyOfNursing,
        [Localized("Faculty of Computers and Artificial Intelligence", "كلية الحاسبات والذكاء الاصطناعى")]
        FacultyOfComputingAndAI,
        [Localized("Faculty of Social Work", "كلية الخدمة الاجتماعية")]
        FacultyOfSocialWork,
        [Localized("Faculty of Pharmacy", "كلية  الصيدلة")]
        FacultyOfPharmacy,
        [Localized("Faculty of Medicine", "كلية الطب")]
        FacultyOfMedicine,
        [Localized("Faculty of Science", "كلية العلوم")]
        FacultyOfScience,
        [Localized("Faculty of Applied Arts", "كلية الفنون التطبيقية")]
        FacultyOfAppliedArts,
        [Localized("Faculty of Fine Arts", "كلية الفنون الجميلة")]
        FacultyOfFineArts,
        [Localized("Faculty of ", "كلية علوم الرياضة بنين")]
        FacultyOfSportsScience,
        [Localized("Faculty of Engineering Mataria", "كلية الهندسه (مطريه)")]
        FacultyOfEngineeringMataria,
        [Localized("Faculty of Engineering Helwan", "كلية الهندسة بحلوان")]
        FacultyOfEngineering,
        [Localized("Faculty of Commerce and Business Administration", "كلية التجارة وإدارة الأعمال ")]
        FacultyOfCommerceAndBusinessAdministration,
        [Localized("Faculty of Art Education", "كلية التربية الفنية")]
        FacultyOfArtEducation,
        [Localized("Technical Institute of Nursing", "معهد التمريض")]
        TechnicalInstituteOfNursing,
        [Localized("Faculty of Technology and Education", "كلية التكنولوجيا و التعليم")]
        FacultyOfTechnologyAndEducation,
        [Localized("Faculty of Law", "كلية الحقوق")]
        FacultyOfLaw,
        [Localized("Faculty of Music Education", "كلية التربية الموسيقية")]
        FacultyOfMusicEducation,
        [Localized("Faculty of Tourism and Hotels", "كلية السياحة و الفنادق")]
        FacultyOfTourismAndHotels,
        [Localized("Faculty of Nutrition Science", "كلية علوم التغذية")]
        FacultyOfNutritionScience
    }
}
