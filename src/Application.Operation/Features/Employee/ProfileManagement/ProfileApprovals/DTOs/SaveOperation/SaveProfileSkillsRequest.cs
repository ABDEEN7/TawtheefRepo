namespace Application.Operation.Features.Employee.ProfileManagement.ProfileApprovals.DTOs.SaveOperation;

public sealed class SaveProfileSkillsRequest
{
    public bool Submit { get; set; }

    public List<SkillUpsertDto> Skills { get; set; } = [];
}
public sealed class SkillUpsertDto
{
    public Guid SkillId { get; set; }    // Id من جدول SkillType
    public Guid LevelId { get; set; }    // Id من جدول RatingGrade (Skill level)
}
