using System.ComponentModel.DataAnnotations.Schema;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Domain.Entities.Lookups.NoneSeeds;

public static class MajorIds
{
    public static readonly Guid Other = Guid.Parse("7044ac66-706b-4384-bb79-75c71801da8b");
    public static readonly Guid SubOther = Guid.Parse("60705656-fe6c-4f16-8bef-c61dfeca3cb2");
}
[Table(nameof(Major), Schema = Schemas.Lookup)]
[Index(nameof(ParentId))]
public class Major : LookupBase
{
    public Guid? ParentId { get; set; }
    public Major? Parent { get; init; }

    public ICollection<Major>? SubMajors { get; init; } = [];
    
    public void ChangeActiveStatus(bool isActive)
    {
        IsActive = isActive;
    }
    
    public IResult<Unit> UpdateDetails(string nameAr, string nameEn, 
        string? descriptionAr, string? descriptionEn,
        bool isActive, Guid? parentId)
    {
        // Keep the hierarchy type fixed:
        // Parent stays Parent, Child stays Child.
        var isParent = ParentId is null;
        var wantsParent = parentId is null;

        if (isParent != wantsParent)
            return Result.Fail<Unit>(ErrorsCodes.ChangingHierarchyTypeNotAllowed);
        
        NameAr = nameAr;
        NameEn = nameEn;
        DescriptionAr = descriptionAr;
        DescriptionEn = descriptionEn;
        IsActive = isActive;
        // Allowed: either both are parent (null) or both are child (non-null).
        ParentId = parentId;

        return Result.Ok(Unit.Value);
    }
}

