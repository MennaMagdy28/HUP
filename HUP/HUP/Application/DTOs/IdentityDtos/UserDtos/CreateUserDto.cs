using HUP.Core.Entities.Identity;
using HUP.Core.Enums.IdentityEnums;
using HUP.Core.Models;

namespace HUP.Application.DTOs.IdentityDtos.UserDtos;

public class CreateUserDto
{
    public string NationalId { get; set; }
    public string? Email { get; set; }
    public string PasswordHash { get; set; }
    public LocalizedText FullName { get; set; }
    public Guid RoleId { get; set; }
    public CreatePersonalInfo PersonalInfo { get; set; }
    public ContactInfoDto ContactInfo { get; set; }
}

public class CreatePersonalInfo
{
    public Gender Gender { get; set; }
    public DateTime BirthDate { get; set; }
    public Religion Religion { get; set; }
    public Nationality Nationality { get; set; }
    public BirthPlace BirthPlace { get; set; }
}