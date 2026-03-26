using Application.Operation.Features.Employee.ManagementMajorSkill.Majors.DTOs;
using Application.Operation.Features.Employee.ManagementMajorSkill.Skills.DTOs;
using Mapster;
using Tawtheef.Application.Common.Models;
using Tawtheef.Domain.Entities.Lookups.NoneSeeds;

namespace Application.Operation.Common.Mappers;

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
             .Map(d => d.IsGeneral, s => s.IsGeneral)
             .Map(d => d.SkillType, s => s.SkillType == null ? null : s.SkillType.Adapt<DropdownOptions>());
     }
}
