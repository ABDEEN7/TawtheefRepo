using MediatR;

namespace Tawtheef.Domain.Common;

public abstract record BaseEvent(DateTimeOffset DateOccurred) : INotification;
