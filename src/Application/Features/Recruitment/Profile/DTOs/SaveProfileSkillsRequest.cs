namespace Tawtheef.Application.Features.Recruitment.Profile.DTOs;

public sealed class SaveProfileSkillsRequest
{
    public bool Submit { get; set; }

    public List<SkillUpsertDto> Skills { get; set; } = [];
}
public sealed class SaveProfileLanguagesRequest
{
    public bool Submit { get; set; }

    public List<ProfileLanguageUpsertDto> Languages { get; set; } = [];
}

public sealed class SkillUpsertDto
{
    public Guid SkillId { get; set; }    // Id من جدول SkillType
    public Guid LevelId { get; set; }    // Id من جدول RatingGrade (Skill level)
}

public sealed class ProfileLanguageUpsertDto
{
    public Guid LanguageId { get; set; } // Id من جدول Language
    public Guid SpeakingLevelId { get; set; }    // Id من جدول LanguageLevel
    public Guid WritingLevelId { get; set; }     // Id من جدول LanguageLevel
    public Guid ReadingLevelId { get; set; }     // Id من جدول LanguageLevel
}

