using Cortex.Mediator.Queries;
using FluentResults;

using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Application.Features.Recruitment.Profile.DTOs;
using Tawtheef.Application.Features.Recruitment.Profile.Queries;

namespace Tawtheef.Application.Features.Recruitment.Profile.Handlers.Queries;

public class GetPersonalInformationByQidQueryHandler(IMoiClient client) :
    IQueryHandler<GetPersonalInformationByQidQuery, IResult<MOEPersonalInfo>>
{
    public async Task<IResult<MOEPersonalInfo>> Handle(GetPersonalInformationByQidQuery query,
        CancellationToken cancellationToken)
    { 
        var result = await client.GetPersonalInfoAsync(query.Request.QID, query.Request.ExpiryDate, cancellationToken);
        if(result.IsFailed)
            return Result.Fail<MOEPersonalInfo>(result.Errors);
        
        return Result.Ok(result.Value);
    }
}
