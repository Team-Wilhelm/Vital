using Microsoft.EntityFrameworkCore;

namespace Models.Pagination;

public class PaginatedList<T>
{
    public List<T> Items { get; private set; }
    public int PageIndex { get; }
    public int TotalPages { get; }
    public int PageSize { get; }
    public int Count { get; }

    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        Count = count;
        PageSize = pageSize;
        Items = items;
    }

    public bool HasPreviousPage => PageIndex > 1;

    public bool HasNextPage => PageIndex < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(
        IEnumerable<T> items, int pageIndex, int pageSize, int count)
    {
        return new PaginatedList<T>(items.ToList(), count, pageIndex, pageSize);
    }

    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> source, int pageIndex, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip(
                (pageIndex - 1) * pageSize)
            .Take(pageSize).ToListAsync();
        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }
}
