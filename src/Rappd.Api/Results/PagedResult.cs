using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Reflection;

namespace Rappd.Api
{
    /// <summary>
    /// Represents a array result splitted into pages.
    /// </summary>
    /// <typeparam name="T">The element type of the paged result.</typeparam>
    public class PagedResult<T> : IResult, IEndpointMetadataProvider, IStatusCodeHttpResult, IValueHttpResult, IValueHttpResult<T[]>
    {
        /// <summary>
        /// The items to return.
        /// </summary>
        private readonly T[] _items;
        /// <summary>
        /// The current page and number of items in one page.
        /// </summary>
        private readonly int _page, _pageSize;
        /// <summary>
        /// The total number of pages and the total number of items.
        /// </summary>
        private readonly long _totalPages, _totalCount;

        /// <summary>
        /// The value to return.
        /// </summary>
        public T[]? Value => _items;
        /// <summary>
        /// The status code to return.
        /// </summary>
        public int StatusCode => StatusCodes.Status200OK;

        /// <summary>
        /// The value to return.
        /// </summary>
        object? IValueHttpResult.Value => Value;
        /// <summary>
        /// The status code to return.
        /// </summary>
        int? IStatusCodeHttpResult.StatusCode => StatusCodes.Status200OK;

        /// <summary>
        /// Creates a new <see cref="PagedResult{T}"/>.
        /// </summary>
        /// <param name="items">The items to return.</param>
        /// <param name="page">The current page.</param>
        /// <param name="pageSize">The number of items in one page.</param>
        /// <param name="totalPages">The total number of pages.</param>
        /// <param name="totalCount">The total number of items.</param>
        public PagedResult(T[] items, int page, int pageSize, long totalPages, long totalCount)
            => (_items, _page, _pageSize, _totalPages, _totalCount) = (items, page, pageSize, totalPages, totalCount);

        /// <summary>
        /// Write an HTTP response reflecting the result.
        /// </summary>
        /// <param name="httpContext">The <see cref="HttpContext"/> for the current request.</param>
        /// <returns>A task that represents the asynchronous execute operation.</returns>
        public Task ExecuteAsync(HttpContext context)
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            context.Response.Headers.Append(CustomHttpHeaders.PAGE, _page.ToString());
            context.Response.Headers.Append(CustomHttpHeaders.PAGE_SIZE, _pageSize.ToString());
            context.Response.Headers.Append(CustomHttpHeaders.TOTAL_PAGES, _totalPages.ToString());
            context.Response.Headers.Append(CustomHttpHeaders.TOTAL_COUNT, _totalCount.ToString());

            context.Response.StatusCode = StatusCode;

            return context.Response.WriteAsJsonAsync(_items);
        }

        /// <summary>
        /// Populates metadata for the related <see cref="Endpoint"/> and <see cref="MethodInfo"/>.
        /// </summary>
        /// <remarks>
        /// This method is called by RequestDelegateFactory when creating a <see cref="RequestDelegate"/> and by MVC when creating endpoints for controller actions.
        /// This is called for each parameter and return type of the route handler or action with a declared type implementing this interface.
        /// Add or remove objects on the <see cref="EndpointBuilder.Metadata"/> property of the <paramref name="builder"/> to modify the <see cref="Endpoint.Metadata"/> being built.
        /// </remarks>
        /// <param name="method">The <see cref="MethodInfo"/> of the route handler delegate or MVC Action of the endpoint being created.</param>
        /// <param name="builder">The <see cref="EndpointBuilder"/> used to construct the endpoint for the given <paramref name="method"/>.</param>
        public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(method, nameof(method));
            ArgumentNullException.ThrowIfNull(builder, nameof(builder));

            builder.Metadata.Add(new ProducesResponseTypeAttribute(typeof(T[]), StatusCodes.Status200OK, MediaTypeNames.Application.Json));
        }
    }
}
