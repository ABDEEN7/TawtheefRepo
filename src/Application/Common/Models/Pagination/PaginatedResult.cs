namespace Tawtheef.Application.Common.Models.Pagination;


public class PaginatedResult<T>(List<T> items, int totalCount, int pageNumber, int pageSize)
{
    public List<T> Items { get; } = items;
    public PaginationMetadata Metadata { get; } = new(totalCount, pageSize, pageNumber);
    
    public object? AdditionalData { get; set; }
    
    public static PaginatedResult<T> Empty => new(new List<T>(), 0, 1, 10);
}
