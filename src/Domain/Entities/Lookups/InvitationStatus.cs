using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;
public static class InvitationStatusIds
{
    //by sys
    public static readonly Guid NewInvitation = Guid.Parse("F0BC801D-F54C-4A0E-8AAE-00694E4FC80D");
    public static readonly Guid Closed = Guid.Parse("CA054592-8617-406D-8E8F-3A773B3D0D5E");
    public static readonly Guid Expired = Guid.Parse("c0a9d348-2175-4fa2-a1fb-fa6a6a729e64");
    //by employee
    public static readonly Guid Cancelled = Guid.Parse("BCED01D8-3784-4E2C-9312-930951CC59D8");
    public static readonly Guid ReturnedAttachment = Guid.Parse("9B86FA2C-D295-46D7-9092-23AD8AB7B10C");
    //by user
    public static readonly Guid ExamEligible = Guid.Parse("22EF7E86-28CB-4A30-98BC-7D45F9B44DE3");
    public static readonly Guid PendingAttachmentApproval = Guid.Parse("6608F560-4DC0-4F2A-A190-6743A9A8C5CB");
    public static readonly Guid Read = Guid.Parse("64236C6A-167A-4213-B1D6-80C2C8C86DDE");
    public static readonly Guid Rejected = Guid.Parse("7103BA49-AD43-4751-B1A5-9084ACA69676");
    // by system - set once a candidate passes the exam stage; consumed by Interview scheduling as the eligible-candidate pool.
    public static readonly Guid InterviewEligible = Guid.Parse("3F6B6E1E-9C2B-4C7E-8B3E-2B6A6E7B9B10");
    // by system - set when an Interview Result Report is approved with this candidate's FinalDecision.
    public static readonly Guid CandidateForHiringProcess = Guid.Parse("5A2C3E4F-7B8D-4F1A-9C6E-1D3B5A7C9E2F");
    public static readonly Guid WaitingList = Guid.Parse("8D4E6F1A-2B3C-4D5E-8F9A-1B2C3D4E5F6A");
}
[Table(nameof(InvitationStatus), Schema = Schemas.Lookup)]
public class InvitationStatus : LookupBase
{
}
