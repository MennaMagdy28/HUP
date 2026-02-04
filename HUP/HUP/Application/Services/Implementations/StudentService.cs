using HUP.Application.DTOs.AcademicDtos;
using HUP.Application.DTOs.AcademicDtos.Student;
using HUP.Application.Mappers;
using HUP.Application.Services.Interfaces;
using HUP.Common.Helpers;
using HUP.Core.Entities.Identity;
using HUP.Core.Enums;
using HUP.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace HUP.Application.Services.Implementations;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _hasher;

    public StudentService(IStudentRepository studentRepository, IUserRepository userRepository, IPasswordHasher<User> hasher)
    {
        _studentRepository = studentRepository;
        _userRepository = userRepository;
        _hasher = hasher;
    }
    
    public async Task<StudentProfileDto> GetStudentProfile(Guid userId , string lang)
    {
        var student = await _studentRepository.GetByIdReadOnly(userId);
        if (student == null)
            return null;
        var profile = StudentMapper.ToStudentProfile(student, lang);
        return profile;
    }
    
    public async Task AddStudent(CreateStudentDto dto)
    {
        var student = StudentMapper.ToCreateStudent(dto);
        var user = student.User;
        user.CreatedAt = DateTime.UtcNow;
        var hashed = _hasher.HashPassword(user, dto.UserInfo.PasswordHash);
        user.PasswordHash = hashed;
        user.Id = new Guid();
        await _userRepository.AddAsync(user);
        student.UserId = user.Id;
        await _studentRepository.AddAsync(student);
        await _studentRepository.SaveChangesAsync();
    }
    
    public async Task<bool> UpdateStudentStatus(StudentStatusDto statusDto)
    {
        var student = await _studentRepository.GetByIdTracking(statusDto.StudentId);
        if (student == null)
            return false;
        student = StudentMapper.UpdateStatus(statusDto);
        await _studentRepository.SaveChangesAsync();
        return true;
    }
}
