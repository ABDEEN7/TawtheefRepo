using System.ComponentModel.DataAnnotations;

namespace Application.Operation.Features.Employee.Job.DTOs;

public class GetJobsQueryDto
{
    public Guid? SectorId { get; set; }
    public Guid? ManagementId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? JobCategoryId { get; set; }
    public Guid? StatusId { get; set; }
    public Guid? MajorId { get; set; }

    [MaxLength(200)]
    public string? Title { get; set; }

    public bool? IsActive { get; set; }
    public bool? IsPublished { get; set; }

    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    public string? SortBy { get; set; }
    public bool IsDescending { get; set; } = false;
}
