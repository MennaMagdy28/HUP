using HUP.BuildingBlocks.Application;
namespace HUP.Modules.AcademicStructure.Application.Features.GetLevels
{
    public class GetLevelsHandler
    {
        public async Task<Result<List<LevelsDto>>> HandleAsync(GetLevelsQuery request, CancellationToken token)
        {
            return Result<List<LevelsDto>>.Success(new List<LevelsDto>());
        }
    }
}
