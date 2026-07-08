using Microsoft.AspNetCore.Http;

namespace Rappd.Api
{
    /// <summary>
    /// Extension methods to simplify result creation.
    /// </summary>
    public static partial class ResultExtensions
    {
        /// <summary>
        /// Created a new <see cref="PagedResult{T}"/>.
        /// </summary>
        /// <typeparam name="T">The element type of the paged result.</typeparam>
        /// <param name="_">The <see cref="IResultExtensions"/> interface used to provide custom result extensions.</param>
        /// <param name="items">The items to return.</param>
        /// <param name="page">The current page.</param>
        /// <param name="pageSize">The number of items in one page.</param>
        /// <param name="totalPages">The total number of pages.</param>
        /// <param name="totalCount">The total number of items.</param>
        /// <returns>A new <see cref="PagedResult{T}"/>.</returns>
        public static PagedResult<T> Paged<T>(this IResultExtensions _, T[] items, int page, int pageSize, long totalPages, long totalCount)
            => new PagedResult<T>(items, page, pageSize, totalPages, totalCount);
    }
}
