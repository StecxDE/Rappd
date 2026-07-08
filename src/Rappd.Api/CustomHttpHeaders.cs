namespace Rappd.Api
{
    /// <summary>
    /// The class containing all defined custom http headers.
    /// </summary>
    public static class CustomHttpHeaders
    {
        /// <summary>
        /// The name of the http header representing the current page.
        /// </summary>
        public const string PAGE = "X-Page";
        /// <summary>
        /// The name of the http header representing the number of items in one page.
        /// </summary>
        public const string PAGE_SIZE = "X-PageSize";
        /// <summary>
        /// The name of the http header representing the total number of pages.
        /// </summary>
        public const string TOTAL_PAGES = "X-TotalPages";
        /// <summary>
        /// The name of the http header representing the total number of items.
        /// </summary>
        public const string TOTAL_COUNT = "X-TotalCount";
    }
}
