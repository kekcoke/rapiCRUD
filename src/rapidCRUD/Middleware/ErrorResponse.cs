namespace rapidCRUD.Middleware;

public record ErrorResponse
{
    public string TraceId { get; init; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
}