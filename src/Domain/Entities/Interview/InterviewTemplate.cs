using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Domain.Entities.Interview;

[Table(nameof(InterviewTemplate), Schema = Schemas.Interview)]
public class InterviewTemplate : EventEntity
{
    public required string TitleAr { get; set; }
    public string? TitleEn { get; set; }
    public Guid? OrganizationScopeId { get; set; }
    public InterviewOrganizationScope? OrganizationScope { get; set; }
    public Guid? JobTitleId { get; set; }
    public JobTitle? JobTitle { get; set; }
    public Guid? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public bool IsActive { get; set; } = true;
}
