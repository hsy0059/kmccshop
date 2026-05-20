using System.ComponentModel.DataAnnotations;

namespace Campus.Common;

public class PageModel
{
    private int _page = 1;
    private int _pageSize = 20;

    [Range(1, int.MaxValue)]
    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    [Range(1, 100)]
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? 20 : (value > 100 ? 100 : value);
    }

    public string? Keyword { get; set; }
    public string? SortBy { get; set; }
    public bool SortDesc { get; set; } = true;
}
