using System.ComponentModel.DataAnnotations;

namespace Tawtheef.Application.Common.Models.Pagination;
public record PaginatedRequest
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;
    public int PageNumber { get; set; } = 1;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
    public string? SortBy { get; set; }
    [AllowedValues("asc", "desc", ErrorMessage = "Sort direction must be either 'asc' or 'desc'.")]
    public string? SortDirection { get; set; } = "asc";
    // ✅ Add these computed properties
    public int Skip => (PageNumber - 1) * PageSize;
    public int Take => PageSize;
}
