using Mapster;
using Tawtheef.Application.Common.Models;
using Tawtheef.Application.Features.Operations.Employee.OrganizationStructures.DTOs;
using Tawtheef.Domain.Entities.Lookups;

namespace Tawtheef.Application.Common.Mappers.EmployeeProfiles;

public class OrganizationStructuresProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        TypeAdapterConfig<Sector, SectorDto>.NewConfig()
            .Map(d => d, s => s.Adapt<DropdownOptions>())
            .Map(d => d.IsActive, s => s.IsActive)
            .Map(d => d.DisplayOrder, s => s.DisplayOrder);

        TypeAdapterConfig<Management, ManagementDto>.NewConfig()
            .Map(d => d, s => s.Adapt<DropdownOptions>())
            .Map(d => d.IsActive, s => s.IsActive)
            .Map(d => d.DisplayOrder, s => s.DisplayOrder)
            .Map(d => d.SectorId, s => s.SectorId)
            .Map(d => d.Sector, s => s.Sector == null ? null : s.Sector.Adapt<DropdownOptions>());

        TypeAdapterConfig<Department, DepartmentDto>.NewConfig()
            .Map(d => d, s => s.Adapt<DropdownOptions>())
            .Map(d => d.IsActive, s => s.IsActive)
            .Map(d => d.DisplayOrder, s => s.DisplayOrder)
            .Map(d => d.ManagementId, s => s.ManagementId)
            .Map(d => d.Management, s => s.Management == null ? null : s.Management.Adapt<DropdownOptions>())
            .Map(d => d.Sector,
                s => s.Management?.Sector == null ? null : s.Management.Sector.Adapt<DropdownOptions>());
    }
}
