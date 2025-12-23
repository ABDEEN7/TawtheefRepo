using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Application.Features.Authenticator.DTOs.Responses;

public sealed record ProfileBootstrapSource(UserProfile Profile, User User, ProfilePrefillDto Prefill);
