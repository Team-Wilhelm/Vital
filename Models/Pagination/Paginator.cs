namespace Models.Pagination;

public class Paginator {
    public int Page { get; set; } = 1;
    public int ItemsPerPage { get; set; } = 50;
    public SortOrder SortOrder { get; set; }
    public string? OrderBy { get; set; }
    public string? Query { get; set; }
}

public enum SortOrder {
    Desc = 1,
    Asc = 0
}
