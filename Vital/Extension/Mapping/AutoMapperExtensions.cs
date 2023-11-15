using AutoMapper;
using Models.Pagination;

namespace Vital.Extension.Mapping;

public static class AutoMapperExtensions
{
    public static PaginatedList<TDestination> MapPaginatedList<TSource, TDestination>(
        this IMapper mapper, PaginatedList<TSource> source)
    {
        var items = source.Items.Select(i => mapper.Map<TDestination>(i)).ToList();
        var paginatedList = new PaginatedList<TDestination>(items, source.Count, source.PageIndex, source.PageSize);
        return paginatedList;
    }
}
