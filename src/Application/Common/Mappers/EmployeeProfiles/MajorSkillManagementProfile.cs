using Mapster;
using MapsterMapper;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Majors.DTOs;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Skills.DTOs;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Common.Mappers.EmployeeProfiles;

public class MajorSkillManagementProfile : IRegister
{
     public void Register(TypeAdapterConfig config)
     {
         TypeAdapterConfig<Major, MajorDetailsDto>.NewConfig()
             .Map(d => d, s => s.Adapt<DropdownOptions>())
             .Map(d => d.IsActive, s => s.IsActive)
             .Map(d => d.Parent, s => s.Parent == null ? null : s.Parent.Adapt<DropdownOptions>());
         TypeAdapterConfig<Skill, SkillDetailsDto>.NewConfig()
             .Map(d => d, s => s.Adapt<DropdownOptions>())
             .Map(d => d.IsActive, s => s.IsActive)
             .Map(d => d.SkillType, s => s.SkillType == null ? null : s.SkillType.Adapt<DropdownOptions>());
     }
}
