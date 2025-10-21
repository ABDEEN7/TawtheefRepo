using System;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class NotificationTypeIds
{
    public static readonly Guid CoursePublished = Guid.Parse("a3b8c1d2-e6f4-4872-9a3b-5c7d9e1f2a4b");
    public static readonly Guid CourseUpdated = Guid.Parse("b4c9d2e3-f7a5-5983-0b4c-6d8e0f1a2b3c");
    public static readonly Guid EnrollmentApproved = Guid.Parse("c5d0e3f4-a8b6-6a94-1c5d-7e9f0a1b2c3d");
    public static readonly Guid PaymentReceived = Guid.Parse("d6e1f4a5-b9c7-7b05-2d6e-8f0a1b2c3d4e");
    public static readonly Guid ContentUpdate = Guid.Parse("e7f2a5b6-c0d8-8c16-3e7f-9a0b1c2d3e4f");
    public static readonly Guid MeetingReminder = Guid.Parse("f8a3b6c7-d1e9-9d27-4f8a-0b1c2d3e4f5a");
}
public class NotificationType : LookupBase 
{
}
