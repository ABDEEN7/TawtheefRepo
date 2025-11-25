namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public sealed class SaveProfileSkillsRequest
{
    public bool Submit { get; set; }

    public List<SkillUpsertDto> Skills { get; set; } = [];
    public List<ProfileLanguageUpsertDto> Languages { get; set; } = [];
}

public sealed class SkillUpsertDto
{
    public Guid SkillId { get; set; }    // Id من جدول SkillType
}

public sealed class ProfileLanguageUpsertDto
{
    public Guid LanguageId { get; set; } // Id من جدول Language
    public Guid LevelId { get; set; }    // Id من جدول LanguageLevel
}

