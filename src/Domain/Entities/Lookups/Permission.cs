using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;

namespace Tawtheef.Domain.Entities.Lookups;

public static class PermissionIds
{
    public static readonly Guid DashboardView = Guid.Parse("9effffff-f331-4f1c-ac99-fd7847c68f31");
    public static readonly Guid RolesView = Guid.Parse("d7471c0d-1c91-4177-a41a-e566c1f60975");
    public static readonly Guid RolesManage = Guid.Parse("be448823-4c4c-49bc-ab45-929cf25ae3a5");
    public static readonly Guid UsersView = Guid.Parse("3372584b-b559-41f2-9b81-76a5a622f56d");
    public static readonly Guid UsersManage = Guid.Parse("b6b80ed9-c665-44fa-b85b-78a306382962");
    public static readonly Guid OfficesView = Guid.Parse("eb2c1b2a-6f58-4de9-b268-115b608a3ed5");
    public static readonly Guid OfficesManage = Guid.Parse("f010a0eb-3ca5-440c-b20d-3f26ef67e5b7");
    public static readonly Guid ProfileView = Guid.Parse("c61411b7-b32e-4caf-886f-cfb264e295e3");
    public static readonly Guid ProfileManage = Guid.Parse("d7303eb3-d5ae-406e-8c88-e6d498b217a0");
    public static readonly Guid ProfileDistributionView = Guid.Parse("73dd6394-e7ab-4cc7-8015-8e9f244d4ca6");
    public static readonly Guid ProfileDistributionManage = Guid.Parse("bcfbc849-a99d-4335-94ec-11270e89ebbf");
    public static readonly Guid ProfileApprovalView = Guid.Parse("e1a6683c-6daf-4127-8d79-631799571c15");
    public static readonly Guid ProfileApprovalReview = Guid.Parse("8beda317-9943-400a-bb2c-00540ada6e4d");
    public static readonly Guid ProfileApprovalChanges = Guid.Parse("8a3c60ba-4049-48d4-b4d5-541526816d2d");
    public static readonly Guid JobsView = Guid.Parse("f531b678-7bf2-4411-8bdf-6a791e9c524f");
    public static readonly Guid JobsManage = Guid.Parse("96bc20d4-d870-4b53-8c86-59886a9adc95");
    public static readonly Guid JobsApprove = Guid.Parse("3ebbcb53-3057-45b9-b0d3-085e55189f11");
    public static readonly Guid JobsPointsManage = Guid.Parse("8144efe8-57e5-4203-bcfe-fb30d569dfd5");
    public static readonly Guid JobsInvitationsView = Guid.Parse("f5ce82ff-19c5-4911-b903-0df310da5617");
    public static readonly Guid NominationsView = Guid.Parse("88043703-c57e-4a22-9952-1ec8bae23073");
    public static readonly Guid NominationsManage = Guid.Parse("d9e3064a-4016-4934-9baa-911edd91ea40");
    public static readonly Guid KawaderManage = Guid.Parse("07db86b8-6ffe-4ce0-8547-0943b80934ce");
    public static readonly Guid OfficeUsersView = Guid.Parse("cc6fca7a-d527-419b-8282-73c3bb75774e");
    public static readonly Guid OfficeUsersManage = Guid.Parse("e8c28ff5-b880-455f-89be-c9d554753936");
    public static readonly Guid MajorSkillsManage = Guid.Parse("d494f5ae-ffbc-4709-9a6e-e4eea70bd12a");
}

[Table(nameof(Permission), Schema = Schemas.Lookup)]
public class Permission : LookupBase
{
}
