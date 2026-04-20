using HUP.BuildingBlocks.Application;
namespace HUP.Modules.AcademicStructure.Application.Features.AddCreditHours
{
    public class AddCreditHoursHandler
    {
        public async Task<Result<Guid>> HandleAsync(AddCreditHHoursQuery request, CancellationToken token)
        {
            return Result<Guid>.Success(Guid.NewGuid());
        }
    }
}
