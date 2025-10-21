using System.Collections.Generic;
using AutoMapper;
using Tawtheef.Application.Common.Models.Pagination;

namespace Tawtheef.Application.Common.Mappers.Utils.Converters;

public class PaginatedListConverter<TSource, TDestination>(IMapper mapper) :
    ITypeConverter<PaginatedResult<TSource>, PaginatedResult<TDestination>>
{
    public PaginatedResult<TDestination> Convert(
        PaginatedResult<TSource> source,
        PaginatedResult<TDestination> destination,
        ResolutionContext context)
    {
        var items = mapper.Map<List<TDestination>>(source.Items);
        
        return new PaginatedResult<TDestination>(
            items,
            source.Metadata.TotalCount,
            source.Metadata.CurrentPage,
            source.Metadata.PageSize);
    }
}
