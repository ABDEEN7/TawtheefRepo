using Application.Operation.Common.Repositories;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Commands;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.DTOs;
using FluentResults;
using MapsterMapper;
using MediatR;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Recruitment.JobDetails;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Handlers.Commands;

public sealed class SaveInvitationExpiryConfigurationCommandHandler(
    IInvitationExpiryConfigurationRepository repository,
    IUnitOfWork uow,
    IMapper mapper)
    : IRequestHandler<SaveInvitationExpiryConfigurationCommand, IResult<InvitationExpiryConfigurationResponseDto>>
{
    public async Task<IResult<InvitationExpiryConfigurationResponseDto>> Handle(
        SaveInvitationExpiryConfigurationCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Request.ExpiryDays < 0)
            return Result.Fail<InvitationExpiryConfigurationResponseDto>(ErrorsCodes.InvitationExpiryDaysInvalid);

        InvitationExpiryConfiguration configuration;
        var existing = await repository.GetAsync();

        if (existing.IsFailed)
        {
            configuration = new InvitationExpiryConfiguration
            {
                Id = InvitationExpiryConfigurationIds.Default,
                ExpiryDays = request.Request.ExpiryDays
            };

            await repository.Repository.AddAsync(configuration, cancellationToken);
        }
        else
        {
            configuration = existing.Value;
            configuration.ExpiryDays = request.Request.ExpiryDays;
        }

        await uow.SaveChangesAsync(cancellationToken);

        return Result.Ok(mapper.Map<InvitationExpiryConfigurationResponseDto>(configuration));
    }
}
