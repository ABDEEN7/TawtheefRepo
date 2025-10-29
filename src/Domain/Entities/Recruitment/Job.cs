using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CSharpFunctionalExtensions;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Entities.Applicant;
using Tawtheef.Domain.Entities.Lookups;
using Tawtheef.Domain.Entities.Users;

namespace Tawtheef.Domain.Entities.Recruitment;

[Table(nameof(Job), Schema = Schemas.Hr)]
public class Job : Entity
{
    [Required, MaxLength(250)] 
    public required string Title { get; set; }
    public required string Description { get; set; }
    
    public Guid? CountryId { get; set; }
    public Country? Country { get; set; }
    
    public Guid? CityId { get; set; }
    public City? City { get; set; }

    public Guid StatusId { get; set; }
    public JobStatus? Status { get; set; }

    /// <summary>
    /// list of responsibilities separated by unique line '\n'
    /// </summary>
    public required string Responsibilities { get; set; }
    [NotMapped]
    public IReadOnlyList<string> ResponsibilitiesList => Responsibilities.Split('\n').ToList();

    public DateTimeOffset? PublishAt { get; set; }
    public DateTimeOffset? ApplicationDeadline { get; set; }
    
    public Guid SectorId { get; set; }
    public Sector? Sector { get; set; }
    
    public Guid WorkTypeId { get; set; }
    public WorkType? WorkType { get; set; }
    
    /// <summary>
    /// list of qualifications separated by unique line '\n'
    /// </summary>
    public required string Qualification { get; set; }
    [NotMapped]
    public IReadOnlyList<string> QualificationList => Qualification.Split('\n').ToList();
    
    /// <summary>
    /// list of benefits separated by unique line '\n'
    /// </summary>
    public required string Benefits { get; set; }
    [NotMapped]
    public IReadOnlyList<string> BenefitsList => Benefits.Split('\n').ToList();

    public required string OrganizationDescription { get; set; }

    public ICollection<Invitations> Invitations { get; init; } = [];

    /// <summary>
    /// Check if the job matches the profile.
    /// </summary>
    /// <param name="profile"></param>
    /// <returns></returns>
    public bool Matches(UserProfile profile)
    {
        return true;
    }
    
    
}
