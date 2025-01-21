using Microsoft.AspNetCore.Mvc;

namespace Rappd.Api
{
    /// <summary>
    /// A parameter implementation representing a common search.
    /// </summary>
    /// <typeparam name="T">The type of the searched items.</typeparam>
    /// <param name="Query">The query used to search items.</param>
    /// <param name="Filter">The filter applied to the search.</param>
    /// <param name="Sort">The info used to sort the result.</param>
    /// <param name="Page">The page to return (if null all found items should be returned).</param>
    /// <param name="PageSize">The selected page size (if null a default value should be used).</param>
    public sealed record Search<T>(
        [FromQuery(Name = "query")] string? Query,
        [FromQuery(Name = "filter")] FilterInfo<T>[]? Filter,
        [FromQuery(Name = "sort")] SortInfo<T>? Sort,
        [FromHeader(Name = CustomHttpHeaders.PAGE)] int? Page,
        [FromHeader(Name = CustomHttpHeaders.PAGE_SIZE)] int? PageSize
    );
}