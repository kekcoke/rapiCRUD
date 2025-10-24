namespace rapidCRUD.Features.Shared;

public record CreateRequest<T>
{
    public T Data { get; init; } = default!;
}

public record UpdateRequest<T>
{
    public T Data { get; init; } = default!;
}

public record PagedRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; } = false;
}
