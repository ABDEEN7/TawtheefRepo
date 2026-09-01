using Application.Recruitment.Features.Profile.Queries;
using MediatR;
using FluentResults;
using Tawtheef.Application.Common.Interfaces.Services.HttpClients;
using Tawtheef.Application.Features.Authenticator.DTOs;

namespace Application.Recruitment.Features.Profile.Handlers.Queries;

public class GetPersonalInformationByQidQueryHandler(IMoiClient client) :
    IRequestHandler<GetPersonalInformationByQidQuery, IResult<MOEPersonalInfo>>
{
    public async Task<IResult<MOEPersonalInfo>> Handle(GetPersonalInformationByQidQuery query,
        CancellationToken cancellationToken)
    { 
        var result = await client.GetPersonalInfoAsync(query.Request.QID, query.Request.ExpiryDate, cancellationToken);
        return result.IsFailed ? Result.Fail<MOEPersonalInfo>(result.Errors) : Result.Ok(result.Value);
    }
}

