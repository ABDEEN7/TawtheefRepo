using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tawtheef.Application.Extensions;
using Tawtheef.Application.Features.Authenticator.Commands;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Constants;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.Handlers.Commands;

public class RegisterUserCommandHandler(UserManager<User> userManager, IMediator mediator)
    : IRequestHandler<RegisterUserCommand, Result<RegistrationResponse>>
{
    public async Task<Result<RegistrationResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not null)
            return Result.Failure<RegistrationResponse>(ErrorsCodes.EmailIsAlreadyTaken);

        var userResult = User.Register(
            email: request.Email,
            displayName: request.Name,
            genderRaw: request.Gender,
            userTypeRaw: request.UserType,
            userTypeNameResolver: id => id == UserTypeIds.Applicant ? nameof(UserTypeIds.Applicant) : nameof(UserTypeIds.Employee)
        );
        if (userResult.IsFailure)
            return Result.Failure<RegistrationResponse>(userResult.Error);

        var user = userResult.Value;
        var create = await userManager.CreateAsync(user, request.Password);

        if (!create.Succeeded)
        {
            var errors = string.Join(", ", create.Errors.Select(e => e.Description));
            return Result.Failure<RegistrationResponse>(errors);
        }

        await user.PublishAndClearAsync(mediator, cancellationToken);
        
        return Result.Success(new RegistrationResponse(
            user.Id,
            user.Email!,
            request.UserType,
            RequiresAdminApproval: true
        ));
    }
}
