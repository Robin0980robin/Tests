using Microsoft.EntityFrameworkCore;

namespace Api.Application.Common;

public class PaginatedList<T>(List<T> items, int count, int page, int pageSize)
{
    public List<T> Items { get; } = items;
    public int Page { get; } = page;
    public int PageSize { get; } = pageSize;
    public int TotalCount { get; } = count;
    public int TotalPages { get; } = (int)Math.Ceiling(count / (double)pageSize);

    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync<TSource>(
        IQueryable<TSource> source, 
        int page, 
        int pageSize, 
        Func<TSource, T> mapper,
        CancellationToken cancellationToken)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
            
        var mappedItems = items.Select(mapper).ToList();
        return new PaginatedList<T>(mappedItems, count, page, pageSize);
    }
}
