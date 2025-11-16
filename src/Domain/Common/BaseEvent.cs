using MediatR;

namespace Tawtheef.Domain.Common;

public abstract record BaseEvent(DateTime DateOccurred) : INotification;
