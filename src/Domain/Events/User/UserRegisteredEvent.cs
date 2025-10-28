using System;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Events.User;

public sealed record UserRegisteredEvent(Guid UserId,string Email,string Name, string UserType,DateTime OccurredOn) 
    : BaseEvent(OccurredOn);
