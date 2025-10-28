using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Domain.Entities.Applicant;

public class Experience : TrainingCourse
{
    /// <summary>
    /// list of achievements split by unique line '\n'
    /// </summary>
    public required string Achievements { get; set; }
    
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
