using FluentResults;
using MediatR;
using Tawtheef.Application.Features.Operations.Admin.Offices.DTOs;

namespace Tawtheef.Application.Features.Operations.Admin.Offices.Queries;

public sealed record GetOfficeCountriesQuery : IRequest<IResult<List<CountryLookupDto>>>;
