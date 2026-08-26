using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tawtheef.Application.Common.Interfaces.Repositories.Base;
using Tawtheef.Application.Common.Interfaces.Services;
using Tawtheef.Domain.Entities.Users;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Handlers.Queries;

public class SearchAllCandidatesQueryHandler(
    IUnitOfWork unitOfWork,
    ILocalizationService localizationService)
    : IRequestHandler<SearchAllCandidatesQuery, IResult<List<CandidateSearchDto>>>
{
    public async Task<IResult<List<CandidateSearchDto>>> Handle(SearchAllCandidatesQuery request, CancellationToken cancellationToken)
    {
        var query = unitOfWork.GetEntityRepository<UserProfile>().DbSet
            .AsNoTracking()
            .Include(p => p.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim().ToLower();
            query = query.Where(p => 
                (p.NationalNumber != null && p.NationalNumber.Contains(term)) ||
                (p.User != null && p.User.Email != null && p.User.Email.ToLower().Contains(term)) ||
                (p.User != null && p.User.FullNameAr != null && p.User.FullNameAr.ToLower().Contains(term)) ||
                (p.User != null && p.User.FullNameEn != null && p.User.FullNameEn.ToLower().Contains(term))
            );
        }

        var isArabic = string.Equals(
            localizationService.GetCurrentLanguage(),
            "ar",
            StringComparison.OrdinalIgnoreCase);

        var results = await query
            .OrderBy(p => isArabic ? p.User!.FullNameAr : p.User!.FullNameEn)
            .Take(50)
            .Select(p => new CandidateSearchDto
            {
                CandidateId = p.UserId,
                FullName = isArabic ? p.User!.FullNameAr : p.User!.FullNameEn,
                NationalId = p.NationalNumber ?? string.Empty,
                Email = p.User.Email ?? string.Empty
            })
            .ToListAsync(cancellationToken);

        return Result.Ok(results);
    }
}
