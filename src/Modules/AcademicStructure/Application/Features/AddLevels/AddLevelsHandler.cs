using HUP.BuildingBlocks.Application;
namespace HUP.Modules.AcademicStructure.Application.Features.AddLevels
{
    public class AddLevelsHandler
    {
        public async Task<Result<Guid>> HandleAsync(AddLevelsQuery request, CancellationToken token)
        {
            return Result<Guid>.Success(Guid.NewGuid());
        }
    }
}
