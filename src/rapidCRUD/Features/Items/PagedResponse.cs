namespace rapidCRUD.Features.Items;

public record PagedResponse<T>
{
    public IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }

    public PagedResponse() { }

    public PagedResponse(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items ?? Enumerable.Empty<T>();
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}