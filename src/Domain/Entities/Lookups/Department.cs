using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class DepartmentIds
{
    public static readonly Guid InformationSystems = Guid.Parse("48d2a180-30dc-4314-b222-92750a9d0afe");
    public static readonly Guid HumanResources = Guid.Parse("7e225d86-c5e5-4225-b9fc-d255a975f5d1");
    public static readonly Guid AdministrativeFinancialAffairs = Guid.Parse("8fd80cb6-794a-4211-b8a4-139e8e026e5b");
    public static readonly Guid Evaluation = Guid.Parse("6881d9fd-7281-457a-a2e1-3cbe827622e8");
    public static readonly Guid Curriculum = Guid.Parse("b858ce40-a3ac-4d27-aba9-86ef62fab4fe");
    public static readonly Guid EarlyChildhoodEducation = Guid.Parse("0bfb044d-9f85-4f46-a1eb-920a1f9f519a");
    public static readonly Guid HigherEducation  = Guid.Parse("1b01eb93-b6e3-447a-8d1c-a9ce59bf5ca7");
    public static readonly Guid PrimaryEducation= Guid.Parse("2a65e4a6-ca1b-4da3-8375-299ec39a50f9");
    public static readonly Guid SchoolAffairs = Guid.Parse("91a511fb-9b43-47fd-9cd5-fc2a9ecbc32e");
    public static readonly Guid CommunicationsMedia = Guid.Parse("8acd1c68-9b65-40e1-8776-ce6d7afcf542");
}
/// <summary>
/// هي الجهة الطالبة للوظيفة
/// </summary>
[Table(nameof(Department), Schema = Schemas.Lookup)]
public class Department : LookupBase
{
    public Guid ManagementId { get; set; }
    public Management? Management { get; set; }
}
