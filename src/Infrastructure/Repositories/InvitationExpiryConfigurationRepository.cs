using Application.Operation.Common.Repositories;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;
using Tawtheef.Infrastructure.Repositories.Base;

namespace Tawtheef.Infrastructure.Repositories;

public class InvitationExpiryConfigurationRepository(IGenericRepository<InvitationExpiryConfiguration> repository)
    : BaseRepository<InvitationExpiryConfiguration>(repository), IInvitationExpiryConfigurationRepository
{
    public async Task<IResult<InvitationExpiryConfiguration>> GetAsync()
    {
        var configuration = await Repository.DbSet.OrderBy(x => x.Id).FirstOrDefaultAsync();

        return configuration == null
            ? Result.Fail<InvitationExpiryConfiguration>("INVITATION_EXPIRY_CONFIGURATION_NOT_FOUND")
            : Result.Ok(configuration);
    }
}
