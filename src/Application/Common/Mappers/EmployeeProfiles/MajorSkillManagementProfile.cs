using Mapster;
using MapsterMapper;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Operations.Employee.ManagementMajorSkill.Skills.DTOs;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Tawtheef.Application.Common.Mappers.EmployeeProfiles;

public class MajorSkillManagementProfile : IRegister
{
     public void Register(TypeAdapterConfig config)
     {
         // Skill -> SkillDetailsDto
         TypeAdapterConfig<Skill, SkillDetailsDto>.NewConfig()
             .Map(d => d, s => s.Adapt<DropdownOptions>())
             .Map(d => d.IsActive, s => s.IsActive)
             .Map(d => d.SkillType, s => s.SkillType == null ? null : s.SkillType.Adapt<DropdownOptions>());
     }
}
