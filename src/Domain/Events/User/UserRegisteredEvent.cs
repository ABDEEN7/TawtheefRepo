using System;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Events.User;

public sealed record UserRegisteredEvent(Guid UserId,string Email,string Name, Guid UserTypeId,DateTime OccurredOn) 
    : BaseEvent(OccurredOn);
