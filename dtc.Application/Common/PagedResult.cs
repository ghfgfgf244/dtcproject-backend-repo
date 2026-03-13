namespace dtc.Application.Common
{
    /// <summary>
    /// Standard pagination result wrapper.
    /// Use with any list endpoint that supports paging.
    /// </summary>
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public static PagedResult<T> Create(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize) => new()
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Standard pagination request parameters.
    /// Inherit or use directly in controller query params.
    /// </summary>
    public class PagedRequest
    {
        private int _pageSize = 10;

        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > 100 ? 100 : value; // Cap at 100 to prevent abuse
        }
    }
}
