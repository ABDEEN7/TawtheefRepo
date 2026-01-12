namespace Application.Recruitment.Features.Profile.DTOs.SaveOperation;

public sealed class SaveProfileLanguagesRequest
{
    public bool Submit { get; set; }

    public List<ProfileLanguageUpsertDto> Languages { get; set; } = [];
}

public sealed class ProfileLanguageUpsertDto
{
    public Guid LanguageId { get; set; } // Id من جدول Language
    public Guid SpeakingLevelId { get; set; }    // Id من جدول LanguageLevel
    public Guid WritingLevelId { get; set; }     // Id من جدول LanguageLevel
    public Guid ReadingLevelId { get; set; }     // Id من جدول LanguageLevel
}
