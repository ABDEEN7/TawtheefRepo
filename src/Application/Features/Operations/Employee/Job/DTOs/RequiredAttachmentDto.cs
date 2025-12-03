using System.ComponentModel.DataAnnotations;

namespace Tawtheef.Application.Features.Operations.Employee.Job.Dtos;

public class RequiredAttachmentDto
{
    public required string Title { get; set; }
    public bool IsMandatory { get; set; }
}
