using Mapster;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Skills.DTOs;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Common.Mappers.EmployeeProfiles;

public class MajorSkillManagementProfile : IRegister
{
     public void Register(TypeAdapterConfig config)
     {
         TypeAdapterConfig<Major, Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Majors.DTOs.MajorDetailsDto>.NewConfig()
             .Map(d => d, s => s.Adapt<DropdownOptions>())
             .Map(d => d.IsActive, s => s.IsActive)
             .Map(d => d.Parent, s => s.Parent == null ? null : s.Parent.Adapt<DropdownOptions>());
         TypeAdapterConfig<Major, Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Mapping.DTOs.MajorDetailsDto>.NewConfig()
             .Map(d => d, s => s.Adapt<DropdownOptions>())
             .Map(d => d.IsActive, s => s.IsActive)
             .Map(d => d.Parent, s => s.Parent == null ? null : s.Parent.Adapt<DropdownOptions>());
         TypeAdapterConfig<Skill, SkillDetailsDto>.NewConfig()
             .Map(d => d, s => s.Adapt<DropdownOptions>())
             .Map(d => d.IsActive, s => s.IsActive)
             .Map(d => d.SkillType, s => s.SkillType == null ? null : s.SkillType.Adapt<DropdownOptions>());
     }
}
