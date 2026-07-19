namespace Application.Recruitment.Features.Profile.DTOs.SaveOperation;

public sealed class SaveProfileLanguagesRequest
{
    public bool Submit { get; set; }

    public List<ProfileLanguageUpsertDto> Languages { get; set; } = [];
}

public sealed class ProfileLanguageUpsertDto
{
    public Guid LanguageId { get; set; }
    // LanguageLevel Table
    public Guid SpeakingLevelId { get; set; }
    public Guid WritingLevelId { get; set; }
    public Guid ReadingLevelId { get; set; }
}
