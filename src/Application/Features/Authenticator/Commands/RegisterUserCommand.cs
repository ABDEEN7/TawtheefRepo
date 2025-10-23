using System.ComponentModel.DataAnnotations;
using CSharpFunctionalExtensions;
using MediatR;
using Tawtheef.Application.Features.Authenticator.DTOs.Responses;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Features.Authenticator.Commands;

public record RegisterUserCommand : IRequest<Result<RegistrationResponse>>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string Name { get; init; }
    [AllowedValues(nameof(GenderIds.Male), nameof(GenderIds.Female), ErrorMessage = "Invalid gender. Valid values are Male or Female.")]
    public required string Gender { get; init; }
    [AllowedValues(nameof(UserTypeIds.Applicant), nameof(UserTypeIds.Employee), ErrorMessage = "Invalid user type. Valid values are Student or Instructor.")]
    public required string UserType { get; init; }
    public string? Qualifications { get; init; }
    public string? Specialization { get; init; }
}
