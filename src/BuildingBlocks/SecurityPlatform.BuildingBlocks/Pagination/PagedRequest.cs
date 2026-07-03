namespace SecurityPlatform.BuildingBlocks.Pagination;

public sealed record PagedRequest(int PageNumber = 1, int PageSize = 10)
{
    public const int MaxPageSize = 100;

    public PagedRequest Normalize()
    {
        var pageNumber = PageNumber < 1 ? 1 : PageNumber;
        var pageSize = PageSize < 1 ? 10 : Math.Min(PageSize, MaxPageSize);

        return new PagedRequest(pageNumber, pageSize);
    }

    public int Skip => (PageNumber - 1) * PageSize;
}
