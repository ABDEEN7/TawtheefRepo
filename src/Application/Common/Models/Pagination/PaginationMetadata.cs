namespace Tawtheef.Application.Common.Models.Pagination;


public class PaginationMetadata(int totalCount, int pageSize, int currentPage)
{
    public int TotalCount { get; } = totalCount;
    public int PageSize { get; } = pageSize;
    public int CurrentPage { get; } = currentPage;
    public int TotalPages { get; } = (int)Math.Ceiling(totalCount / (double)pageSize);
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
}
