using System;
using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class NotificationTypeIds
{
    public static readonly Guid SubmitApplication = Guid.Parse("c5d0e3f4-a8b6-6a94-1c5d-7e9f0a1b2c3d");
}
[Table(nameof(NotificationType), Schema = Schemas.Lookup)]
public class NotificationType : LookupBase 
{
}
