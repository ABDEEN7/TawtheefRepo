using Application.Operation.Features.Employee.Exceptions.Queries;
using FluentValidation;
using Tawtheef.Domain.Constants;

namespace Application.Operation.Features.Employee.Exceptions.Validators;

public sealed class GetExceptionJobsQueryValidator : AbstractValidator<GetExceptionJobsQuery>
{
    public GetExceptionJobsQueryValidator()
    {
        RuleFor(query => query.ManagementId)
            .NotEmpty()
            .WithErrorCode(JobMessages.ManagementRequired)
            .WithMessage(JobMessages.ManagementRequired);
    }
}
