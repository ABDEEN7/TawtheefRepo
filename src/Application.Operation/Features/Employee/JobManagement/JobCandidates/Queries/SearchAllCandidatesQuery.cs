using System.Collections.Generic;
using Application.Operation.Features.Employee.JobManagement.JobCandidates.Models;
using FluentResults;
using MediatR;

namespace Application.Operation.Features.Employee.JobManagement.JobCandidates.Queries;

public class SearchAllCandidatesQuery : IRequest<IResult<List<CandidateSearchDto>>>
{
    public string? SearchTerm { get; set; }
}
