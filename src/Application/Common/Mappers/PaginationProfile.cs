using AutoMapper;
using Tawtheef.Application.Common.Mappers.Utils.Converters;
using Tawtheef.Application.Common.Models.Pagination;

namespace Tawtheef.Application.Common.Mappers;

public class PaginationProfile : Profile
{
    public PaginationProfile()
    {
        CreateMap(typeof(PaginatedResult<>), typeof(PaginatedResult<>))
            .ConvertUsing(typeof(PaginatedListConverter<,>));
    }
}