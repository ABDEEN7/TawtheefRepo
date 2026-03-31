using FluentResults;
using MediatR;
using Tawtheef.Domain.Entities.Recruitment;

namespace Application.Operation.Features.Employee.JobManagement.JobInvitationSummaryDetails.Queries;

public record GetInvitationAttachmentsQuery(Guid InvitationId) : IRequest<Result<List<InvitationAttachmentDto>>>;

public class InvitationAttachmentDto
{
    public Guid Id { get; set; }
    public Guid JobRequiredAttachmentId { get; set; }
    public string TitleEn { get; set; } = null!;
    public string TitleAr { get; set; } = null!;
    public bool IsMandatory { get; set; }
    public Guid? ResourceId { get; set; }
    public string? ResourceUrl { get; set; }
    public string? ResourceName { get; set; }
    public bool IsApproved { get; set; }
    public bool IsReturned { get; set; }
    public string? ReviewNote { get; set; }
}
