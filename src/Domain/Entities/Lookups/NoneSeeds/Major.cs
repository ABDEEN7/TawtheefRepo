using System.ComponentModel.DataAnnotations.Schema;
using Cortex.Mediator;
using FluentResults;

using Tawtheef.Domain.Common;
using Tawtheef.Domain.Constants;

namespace Tawtheef.Domain.Entities.Lookups.NoneSeeds;

[Table(nameof(Major), Schema = Schemas.Lookup)]
public class Major : LookupBase
{
    public Guid? ParentId { get; set; }
    public Major? Parent { get; init; }

    public ICollection<Major>? SubMajors { get; init; } = [];
    
    public void ChangeActiveStatus(bool isActive)
    {
        IsActive = isActive;
    }
    
    public IResult<Unit> UpdateDetails(string nameAr, string nameEn, bool isActive, Guid? parentId)
    {
        // Keep the hierarchy type fixed:
        // Parent stays Parent, Child stays Child.
        var isParent = ParentId is null;
        var wantsParent = parentId is null;

        if (isParent != wantsParent)
            return Result.Fail<Unit>(ErrorsCodes.ChangingHierarchyTypeNotAllowed);
        
        NameAr = nameAr;
        NameEn = nameEn;
        IsActive = isActive;
        // Allowed: either both are parent (null) or both are child (non-null).
        ParentId = parentId;

        return Result.Ok(Unit.Value);
    }
}
