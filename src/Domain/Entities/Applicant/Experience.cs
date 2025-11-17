using System.ComponentModel.DataAnnotations.Schema;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Applicant;

[Table(nameof(Experience), Schema = Schemas.Profile)]
public class Experience : EventEntity
{
    public required string Organization { get; set; }
    public required string Position { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    
    public Guid CertificateId { get; set; }
    public Resource? Certificate { get; set; }
    
    public Guid UserProfileId { get; set; }
    public UserProfile? UserProfile { get; set; }
    /// <summary>
    /// list of achievements  separated by unique line '\n'
    /// </summary>
    public required string Achievements { get; set; }
    [NotMapped]
    public IReadOnlyList<string> AchievementsList => Achievements.Split('\n').ToList();
    
    public void AddAchievement(string achievement)
    {
        if (string.IsNullOrWhiteSpace(Achievements))
        {
            Achievements = achievement;
        }
        else
        {
            Achievements += "\n" + achievement;
        }
    }
    public void RemoveAchievement(string achievement)
    {
        Achievements = Achievements.Replace(achievement, string.Empty);
    }
    public void ClearAchievements()
    {
        Achievements = string.Empty;
    }
    public List<string> GetAchievements()
    {
        return Achievements.Split("\n").ToList();
    }
}
