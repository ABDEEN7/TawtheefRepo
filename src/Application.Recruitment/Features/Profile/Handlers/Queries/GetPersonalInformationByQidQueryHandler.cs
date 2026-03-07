using Application.Recruitment.Common.Interfaces.Services.HttpClients;
using Application.Recruitment.Features.Profile.Queries;
using MediatR;
using FluentResults;
using Tawtheef.Application.Features.Authenticator.DTOs;

namespace Application.Recruitment.Features.Profile.Handlers.Queries;

public class GetPersonalInformationByQidQueryHandler(IMoiClient client) :
    IRequestHandler<GetPersonalInformationByQidQuery, IResult<MOEPersonalInfo>>
{
    public async Task<IResult<MOEPersonalInfo>> Handle(GetPersonalInformationByQidQuery query,
        CancellationToken cancellationToken)
    { 
        var result = await client.GetPersonalInfoAsync(query.Request.QID, query.Request.ExpiryDate, cancellationToken);
        if (result.IsFailed)
            return Result.Fail<MOEPersonalInfo>(result.Errors);
        
        return Result.Ok(result.Value);
    }
}

