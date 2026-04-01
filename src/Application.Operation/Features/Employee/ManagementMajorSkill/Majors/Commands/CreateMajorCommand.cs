using MediatR;
using FluentResults;

namespace Application.Operation.Features.Employee.ManagementMajorSkill.Majors.Commands;

public record CreateMajorCommand(string NameAr, string NameEn, 
    string DescriptionAr, string DescriptionEn,
    bool IsActive, Guid? ParentMajorId): IRequest<IResult<Unit>>;

