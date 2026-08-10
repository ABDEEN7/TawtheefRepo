using System.ComponentModel.DataAnnotations;

namespace Tawtheef.Application.Common.Models.Pagination;
public record PaginatedRequest
{
    private int _pageSize = 10;
    protected virtual int MaximumPageSize => 50;
    public int PageNumber { get; init; } = 1;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaximumPageSize) ? MaximumPageSize : value;
    }
    public string? SortBy { get; init; }
    [AllowedValues("asc", "desc", ErrorMessage = "Sort direction must be either 'asc' or 'desc'.")]
    public string? SortDirection { get; init; } = "asc";

    public string Language { get; init; } = "ar";
}
