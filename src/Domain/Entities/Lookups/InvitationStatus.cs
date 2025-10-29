using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;
public static class InvitationStatusIds
{
    public static readonly Guid Submitted               = Guid.Parse("3F0B7DAB-7C9F-4B3F-8A5F-1F8D2B7F2C11");
    public static readonly Guid Returned                = Guid.Parse("A1B923D2-2C7E-4E1A-9B14-0B6D2F5C7E90");
    public static readonly Guid UnderReview             = Guid.Parse("5A7D2C4F-3B1E-44A6-A9E5-9E7D23B4C1A2");
    public static readonly Guid AdminShortlisting       = Guid.Parse("D3E2A4B6-8F1C-4E7A-9D2B-7C5F1A9E3B40");
    public static readonly Guid TechnicalShortlisting   = Guid.Parse("9C2A7D5B-6E4F-4D1A-83B2-5E7F9A1C2D30");
    public static readonly Guid TestProcessing          = Guid.Parse("2E4B6A8C-1D2F-4B6A-9C3E-7A1B2C3D4E55");
    public static readonly Guid InterviewProcessing     = Guid.Parse("7B9E1C3D-5A2F-4F6B-8A1D-2C3E4F5A6B70");
    public static readonly Guid HiringProcessing        = Guid.Parse("6A5B4C3D-2E1F-4A6B-9C8D-1E2F3A4B5C60");
    public static readonly Guid FinalApprovalProcessing = Guid.Parse("1C2D3E4F-5A6B-7C8D-9E1F-2A3B4C5D6E20");
}
[Table(nameof(InvitationStatus), Schema = Schemas.Lookup)]
public class InvitationStatus : LookupBase
{
}
