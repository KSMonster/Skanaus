namespace Skanaus.Helpers;

public class SearchParameters
{
    // sort
    // shaping
    private int _pageSize = 2;
    private int _pageNumber = 1;

    private const int MaxPageSize = 50;
    
    public int? PageNumber { get => _pageNumber; set => _pageNumber = value ?? _pageNumber; }
    public int? PageSize { get=> _pageSize; set => _pageSize = value ?? _pageSize; }
    
}