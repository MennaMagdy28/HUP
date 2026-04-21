using HUP.BuildingBlocks.Application;
namespace HUP.Modules.AcademicStructure.Application.Features.CreateFaculty
{
    public class CreateFacultyHandler
    {
        public async Task<Result<Guid>> HandleAsync(CreateFacultyQuery request, CancellationToken token)
        {
            return Result<Guid>.Success(Guid.NewGuid());
        }
    }
}
